// ==============================================================
//
// abstract class HockeyTeam
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
//  Rules of the road for generating a team roster:
//  1. Which players are available? From the survey.
//  2. No more than 10+1(G) players per team.
//  3. Specific full-time players stay with specific teams (Ben and Barry).
//  4. Unaffiliated players can be assigned to any team.
//  5. Remaining full-time assignments next; then subs.
//  6. Each team has one goalie.
//  7. Team skill scores must be very close.
// ==============================================================

using System;
using System.Collections;
using System.Linq;

namespace Hammertime
{
    public abstract class HockeyTeam
    {
        protected static ArrayList _availableFullTimePlayers;
        protected static ArrayList _availableSubPlayers;

        public enum Residence
        {
            Home,
            Away
        }

        public int FullTimePlayerCount  { get { return _availableFullTimePlayers.Count; } }
        public int SubPlayerCount       { get { return _availableSubPlayers.Count; } }
        public Residence Location       { get; set; }
        private static bool RostersInitialized { get; set; }

        // ==============================================================
        public HockeyTeam(Residence residence)
        // ==============================================================
        {
            // Establish home/away (white/dark)
            Location = residence;
            // Build the available player rosters from Teamopolis
            BuildAvailablePlayerRosters();
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
                string mySqlQuery = $"update mondaynighthockey.players set player_last_wk=\"{player.PlayerLastWeek}\" where player_id={player.PlayerID}";
                teamsSaved = connection.Update(mySqlQuery);
                if (teamsSaved == false) break;
            }

            if (teamsSaved == true)
                Console.WriteLine("\nTeams written to the database.\n");

            return teamsSaved;
        }

        // ==============================================================
        public static HockeyTeam StrongerTeam(HomeTeam home, VisitorTeam visitor)
        // ==============================================================
        {
            int[] homeComposition = home.TeamComposition;
            int[] VisitorComposition = visitor.TeamComposition;

            if (homeComposition[0] == VisitorComposition[0])
            {
                if (homeComposition[1] == VisitorComposition[1])
                {
                    if (homeComposition[2] == VisitorComposition[2])
                    {
                        if (homeComposition[3] >= VisitorComposition[3])
                            return home;
                        else
                            return visitor;
                    }
                    else if (homeComposition[2] > VisitorComposition[2])
                        return home;
                    else
                        return visitor;
                }
                else if (homeComposition[1] > VisitorComposition[1])
                    return home;
                else
                    return visitor;
            }
            else if (homeComposition[0] > VisitorComposition[0])
                return home;
            else
                return visitor;

        }
        // ==============================================================
        protected int[] TeamComposition(ArrayList teamRoster)
        // ==============================================================
        {
            int[] composition = new int[] { 0, 0, 0, 0 };

            var query = from HockeyPlayer player in teamRoster
                        select player;

            foreach (HockeyPlayer player in query)
            {
                switch (player.Level)
                {
                    case HockeyPlayer.PlayerSkill.Level_A:
                        composition[0]++; ;
                        break;
                    case HockeyPlayer.PlayerSkill.Level_B:
                        composition[1]++; ;
                        break;
                    case HockeyPlayer.PlayerSkill.Level_C:
                        composition[2]++; ;
                        break;
                    case HockeyPlayer.PlayerSkill.Level_D:
                    default:
                        composition[3]++; ;
                        break;
                }
            }

            return composition;
        }


        // ==============================================================
        protected int TeamScore(ArrayList teamRoster)
        // ==============================================================
        {
            int score = 0;

            var query = from HockeyPlayer player in teamRoster
                        select player;

            foreach (HockeyPlayer player in query)
            {
                switch (player.Level)
                {
                    case HockeyPlayer.PlayerSkill.Level_A:
                        score += (int)HockeyPlayer.PlayerValue.Level_A;
                        break;
                    case HockeyPlayer.PlayerSkill.Level_B:
                        score += (int)HockeyPlayer.PlayerValue.Level_B;
                        break;
                    case HockeyPlayer.PlayerSkill.Level_C:
                        score += (int)HockeyPlayer.PlayerValue.Level_C;
                        break;
                    case HockeyPlayer.PlayerSkill.Level_D:
                    default:
                        score += (int)HockeyPlayer.PlayerValue.Level_D;
                        break;
                }
            }

            return score;
        }

        // ==============================================================
        protected void PrintTeamRoster(ArrayList teamRoster)
        // ==============================================================
        {
            string formatString = "{0,22}{1,10} |{2,7} |{3,10} |{4,15}";
            string teamId;

            if (Location == Residence.Home)
            {
                teamId = "Home Team (Black)";
            }
            else
            {
                teamId = "Visiting Team (White)";

            }

            Console.WriteLine("==========================================================================");
            Console.WriteLine(formatString, teamId, "Player", "Level", "Position", "Team");
            Console.WriteLine("==========================================================================");

            string level;
            formatString = "{0,32} |{1,7} |{2,10} |{3,15}";

            var query = from HockeyPlayer player in teamRoster
                        select player;

            foreach (HockeyPlayer player in query)
            {
                switch (player.Level)
                {
                    case HockeyPlayer.PlayerSkill.Level_A:
                        level = "A";
                        break;
                    case HockeyPlayer.PlayerSkill.Level_B:
                        level = "B";
                        break;
                    case HockeyPlayer.PlayerSkill.Level_C:
                        level = "C";
                        break;
                    case HockeyPlayer.PlayerSkill.Level_D:
                    default:
                        level = "D";
                        break;
                }

                string playerName = player.FirstName + " " + player.LastName;
                Console.WriteLine(formatString, playerName, level, player.PlayerPos, player.PlayerTeam);
            }

            Console.WriteLine();
            int[] teamComposition = TeamComposition(teamRoster);
            Console.WriteLine($"Team talent composition: {teamComposition[0]}{teamComposition[1]}{teamComposition[2]}{teamComposition[3]}");
            Console.WriteLine($"Team talent score: {TeamScore(teamRoster)}");
            Console.WriteLine();
        }

        // Private
        private HockeyTeam() {; }
        private HockeyTeam(HockeyTeam team) // Copy constructor
        {
            Location = team.Location;
        }

        // ==============================================================
        // Add/Remove players from the roster
        // ==============================================================
        public abstract HockeyPlayer GetASkillPlayer(HockeyPlayer.PlayerSkill skillLevel);
        public abstract void AddAPlayer(HockeyPlayer hockeyPlayer);     // Provided in team classes
        public abstract void RemoveAPlayer(HockeyPlayer hockeyPlayer);  // Provided in team classes
        // ==============================================================

        // ==============================================================
        // Add a goalie to the roster if one is available.
        // First try the available full time roster players.
        // If nothing available try the available sub players.
        // Return true if added; false if not added.
        // ==============================================================
        public abstract bool AddAGoalie(bool strongTeam);   // Provided in team classes
        // ==============================================================
        protected bool AddAGoalie(ArrayList teamRoster, bool strongTeam)    // Called from team classes
        // ==============================================================
        {
            bool goalieAdded = false;

            var query = from HockeyPlayer player in _availableFullTimePlayers
                        select player;

            foreach (HockeyPlayer player in query)
            {
                if ((player.AssignedToTeam == false) &&
                    (player.PlayerPos == "Goalie"))
                {
                    //Console.WriteLine("AddAGoalie from _availableFullTimePlayers");
                    goalieAdded = true;
                    player.AssignedToTeam = true;
                    teamRoster.Add(player);

                    if (Location == Residence.Home)
                        player.PlayerLastWeek = "Black";
                    else
                        player.PlayerLastWeek = "White";
                }

                if (goalieAdded) break;
            }

            if (goalieAdded == false)
            {
                query = from HockeyPlayer player in _availableSubPlayers
                        select player;

                foreach (HockeyPlayer player in query)
                {
                    if ((player.AssignedToTeam == false) &&
                        (player.PlayerPos == "Goalie"))
                    {
                        //Console.WriteLine("AddAGoalie from _availableSubPlayers");
                        goalieAdded = true;
                        player.AssignedToTeam = true;
                        teamRoster.Add(player);
                    }

                    if (goalieAdded) break;
                }
            }
            /*
            if (goalieAdded == false)
                Console.WriteLine("Goalie not added to team.");
            */
            return goalieAdded;
        }

        // ==============================================================
        // Add a skill level player to the roster if one is available.
        // First try the available full time roster players.
        // If nothing available try the available sub players.
        // Return true if added; false if not added.
        // ==============================================================
        public abstract bool AddASkillPlayer(HockeyPlayer.PlayerSkill skillLevel);  // Provided in team classes
        // ==============================================================
        protected bool AddASkillPlayer(ArrayList teamRoster, HockeyPlayer.PlayerSkill skillLevel)    // Called from team classes
        // ==============================================================
        {
            bool playerAdded = false;

            var query = from HockeyPlayer player in _availableFullTimePlayers
                        select player;

            foreach (HockeyPlayer player in query)
            {
                if ((player.AssignedToTeam == false) &&
                    (player.PlayerPos != "Goalie") &&      // Save Goalies to the end
                    (player.Level == skillLevel))
                {
                    //Console.WriteLine("AddASkillPlayer from _availableFullTimePlayers");
                    playerAdded = true;
                    player.AssignedToTeam = true;
                    teamRoster.Add(player);

                    if (Location == Residence.Home)
                        player.PlayerLastWeek = "Black";
                    else
                        player.PlayerLastWeek = "White";
                }

                if (playerAdded) break;
            }

            if (playerAdded == false)
            {
                query = from HockeyPlayer player in _availableSubPlayers
                        select player;

                foreach (HockeyPlayer player in query)
                {
                    if ((player.AssignedToTeam == false) &&
                        (player.PlayerPos != "Goalie") &&      // Save Goalies to the end
                        (player.Level == skillLevel))
                    {
                        //Console.WriteLine("AddASkillPlayer from _availableSubPlayers");
                        playerAdded = true;
                        player.AssignedToTeam = true;
                        teamRoster.Add(player);
                    }

                    if (playerAdded) break;
                }
            }

            return playerAdded;
        }

        // ==============================================================
        // Abstract method to build team roster based on residence (home/away)
        protected abstract void BuildTeamRoster();

        // ==============================================================
        // Abstract property to get count of players on roster (home/away)
        public abstract int PlayerCount { get; }

        // ==============================================================
        private void BuildAvailablePlayerRosters()
        // ==============================================================
        {
            if (RostersInitialized == false)
            {
                //Console.WriteLine("Building the master roster.");
                _availableFullTimePlayers = new ArrayList();
                _availableSubPlayers = new ArrayList();

                DbConnection myDbConnection = HammerMainDb.getInstance();
                ArrayList _availablePlayers = TeamopolisReader.Instance.AvailablePlayers;

                foreach (string player in _availablePlayers)
                {
                    HockeyPlayer dbPlayer = myDbConnection.Read(player);
                    if (dbPlayer != null)
                    {
                        if (dbPlayer.PlayerType == 'F')
                            _availableFullTimePlayers.Add(dbPlayer);
                        else
                            _availableSubPlayers.Add(dbPlayer);
                    }
                }

                RostersInitialized = true;
            }
        }
    }
}