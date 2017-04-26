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
                    if (player.PlayerLastWeek == "White")
                        home.AddAPlayer(player);
                    else if (player.PlayerLastWeek == "Black")
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
                else
                {
                    if (home.PlayerCount <= away.PlayerCount)
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
            bool singleList = true;

            if (singleList == true)
            {
                query = from player in _availableFullTimePlayers
                        where player.Captain != true        // Not Captain
                        where player.AltCaptain != true     // Not Alt Captain
                        where player.PlayerPos != "Goalie"  // Not a goalie
                        select player;

                foreach (var player in query)
                    TeamAssign(player);
            }
            else
            {
                // ==============================================================
                // Defense
                // ==============================================================
                query = from player in _availableFullTimePlayers
                        where player.Captain != true            // Not Captain
                        where player.AltCaptain != true         // Not Alt Captain
                        where player.PlayerPos == "Defense"
                        where player.Level == HockeyPlayer.PlayerSkill.Level_A
                        select player;

                foreach (var player in query)
                    TeamAssign(player);

                query = from player in _availableFullTimePlayers
                        where player.Captain != true            // Not Captain
                        where player.AltCaptain != true         // Not Alt Captain
                        where player.PlayerPos == "Defense"
                        where player.Level == HockeyPlayer.PlayerSkill.Level_B
                        select player;

                foreach (var player in query)
                    TeamAssign(player);

                query = from player in _availableFullTimePlayers
                        where player.Captain != true            // Not Captain
                        where player.AltCaptain != true         // Not Alt Captain
                        where player.PlayerPos == "Defense"
                        where player.Level == HockeyPlayer.PlayerSkill.Level_C
                        select player;

                foreach (var player in query)
                    TeamAssign(player);

                query = from player in _availableFullTimePlayers
                        where player.Captain != true            // Not Captain
                        where player.AltCaptain != true         // Not Alt Captain
                        where player.PlayerPos == "Defense"
                        where player.Level == HockeyPlayer.PlayerSkill.Level_D
                        select player;

                foreach (var player in query)
                    TeamAssign(player);

                // ==============================================================
                // Forwards
                // ==============================================================
                query = from player in _availableFullTimePlayers
                        where player.Captain != true            // Not Captain
                        where player.AltCaptain != true         // Not Alt Captain
                        where player.PlayerPos == "Forward"
                        where player.Level == HockeyPlayer.PlayerSkill.Level_A
                        select player;

                foreach (var player in query)
                    TeamAssign(player);

                query = from player in _availableFullTimePlayers
                        where player.Captain != true            // Not Captain
                        where player.AltCaptain != true         // Not Alt Captain
                        where player.PlayerPos == "Forward"
                        where player.Level == HockeyPlayer.PlayerSkill.Level_B
                        select player;

                foreach (var player in query)
                    TeamAssign(player);

                query = from player in _availableFullTimePlayers
                        where player.Captain != true            // Not Captain
                        where player.AltCaptain != true         // Not Alt Captain
                        where player.PlayerPos == "Forward"
                        where player.Level == HockeyPlayer.PlayerSkill.Level_C
                        select player;

                foreach (var player in query)
                    TeamAssign(player);

                query = from player in _availableFullTimePlayers
                        where player.Captain != true            // Not Captain
                        where player.AltCaptain != true         // Not Alt Captain
                        where player.PlayerPos == "Forward"
                        where player.Level == HockeyPlayer.PlayerSkill.Level_D
                        select player;

                foreach (var player in query)
                    TeamAssign(player);

            }

            foreach (HockeyPlayer player in _availableSubPlayers)
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
