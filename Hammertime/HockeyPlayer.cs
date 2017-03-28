// ==============================================================
//
// class HockeyPlayer
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
using System.Text;

namespace Hammertime
{
    // ====================
    public class HockeyPlayer
    // ====================
    {
        public enum PlayerAttributes
        {
            ID,
            LastName,
            FirstName,
            Level,
            Position,
            Goalie,
            Type,
            Team,
            LastWeek
        }

        public enum PlayerSkill
        {
            Level_D = 1,
            Level_C = 10,
            Level_B = 100,
            Level_A = 1000
        }

        public enum PlayerValue
        {
            Level_D = 1,
            Level_C = 2,
            Level_B = 3,
            Level_A = 4
        }

        // Public
        // ==============================================================
        public HockeyPlayer(
            int player_id,
            string player_last_name,
            string player_first_name,
            PlayerSkill player_level,
            string player_position,
            bool player_goalie,
            char player_type,
            string player_team,
            string player_last_wk)
        // ==============================================================
        {
            PlayerID = player_id;
            LastName = player_last_name;
            FirstName = player_first_name;
            Level = player_level;
            PlayerPos = player_position;
            Goalie = player_goalie;
            PlayerType = player_type;
            PlayerTeam = player_team;
            PlayerLastWeek = player_last_wk;
            AssignedToTeam = false;

            switch (Level)
            {
                case PlayerSkill.Level_A:
                    PlayerScore = PlayerValue.Level_A;
                    break;
                case PlayerSkill.Level_B:
                    PlayerScore = PlayerValue.Level_B;
                    break;
                case PlayerSkill.Level_C:
                    PlayerScore = PlayerValue.Level_C;
                    break;
                case PlayerSkill.Level_D:
                default:
                    PlayerScore = PlayerValue.Level_D;
                    break;
            }
        }

        public int PlayerID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public PlayerSkill Level { get; set; }
        public string PlayerPos { get; set; }           // Player's normal position
        public bool Goalie { get; set; }                // Also can play goalie
        public char PlayerType { get; set; }            // Full time, Sub
        public string PlayerTeam { get; set; }          // Ben, Barry, Unaffiliated
        public string PlayerLastWeek { get; set; }      // White, Black, Zed (Didn't play)
        public bool AssignedToTeam { get; set; }        // Is the player assigned to a team yet
        public PlayerValue PlayerScore { get; set; }    // Derived from Skill Level

        // ==============================================================
        public HockeyPlayer(HockeyPlayer player)
        // ==============================================================
        {
            LastName = player.LastName;
            PlayerID = player.PlayerID;
            FirstName = player.FirstName;
            Level = player.Level;
            PlayerPos = player.PlayerPos;
            Goalie = player.Goalie;
            PlayerType = player.PlayerType;
            PlayerTeam = player.PlayerTeam;
            PlayerLastWeek = player.PlayerLastWeek;
            AssignedToTeam = player.AssignedToTeam;
            PlayerScore = player.PlayerScore;
        }
    }
}