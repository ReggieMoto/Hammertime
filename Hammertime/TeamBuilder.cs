// ==============================================================
//
// class TeamBuilder
//
// Copyright (c) 2017 David Hammond 
// All Rights Reserved.
// 
// ==============================================================
// NOTICE:  All information contained herein is and remains the
// property of David Hammond. The intellectual and technical
// concepts contained herein are proprietary to David Hammond
// and may be covered by U.S.and Foreign Patents, patents in
// process, and are protected by trade secret or copyright law.
// Dissemination of this information or reproduction of this
// material is strictly forbidden unless prior written permission
// is obtained David Hammond.
// ==============================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hammertime
{
    public class TeamBuilderException : System.Exception
    {
        string _message;

        // ==============================================================
        public TeamBuilderException(string message) : base(message)
        // ==============================================================
        {
            _message = message;
        }
    }

    public class TeamBuilder
    {
        private static TeamBuilder _teamBuilder = null;

        // ==============================================================
        public static TeamBuilder Instance
        // ==============================================================
        {
            get
            {
                if (_teamBuilder == null)
                    _teamBuilder = new TeamBuilder();
                return _teamBuilder;
            }
        }

        // Player lists sorted on skill level A -> D
        private static List<HockeyPlayer> _availableFullTimePlayers;
        private static List<HockeyPlayer> _availableSubPlayers;

        private List<HockeyPlayer> _captains;

        // ==============================================================
        private TeamBuilder()
        //
        // Initialize the lists of subs and full-time players
        // Fetch the players and fill the lists
        // ==============================================================
        {
            _availableFullTimePlayers = new List<HockeyPlayer>();
            _availableSubPlayers = new List<HockeyPlayer>();
            _captains = new List<HockeyPlayer>();

            FetchAvailablePlayers();
            SortAvailablePlayers();
        }

        public List<HockeyPlayer> GetAvailableFullTimePlayers() { return _availableFullTimePlayers; }
        public List<HockeyPlayer> GetAvailableSubPlayers() { return _availableSubPlayers; }

        // ==============================================================
        private void FetchAvailablePlayers()
        //
        // Currently being fetched from teamopolis.
        // Should be configurable
        // ==============================================================
        {
            DbConnection myDbConnection = HammerMainDb.getInstance();
            List<string> _availablePlayers = TeamopolisReader.Instance.AvailablePlayers;

            foreach (string player in _availablePlayers)
            {
                HockeyPlayer dbPlayer = myDbConnection.Read(player);
                if (dbPlayer != null)
                {
                    dbPlayer.AssignedToTeam = false;
                    if (dbPlayer.PlayerType == 'F')
                        _availableFullTimePlayers.Add(dbPlayer);
                    else
                        _availableSubPlayers.Add(dbPlayer);
                }
            }
        }

        // ==============================================================
        private void SortAvailablePlayers()
        // ==============================================================
        {
            // First sort the players by skill level;
            _availableFullTimePlayers.Sort();
            _availableSubPlayers.Sort();

            // Next place the sorted players into the position queues
            foreach (HockeyPlayer player in _availableFullTimePlayers)
                if (player.Captain == true || player.AltCaptain == true)
                    _captains.Add(player);
        }

        // ==============================================================
        public void TeamAssign(HockeyPlayer player)
        // ==============================================================
        {
            HomeTeam home = HomeTeam.Instance;       // The home team is the "black" team
            VisitorTeam away = VisitorTeam.Instance; // The away team is the "white" team

            if (player != null)
            {
                if (player.Captain == true)
                {
                    if (player.PlayerLastWeek == "White")   // Last week away, this week home
                        home.AddAPlayer(player);
                    else if (player.PlayerLastWeek == "Black")   // Last week home, this week away
                        away.AddAPlayer(player);
                    else
                    {
                    }
                }
                else if (player.AltCaptain == true)
                {
                    if (player.PlayerLastWeek == "White")
                        home.AddAPlayer(player);
                    else if (player.PlayerLastWeek == "Black")
                        away.AddAPlayer(player);
                    else
                    {
                    }
                }
                else // Players not captains or alt-captains
                {
                    int[] homeTeamComp = home.TeamComposition;
                    int[] awayTeamComp = away.TeamComposition;
                    bool assignHome = false;

                    switch (player.Level)
                    {
                        case HockeyPlayer.PlayerSkill.Level_A:
                            if ((home.PlayerCount < away.PlayerCount) &&
                                (homeTeamComp[0] < awayTeamComp[0]))
                                assignHome = true;
                            break;

                        case HockeyPlayer.PlayerSkill.Level_B:
                            if ((home.PlayerCount <= away.PlayerCount) &&
                                ((homeTeamComp[0] <= awayTeamComp[0]) ||
                                (homeTeamComp[1] <= awayTeamComp[1])))
                                assignHome = true;
                            break;

                        case HockeyPlayer.PlayerSkill.Level_C:
                            if ((home.PlayerCount <= away.PlayerCount) &&
                                ((homeTeamComp[0] <= awayTeamComp[0]) ||
                                (homeTeamComp[1] <= awayTeamComp[1]) ||
                                (homeTeamComp[2] <= awayTeamComp[2])))
                                assignHome = true;
                            break;

                        case HockeyPlayer.PlayerSkill.Level_D:
                            /*
                            if ((homeTeamComp[3] < awayTeamComp[3]) &&
                                (home.TeamScore < away.TeamScore))
                                */
                            if ((home.PlayerCount <= away.PlayerCount) &&
                                ((homeTeamComp[0] <= awayTeamComp[0]) ||
                                (homeTeamComp[1] <= awayTeamComp[1]) ||
                                (homeTeamComp[2] <= awayTeamComp[2]) ||
                                (homeTeamComp[3] <= awayTeamComp[3])))
                                assignHome = true;
                            break;
                    }

                    if (assignHome == true)
                        home.AddAPlayer(player);
                    else
                        away.AddAPlayer(player);
                }
            }
        }

        // ==============================================================
        public void BuildTeams()
        // ==============================================================
        {
            // ==============================================================
            // Captains and Alt Captains
            // ==============================================================
            var query = from player in _availableFullTimePlayers
                        where player.Captain == true            // Captain
                        where player.PlayerLastWeek != "Zed"    // Captain is either black or white
                        select player;

            foreach (var player in query)
                TeamAssign(player);

            query = from player in _availableFullTimePlayers
                    where player.AltCaptain == true         // Alt Captain
                    where player.PlayerLastWeek != "Zed"    // Alt Captain is either black or white
                    select player;

            foreach (var player in query)
                TeamAssign(player);

            query = from player in _availableFullTimePlayers
                    where player.Captain == true            // Captain
                    where player.PlayerLastWeek == "Zed"    // Captain is neither black nor white
                    select player;

            foreach (var player in query)
                TeamAssign(player);

            query = from player in _availableFullTimePlayers
                    where player.AltCaptain == true         // Alt Captain
                    where player.PlayerLastWeek == "Zed"    // Alt Captain is neither black nor white
                    select player;

            foreach (var player in query)
                TeamAssign(player);

            // ==============================================================
            // End of the captains. Now for the players.
            // ==============================================================
            query = from player in _availableFullTimePlayers
                    where player.Captain != true            // Not Captain
                    where player.AltCaptain != true         // Not Alt Captain
                    where player.PlayerPos != "Goalie"      // Not a goalie
                    where player.Level == HockeyPlayer.PlayerSkill.Level_A
                    select player;

            foreach (var player in query)
                TeamAssign(player);

            query = from player in _availableFullTimePlayers
                    where player.Captain != true            // Not Captain
                    where player.AltCaptain != true         // Not Alt Captain
                    where player.PlayerPos != "Goalie"      // Not a goalie
                    where player.Level == HockeyPlayer.PlayerSkill.Level_B
                    select player;

            foreach (var player in query)
                TeamAssign(player);

            query = from player in _availableFullTimePlayers
                    where player.Captain != true            // Not Captain
                    where player.AltCaptain != true         // Not Alt Captain
                    where player.PlayerPos != "Goalie"      // Not a goalie
                    where player.Level == HockeyPlayer.PlayerSkill.Level_C
                    select player;

            foreach (var player in query)
                TeamAssign(player);

            query = from player in _availableFullTimePlayers
                    where player.Captain != true            // Not Captain
                    where player.AltCaptain != true         // Not Alt Captain
                    where player.PlayerPos != "Goalie"      // Not a goalie
                    where player.Level == HockeyPlayer.PlayerSkill.Level_D
                    select player;

            foreach (var player in query)
                TeamAssign(player);

            // ==============================================================
            // Subs
            // ==============================================================
            foreach (HockeyPlayer player in _availableSubPlayers)
                if (player.PlayerPos != "Goalie")
                    TeamAssign(player);
        }

        // ==============================================================
        public static bool SaveTeams()
        // ==============================================================
        {
            bool teamsSaved = false;

            DbConnection connection = HammerMainDb.getInstance();

            var query = from HockeyPlayer player in _availableFullTimePlayers select player;
            foreach (HockeyPlayer player in query)
            {
                teamsSaved = connection.Update(player);
                if (teamsSaved == false) break;
            }

            if (teamsSaved == true)
                Console.WriteLine("\nTeams written to the database.\n");

            return teamsSaved;
        }
    }
}
