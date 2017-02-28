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
            Console.WriteLine("Initializing the visiting team.");
            BuildTeamRoster();
        }

        public void PrintVisitingTeamRoster()
        {
            PrintTeamRoster(_visitorRoster);
        }

        protected override void BuildTeamRoster()
        {
            Console.WriteLine("Building the visiting team roster.");

            _visitorRoster = new ArrayList();

            // The visiting team is the "dark" team
            // Look to see which which players were on the home "white" team last week and make them the visiting "dark" team this week
            foreach (HockeyPlayer player in _masterRoster)
            {
                if ((player.PlayerType == 'F') &&       // Full-time player
                    (player.PlayerLastWeek == "White")) // Last week player was on the home "white" team
                {
                    _visitorRoster.Add(player); // This week the player will be on the visiting "dark" team
                }
            }

        }

        private ArrayList _visitorRoster;
    }
}