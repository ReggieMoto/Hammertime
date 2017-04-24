// ==============================================================
//
// sealed class VisitorTeam
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
    public sealed class VisitorTeam : HockeyTeam
    {
        private static VisitorTeam _visitorTeam = null;

        // ==============================================================
        public static VisitorTeam Instance
        // ==============================================================
        {
            get
            {
                if (_visitorTeam == null)
                    _visitorTeam = new VisitorTeam();
                return _visitorTeam;
            }
        }

        private List<HockeyPlayer> _visitorRoster;

        // ==============================================================
        private VisitorTeam()
        // ==============================================================
            : base(Residence.Away)
        {
            _visitorRoster = new List<HockeyPlayer>();
        }

        // ==============================================================
        public void PrintVisitingTeamRoster()
        // ==============================================================
        {
            PrintTeamRoster(_visitorRoster);
        }

        // ==============================================================
        public new int TeamScore
        // ==============================================================
        {
            get { return TeamScore(_visitorRoster); }
        }

        // ==============================================================
        public new int[] TeamComposition
        // ==============================================================
        {
            get { return TeamComposition(_visitorRoster); }
        }

        // ==============================================================
        public override int PlayerCount
        // ==============================================================
        {
            get { return _visitorRoster.Count; }
        }

        // ==============================================================
        public override bool AddASkillPlayer(HockeyPlayer.PlayerSkill skillLevel)
        // ==============================================================
        {
            //Console.WriteLine("AddASkillPlayer for the visitor team");
            return AddASkillPlayer(_visitorRoster, skillLevel);
        }

        // ==============================================================
        public override void AddAPlayer(HockeyPlayer hockeyPlayer)
        // ==============================================================
        {
            //Console.WriteLine("AddAPlayer for the visitor team");
            hockeyPlayer.AssignedToTeam = true;
            hockeyPlayer.PlayerLastWeek = "White";    // This week the player will be on the home "black" team
            _visitorRoster.Add(hockeyPlayer);
        }

        // ==============================================================
        public override void RemoveAPlayer(HockeyPlayer hockeyPlayer)
        // ==============================================================
        {
            //Console.WriteLine("RemoveAPlayer from the visitor team");
            _visitorRoster.Remove(hockeyPlayer);
            hockeyPlayer.AssignedToTeam = false;
        }

        // ==============================================================
        public override HockeyPlayer GetASkillPlayer(HockeyPlayer.PlayerSkill skillLevel)
        // ==============================================================
        {
            HockeyPlayer skillPlayer = null;

            foreach (HockeyPlayer player in _visitorRoster)
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
            return AddAGoalie(_visitorRoster, strongerTeam);
        }
    }
}