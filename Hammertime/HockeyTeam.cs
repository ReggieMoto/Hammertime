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
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        protected void printTeamRoster(HockeyPlayer[] teamRoster)
        {
            string formatString = "{0,30} |{1,7} |{2,10} |{3,15}";
            Console.WriteLine("==========================================================================");
            Console.WriteLine(formatString, "Player", "Level", "Position", "Team");
            Console.WriteLine("==========================================================================");

            int score = 0;
            string level;

            foreach (HockeyPlayer player in teamRoster)
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
        protected int MasterRosterPlayerCount { get; set; }
        private static bool MasterRosterInitialized { get; set; }

        // Private
        private HockeyTeam() {; }
        private HockeyTeam(HockeyTeam team) // Copy constructor
        {
            Location = team.Location;
            MasterRosterPlayerCount = team.MasterRosterPlayerCount;
        }

        protected abstract void buildTeamRoster();

        private void buildMasterRoster()
        {
            if (MasterRosterInitialized == true)
                return;

            MasterRosterInitialized = true;
            Console.WriteLine("Building the master roster.");

            // Build a roster from the server based on residence (home/away)
            DbConnection myDbConnection = DbConnection.getInstance();

            MasterRosterPlayerCount = myDbConnection.Count();       // Number of records

            _masterRoster = new HockeyPlayer[MasterRosterPlayerCount];        // Number of Hockey Players
            List<string>[] dbRecords = myDbConnection.Select(); // Read the records out of the DB

            for (int index=0; index < MasterRosterPlayerCount; index++)          
            {
                int playerID;
                int.TryParse(dbRecords[0][index], out playerID);

                char level = dbRecords[3][index][0];
                HockeyPlayer.PlayerSkill skillLevel;

                if (level == 'D')
                    skillLevel = HockeyPlayer.PlayerSkill.Level_D;
                else if (level == 'C')
                    skillLevel = HockeyPlayer.PlayerSkill.Level_C;
                else if (level == 'B')
                    skillLevel = HockeyPlayer.PlayerSkill.Level_B;
                else // (level == 'A')
                    skillLevel = HockeyPlayer.PlayerSkill.Level_A;

                _masterRoster[index] = new Hammertime.HockeyPlayer(
                    playerID,
                    dbRecords[1][index],    // Last Name
                    dbRecords[2][index],    // First Name
                    skillLevel,             // Level
                    dbRecords[4][index],    // Position
                    dbRecords[5][index][0], // Type
                    dbRecords[6][index],    // Team
                    dbRecords[7][index]     // Last week's team
                    );
            }
        }

        protected static HockeyPlayer[] _masterRoster;
    }
}