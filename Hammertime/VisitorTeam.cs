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

using System;
using System.Collections;

namespace Hammertime
{
    public sealed class VisitorTeam : HockeyTeam
    {
        public static VisitorTeam Instance
        {
            get
            {
                if (_visitorTeam == null)
                    _visitorTeam = new VisitorTeam();
                return _visitorTeam;
            }
        }

        private static VisitorTeam _visitorTeam;

        private VisitorTeam()
            : base(Residence.Away)
        {
            // Base class: Attach to the DB server
            // Base class: Build a roster from the server
            BuildTeamRoster();
        }

        public void PrintVisitingTeamRoster()
        {
            PrintTeamRoster(_visitorRoster);
        }

        public new int TeamScore
        {
            get { return TeamScore(_visitorRoster); }
        }

        public new int[] TeamComposition
        {
            get { return TeamComposition(_visitorRoster); }
        }

        public override int PlayerCount
        {
            get { return _visitorRoster.Count; }
        }

        public override bool AddASkillPlayer(HockeyPlayer.PlayerSkill skillLevel)
        {
            Console.WriteLine("AddASkillPlayer for the visitor team");
            return AddASkillPlayer(_visitorRoster, skillLevel);
        }

        // ==============================================================
        //  Rules of the road for generating a team roster:
        //  1. Which players are available? From the survey.
        //  2. No more than 10(S)+1(G) players per team.
        //  3. Specific full-time players stay with specific teams (Ben and Barry).
        //  4. Unaffiliated players can be assigned to any team.
        //  5. Remaining full-time assignments next; then subs.
        //  6. Each team has one goalie.
        //  7. Team skill scores must be very close.
        // ==============================================================
        protected override void BuildTeamRoster()
        // ==============================================================
        {
            //Console.WriteLine("Building the visiting team roster.");

            _visitorRoster = new ArrayList();

            // The visiting team is the "dark" team
            // Look to see which which players were on the home "white" team last week and make them the visiting "dark" team this week
            foreach (HockeyPlayer player in _availableFullTimePlayers)
            {
                // First get available full-time players associated with either Ben or Barry
                if ((player.AssignedToTeam == false) &&
                    (player.PlayerTeam != "Unaffiliated") &&    // Not unaffiliated means affiliated with either Ben or Barry
                    (player.PlayerLastWeek == "White"))         // Last week player was on the home "white" team

                {
                    player.AssignedToTeam = true;
                    player.PlayerLastWeek = "Black";    // This week the player will be on the visiting "black" team
                    _visitorRoster.Add(player);
                }
            }

        }

        private ArrayList _visitorRoster;
    }
}