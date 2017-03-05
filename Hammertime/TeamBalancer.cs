// ==============================================================
//
// class TeamBalancer
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
    public class TeamBalancer
    {
        private static TeamBalancer _teamBalancer;

        private TeamBalancer() {; }

        // ==============================================================
        public static TeamBalancer Instance
        // ==============================================================
        {
            get
            {
                if (_teamBalancer == null)
                    _teamBalancer = new TeamBalancer();
                return _teamBalancer;
            }
        }

        // ==============================================================
        private bool AddLowerSkillPlayer(HockeyTeam team)
        // ==============================================================
        {
            bool addedPlayerToRoster = false;
            HockeyTeam teamInstance = null;

            if (team.Location == HockeyTeam.Residence.Home)
                teamInstance = HomeTeam.Instance;
            else
                teamInstance = VisitorTeam.Instance;

            Console.WriteLine("AddLowerSkillLevelPlayer: Check for Level D");
            addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
            if (addedPlayerToRoster == false)
            {
                Console.WriteLine("AddLowerSkillLevelPlayer: Check for Level C");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
            }
            if (addedPlayerToRoster == false)
            {
                Console.WriteLine("AddLowerSkillLevelPlayer: Check for Level B");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
            }
            if (addedPlayerToRoster == false)
            {
                Console.WriteLine("AddLowerSkillLevelPlayer: Check for Level A");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
            }

            return addedPlayerToRoster;
        }

        // ==============================================================
        private bool AddHigherSkillPlayer(HockeyTeam team, int[] teamCompositionDiff)
        // ==============================================================
        {
            bool addedPlayerToRoster = false;
            HockeyTeam teamInstance = null;

            if (team.Location == HockeyTeam.Residence.Home)
                teamInstance = HomeTeam.Instance;
            else
                teamInstance = VisitorTeam.Instance;

            // Check for Level A
            if (teamCompositionDiff[0] != 0)
            {
                Console.WriteLine("AddHighSkillLevelPlayer: Check for Level A");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
            }

            // Check for Level B
            if (addedPlayerToRoster == false && (teamCompositionDiff[1] != 0))
            {
                Console.WriteLine("AddHighSkillLevelPlayer: Check for Level B");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
            }

            // Check for Level C
            if (addedPlayerToRoster == false && (teamCompositionDiff[2] != 0))
            {
                Console.WriteLine("AddHighSkillLevelPlayer: Check for Level C");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
            }

            // Check for Level D
            if (addedPlayerToRoster == false && (teamCompositionDiff[3] != 0))
            {
                Console.WriteLine("AddHighSkillLevelPlayer: Check for Level D");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
            }

            return addedPlayerToRoster;
        }

        // ==============================================================
        private int[] SkillDifference(int[] strong, int[] weak)
        // ==============================================================
        {
            int[] difference = new int[] { 0, 0, 0, 0 };

            if (strong[0] > weak[0])
                difference[0] = strong[0] - weak[0];

            if (strong[1] > weak[1])
                difference[1] = strong[1] - weak[1];

            if (strong[2] > weak[2])
                difference[2] = strong[2] - weak[2];

            if (strong[3] > weak[3])
                difference[3] = strong[3] - weak[3];

            return difference;
        }

        // ==============================================================
        public void Balance(HomeTeam home, VisitorTeam visitor)
        // ==============================================================
        {
            bool moreAvailablePlayers = true;
            int controlCounter = 0;

            // As long as either team has less than 10 players and there are more available players
            // Keep adding players. The team compositions and scores are factored in below.
            while ((home.PlayerCount < 10 ||
                visitor.PlayerCount < 10) &&
                moreAvailablePlayers == true &&
                controlCounter++ <= 20) // Emergency runaway shutoff
            {
                // Booleans to keep track of whether players have been added or not
                bool addedToHome = false, addedToVisitor = false;

                if (home.PlayerCount <= visitor.PlayerCount)
                {
                    // Add a player to the home team
                    if (HockeyTeam.StrongerTeam(home, visitor) == visitor)
                    {
                        // Added highest player available
                        addedToHome = AddHigherSkillPlayer(home, SkillDifference(visitor.TeamComposition, home.TeamComposition));
                    }
                    else // (HockeyTeam.StrongerTeam(home, visitor) == visitor)
                    {
                        // Add lowest player available
                        addedToHome = AddLowerSkillPlayer(home);
                    }
                }
                else // (home.PlayerCount > visitor.PlayerCount)
                {
                    // Add a player to the visitor team
                    if (HockeyTeam.StrongerTeam(home, visitor) == home)
                    {
                        // Added highest player available
                        addedToVisitor = AddHigherSkillPlayer(visitor, SkillDifference(home.TeamComposition, visitor.TeamComposition));
                    }
                    else // (visitor.TeamComposition > home.TeamComposition)
                    {
                        // Add lowest player available
                        addedToVisitor = AddLowerSkillPlayer(visitor);
                    }
                }
                Console.WriteLine();
                if (addedToHome == false && addedToVisitor == false)
                    moreAvailablePlayers = false;
            }

            Console.WriteLine($"moreAvailablePlayers: {moreAvailablePlayers}.");
            Console.WriteLine($"Control counter: {controlCounter}.");
        }

        /*
        // ==============================================================
        public void Balance(HomeTeam home, VisitorTeam visitor)
        // ==============================================================
        {
            bool moreAvailablePlayers = true;
            int controlCounter = 0;

            // As long as either team has less than 10 players and there are more available players
            // Keep adding players. The team compositions and scores are factored in below.
            while ((home.PlayerCount < 10 ||
                visitor.PlayerCount < 10) &&
                moreAvailablePlayers == true &&
                controlCounter++ <= 20) // Emergency runaway shutoff
            {
                // Get how many A's, B's, C's and D's there are per team
                int homeTeamComposition = home.TeamComposition;
                int visitorTeamComposition = visitor.TeamComposition;
                // Get a count of how many players per team there are
                int homeTeamPlayerCount = home.PlayerCount;
                int visitorTeamPlayerCount = visitor.PlayerCount;
                // Booleans to keep track of whether players have been added or not
                bool addedToHome = false, addedToVisitor = false;

                int teamCompositionDiff = 0;
                if (homeTeamComposition <= visitorTeamComposition)
                    teamCompositionDiff = visitorTeamComposition - homeTeamComposition;

                // Gotta start somewhere so start with the home team
                if ((homeTeamPlayerCount < 10) &&
                    (homeTeamPlayerCount <= visitorTeamPlayerCount) &&
                    (homeTeamComposition <= visitorTeamComposition))
                {
                    // Check for Level A
                    if (teamCompositionDiff/1000 != 0)
                    {
                        Console.WriteLine("Home team: Check for Level A");
                        addedToHome = home.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                        if (addedToHome == false)
                            addedToHome = home.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                        if (addedToHome == false)
                            addedToHome = home.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                        if (addedToHome == false)
                            addedToHome = home.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                    }

                    // Check for Level B
                    if ((teamCompositionDiff / 1000 != 0) && addedToHome == false)
                    {
                        Console.WriteLine("Home team: Check for Level B");
                        teamCompositionDiff -= (teamCompositionDiff / 1000) * 1000;

                        if (teamCompositionDiff/100 != 0)
                            addedToHome = home.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                        if (addedToHome == false)
                            addedToHome = home.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                        if (addedToHome == false)
                            addedToHome = home.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                    }

                    // Check for Level C
                    if ((teamCompositionDiff / 100 != 0) && addedToHome == false)
                    {
                        Console.WriteLine("Home team: Check for Level C");
                        teamCompositionDiff -= (teamCompositionDiff / 100) * 100;

                        if (teamCompositionDiff / 10 != 0)
                            addedToHome = home.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                        if (addedToHome == false)
                            addedToHome = home.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                    }

                    // Check for Level D
                    if ((teamCompositionDiff / 10 != 0) && addedToHome == false)
                    {
                        Console.WriteLine("Home team: Check for Level D");
                        teamCompositionDiff -= (teamCompositionDiff / 10) * 10;

                        if (teamCompositionDiff / 1 != 0)
                            addedToHome = home.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                    }
                }

                // Now check on the visiting team
                if ((visitorTeamPlayerCount < 10) &&
                    (visitorTeamPlayerCount <= homeTeamPlayerCount) &&
                    (visitorTeamComposition <= homeTeamComposition))
                {
                    int teamCompositionDiff = homeTeamComposition - visitorTeamComposition;

                    // Check for Level A
                    if (teamCompositionDiff / 1000 != 0)
                    {
                        Console.WriteLine("Visitor team: Check for Level A");
                        addedToVisitor = visitor.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                        if (addedToVisitor == false)
                            addedToVisitor = visitor.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                        if (addedToVisitor == false)
                            addedToVisitor = visitor.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                        if (addedToVisitor == false)
                            addedToVisitor = visitor.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                    }

                    // Check for Level B
                    if ((teamCompositionDiff / 1000 != 0) && addedToVisitor == false)
                    {
                        Console.WriteLine("Visitor team: Check for Level B");
                        teamCompositionDiff -= (teamCompositionDiff / 1000) * 1000;

                        if (teamCompositionDiff / 100 != 0)
                            addedToVisitor = visitor.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                        if (addedToVisitor == false)
                            addedToVisitor = visitor.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                        if (addedToVisitor == false)
                            addedToVisitor = visitor.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                    }

                    // Check for Level C
                    if ((teamCompositionDiff / 100 != 0) && addedToVisitor == false)
                    {
                        Console.WriteLine("Visitor team: Check for Level C");
                        teamCompositionDiff -= (teamCompositionDiff / 100) * 100;

                        if (teamCompositionDiff / 10 != 0)
                            addedToVisitor = visitor.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                        if (addedToVisitor == false)
                            addedToVisitor = visitor.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                    }

                    // Check for Level D
                    if ((teamCompositionDiff / 10 != 0) && addedToVisitor == false)
                    {
                        Console.WriteLine("Visitor team: Check for Level D");
                        teamCompositionDiff -= (teamCompositionDiff / 10) * 10;

                        if (teamCompositionDiff / 1 != 0)
                            addedToVisitor = visitor.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                    }
                    if (addedToHome == false && addedToVisitor == false)
                        moreAvailablePlayers = false;
                }
            }

            Console.WriteLine($"Control counter: {controlCounter}.");
        }
        */
    }
}