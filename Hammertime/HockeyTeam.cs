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

using System;
using System.Collections;
using System.Linq;

namespace Hammertime
{
    public abstract class HockeyTeam
    {
        public enum Residence
        {
            Home,
            Away
        }

        // Public
        public HockeyTeam(Residence residence)
        {
            // Establish home/away (white/dark)
            Location = residence;

           // Build the master roster
            buildMasterRoster();
        }

        protected void printTeamRoster(ArrayList teamRoster)
        {
            string formatString = "{0,22}{1,10} |{2,7} |{3,10} |{4,15}";
            string teamId;

            if (Location == Residence.Home)
            {
                teamId = "Home Team (White)";
            }
            else
            {
                teamId = "Visiting Team (Dark)";

            }

            Console.WriteLine("==========================================================================");
            Console.WriteLine(formatString, teamId, "Player", "Level", "Position", "Team");
            Console.WriteLine("==========================================================================");

            int score = 0;
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
                        score += 1000;
                        break;
                    case HockeyPlayer.PlayerSkill.Level_B:
                        level = "B";
                        score += 100;
                        break;
                    case HockeyPlayer.PlayerSkill.Level_C:
                        level = "C";
                        score += 10;
                        break;
                    case HockeyPlayer.PlayerSkill.Level_D:
                    default:
                        level = "D";
                        score += 1;
                        break;
                }

                string playerName = player.FirstName + " " + player.LastName;
                Console.WriteLine(formatString, playerName, level, player.PlayerPos, player.PlayerTeam);
            }

            Console.WriteLine();
            Console.WriteLine($"Team talent score: {score}");
            Console.WriteLine();
        }

        public Residence Location { get; set; }
        private static bool MasterRosterInitialized { get; set; }

        // Private
        private HockeyTeam() {; }
        private HockeyTeam(HockeyTeam team) // Copy constructor
        {
            Location = team.Location;
        }

        // Abstract method to build team roster based on residence (home/away)
        protected abstract void buildTeamRoster();

        private void buildMasterRoster()
        {
            if (MasterRosterInitialized == false)
            {
                Console.WriteLine("Retrieving the master roster.");

                DbConnection myDbConnection = DbConnection.getInstance();
                _masterRoster = myDbConnection.Select();            // Read the records out of the DB

                MasterRosterInitialized = true;
            }
        }

        protected static ArrayList _masterRoster;
    }
}