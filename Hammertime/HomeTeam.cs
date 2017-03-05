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

using System;
using System.Collections;

namespace Hammertime
{
    public sealed class HomeTeam : HockeyTeam
    {
        public static HomeTeam Instance
        {
            get
            {
                if (_homeTeam == null)
                    _homeTeam = new HomeTeam();
                return _homeTeam;
            }
        }

        private static HomeTeam _homeTeam;

        private HomeTeam()
            : base(Residence.Home)
        {
            // Base class: Attach to the DB server
            // Base class: Build a roster from the server
            BuildTeamRoster();
        }

        public void PrintHomeTeamRoster()
        {
            PrintTeamRoster(_homeRoster);
        }

        public new int TeamScore
        {
            get { return TeamScore(_homeRoster); }
        }

        public new int[] TeamComposition
        {
            get { return TeamComposition(_homeRoster); }
        }

        public override int PlayerCount
        {
            get { return _homeRoster.Count; }
        }

        public override bool AddASkillPlayer(HockeyPlayer.PlayerSkill skillLevel)
        {
            //Console.WriteLine("AddASkillPlayer for the home team");
            return AddASkillPlayer(_homeRoster, skillLevel);
        }

        // ==============================================================
        //  Rules of the road for generating a team roster:
        //  1. Which players are available? From the survey and base class contains lists.
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
            //Console.WriteLine("Building the home team roster.");

            _homeRoster = new ArrayList();

            // The home team is the "white" team
            // Look to see which which full-time players were on the visiting "black" team last week and make them the home "white" team this week

            foreach (HockeyPlayer player in _availableFullTimePlayers)
            {
                // First get available full-time players associated with either Ben or Barry
                if ((player.AssignedToTeam == false) &&
                    (player.PlayerTeam != "Unaffiliated") &&    // Not unaffiliated means affiliated with either Ben or Barry
                    (player.PlayerLastWeek == "Black"))         // Last week player was on the visiting "black" team

                {
                    player.AssignedToTeam = true;
                    player.PlayerLastWeek = "White";    // This week the player will be on the home "white" team
                    _homeRoster.Add(player);
                }
            }
        }

        private ArrayList _homeRoster;
    }
}