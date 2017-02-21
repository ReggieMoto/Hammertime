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
using System.Collections.Generic;
using System.Linq;
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

        // Public
        public HockeyPlayer(
            int player_id,
            string player_last_name,
            string player_first_name,
            PlayerSkill player_level,
            string player_position,
            char player_type,
            string player_team,
            string player_last_wk)
        {
            PlayerID = player_id;
            LastName = player_last_name;
            FirstName = player_first_name;
            Level = player_level;
            PlayerPos = player_position;
            PlayerType = player_type;
            PlayerTeam = player_team;
            PlayerLastWeek = player_last_wk;
        }

        public int PlayerID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public PlayerSkill Level { get; set; }
        public string PlayerPos { get; set; }
        public char PlayerType { get; set; }        // Full time, Sub
        public string PlayerTeam { get; set; }      // Ben, Barry, Unaffiliated
        public string PlayerLastWeek { get; set; }  // White, Black, Zed (Didn't play)

        public  HockeyPlayer(HockeyPlayer player)
        {
            LastName = player.LastName;
            PlayerID = player.PlayerID;
            FirstName = player.FirstName;
            Level = player.Level;
            PlayerPos = player.PlayerPos;
            PlayerType = player.PlayerType;
            PlayerTeam = player.PlayerTeam;
            PlayerLastWeek = player.PlayerLastWeek;
        }
    }
}