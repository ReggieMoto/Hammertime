// ==============================================================
//
// sealed class HomeTeam
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

using System.Collections.Generic;

namespace Hammertime
{
    public sealed class HomeTeam : HockeyTeam
    {
        private static HomeTeam _homeTeam = null;

        // ==============================================================
        public static HomeTeam Instance
        // ==============================================================
        {
            get
            {
                if (_homeTeam == null)
                    _homeTeam = new HomeTeam();
                return _homeTeam;
            }
        }

        private List<HockeyPlayer> _homeRoster = null;

        // ==============================================================
        private HomeTeam()
        // ==============================================================
            : base(Residence.Home)
        {
            _homeRoster = new List<HockeyPlayer>();
        }

        // ==============================================================
        public override int[] TeamComposition
        // ==============================================================
        {
            get { return GetTeamComposition(_homeRoster); }
        }

        // ==============================================================
        public override int TeamScore
        // ==============================================================
        {
            get { return GetTeamScore(_homeRoster); }
        }

        // ==============================================================
        public override int PlayerCount
        // ==============================================================
        {
            get { return _homeRoster.Count; }
        }

        // ==============================================================
        public override void PrintRoster()
        // ==============================================================
        {
            PrintTeamRoster(_homeRoster);
        }

        // ==============================================================
        public override bool AddASkillPlayer(HockeyPlayer.PlayerSkill skillLevel)
        // ==============================================================
        {
            //Console.WriteLine("AddASkillPlayer for the home team");
            return AddASkillPlayer(_homeRoster, skillLevel);
        }

        // ==============================================================
        public override void AddAPlayer(HockeyPlayer hockeyPlayer)
        // ==============================================================
        {
            //Console.WriteLine("AddAPlayer for the home team");
            hockeyPlayer.AssignedToTeam = true;
            hockeyPlayer.PlayerLastWeek = "Black";    // This week the player will be on the home "black" team
            _homeRoster.Add(hockeyPlayer);
        }

        // ==============================================================
        public override void RemoveAPlayer(HockeyPlayer hockeyPlayer)
        // ==============================================================
        {
            //Console.WriteLine("RemoveAPlayer from the home team");
            hockeyPlayer.AssignedToTeam = false;
            _homeRoster.Remove(hockeyPlayer);
        }

        // ==============================================================
        public override HockeyPlayer GetASkillPlayer(HockeyPlayer.PlayerSkill skillLevel)
        // ==============================================================
        {
            HockeyPlayer skillPlayer = null;

            foreach (HockeyPlayer player in _homeRoster)
            {
                if ((player.PlayerTeam == "Unaffiliated") &&
                    (player.PlayerPos != "Goalie") &&
                    (player.Level == skillLevel))
                {
                    skillPlayer = player;
                    break;
                }
            }

            return skillPlayer;
        }

        // ==============================================================
        public override bool AddAGoalie(bool strongerTeam)
        // ==============================================================
        {
            //Console.WriteLine("AddASkillPlayer for the home team");
            return AddAGoalie(_homeRoster, strongerTeam);
        }
    }
}