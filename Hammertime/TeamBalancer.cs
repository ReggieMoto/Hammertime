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

namespace Hammertime
{
    public class TeamBalancerException : System.Exception
    {
        string _message;

        // ==============================================================
        public TeamBalancerException(string message) : base(message)
        // ==============================================================
        {
            _message = message;
        }
    }

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
        private bool AddLowerSkillPlayer(HockeyTeam team, int scoreDifferential)
        // ==============================================================
        {
            bool addedPlayerToRoster = false;
            HockeyTeam teamInstance = null;

            if (team.Location == HockeyTeam.Residence.Home)
                teamInstance = HomeTeam.Instance;
            else
                teamInstance = VisitorTeam.Instance;

            //Console.WriteLine($"AddLowerSkillPlayer: scoreDifferential = {scoreDifferential}");
            //Console.WriteLine();

            if (scoreDifferential <= 1)
            {
                //Console.WriteLine("AddLowerSkillLevelPlayer: scoreDifferential <= 1");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
            }
            else if (scoreDifferential == 2)
            {
                //Console.WriteLine("AddLowerSkillLevelPlayer: scoreDifferential == 2");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
            }
            else if (scoreDifferential == 3)
            {
                //Console.WriteLine("AddLowerSkillLevelPlayer: scoreDifferential == 3");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
            }
            else // (scoreDifferential == 4)
            {
                //Console.WriteLine("AddLowerSkillLevelPlayer: scoreDifferential == 4");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
            }

            //Console.WriteLine($"addedPlayerToRoster: {addedPlayerToRoster}");
            //Console.WriteLine();
            return addedPlayerToRoster;
        }

        // ==============================================================
        private bool AddHigherSkillPlayer(HockeyTeam team, int[] teamCompositionDiff, int scoreDifferential)
        // ==============================================================
        {
            bool addedPlayerToRoster = false;
            HockeyTeam teamInstance = null;

            if (team.Location == HockeyTeam.Residence.Home)
                teamInstance = HomeTeam.Instance;
            else
                teamInstance = VisitorTeam.Instance;

            //Console.WriteLine($"AddHigherSkillPlayer: scoreDifferential = {scoreDifferential}");
            //Console.WriteLine($"AddHigherSkillPlayer: {teamCompositionDiff[0]}.{teamCompositionDiff[1]}.{teamCompositionDiff[2]}.{teamCompositionDiff[3]}");
            //Console.WriteLine();

            // Check for Level A
            if (teamCompositionDiff[0] != 0) // Looking for an 'A' level player
            {
                //Console.WriteLine($"AddHighSkillLevelPlayer: teamCompositionDiff[A] == {teamCompositionDiff[0]}");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
            }

            // Check for Level B
            else if (teamCompositionDiff[1] != 0) // Looking for a 'B' level or better player
            {
                //Console.WriteLine($"AddHighSkillLevelPlayer: teamCompositionDiff[B] == {teamCompositionDiff[1]}");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
            }

            // Check for Level C
            else if (teamCompositionDiff[2] != 0) // Looking for a 'C' level or better player
            {
                //Console.WriteLine($"AddHighSkillLevelPlayer: teamCompositionDiff[C] == {teamCompositionDiff[2]}");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
            }

            // Check for Level D
            else // (teamCompositionDiff[3] != 0)
            {
                //Console.WriteLine($"AddHighSkillLevelPlayer: teamCompositionDiff[D] == {teamCompositionDiff[3]}");
                addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = teamInstance.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
            }

            //Console.WriteLine($"addedPlayerToRoster: {addedPlayerToRoster}");
            //Console.WriteLine();
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
        // Add goalies. Strong goalies to weaker teams.
        // ==============================================================
        private void AddGoalies(HomeTeam home, VisitorTeam visitor)
        // ==============================================================
        {
            if (home.TeamScore >= visitor.TeamScore)
            {
                home.AddAGoalie(true);
                visitor.AddAGoalie(false);
            }
            else
            {
                home.AddAGoalie(false);
                visitor.AddAGoalie(true);
            }
        }

        // ==============================================================
        private void BalanceDifferential2(HockeyTeam strong, HockeyTeam weak)
        // ==============================================================
        {
            HockeyPlayer strongPlayer = null;
            HockeyPlayer weakPlayer = null;

            if (strong.PlayerCount == weak.PlayerCount) // The player counts are equal so swap one for one
            {
                // These swaps are for unaffiliated players only
                // Try to swap an A with a B
                Console.WriteLine("BalanceDifferential2: Try to swap an A with a B");
                strongPlayer = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                weakPlayer = weak.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);

                if (strongPlayer == null || weakPlayer == null)
                {
                    // If we can't do that try to swap a B with a C
                    Console.WriteLine("BalanceDifferential2: Try to swap a B with a C");
                    strongPlayer = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                    weakPlayer = weak.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                }

                if (strongPlayer == null || weakPlayer == null)
                {
                    // If we can't do that try to swap a B with a C
                    Console.WriteLine("BalanceDifferential2: Try to swap a C with a D");
                    strongPlayer = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                    weakPlayer = weak.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                }

                // Perform the swap if the players are available
                if (strongPlayer != null && weakPlayer != null)
                {
                    Console.WriteLine("BalanceDifferential2: Perform the swap");
                    strong.AddAPlayer(weakPlayer);
                    weak.AddAPlayer(strongPlayer);

                    strong.RemoveAPlayer(strongPlayer);
                    weak.RemoveAPlayer(weakPlayer);
                }
            }
            else if (strong.PlayerCount > weak.PlayerCount)
            {
                // Try to move a D player from strong to weak
                // If that can't happen, try to move a C player from strong to weak
                HockeyPlayer player = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);

                // Perform the swap if the players are available
                if (player == null)
                    player = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);

                // Perform the swap if a player is available
                if (player != null)
                {
                    weak.AddAPlayer(player);
                    strong.RemoveAPlayer(player);
                }
            }
        }

        // ==============================================================
        private void BalanceDifferential3(HockeyTeam strong, HockeyTeam weak)
        // ==============================================================
        {
            HockeyPlayer strongPlayer = null;
            HockeyPlayer weakPlayer = null;

            if (strong.PlayerCount == weak.PlayerCount) // The player counts are equal so swap one for one
            {
                // These swaps are for unaffiliated players only
                // Swap an A (4) with a C (2)
                Console.WriteLine("BalanceDifferential3: Try to swap an A with a C");
                strongPlayer = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                weakPlayer = weak.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);

                if (strongPlayer == null || weakPlayer == null)
                {
                    // If we can't do that try to swap a B (3) with a D (1)
                    Console.WriteLine("BalanceDifferential3: Try to swap a B with a D");
                    strongPlayer = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                    weakPlayer = weak.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                }

                // Perform the swap if the players are available
                if (strongPlayer != null && weakPlayer != null)
                {
                    Console.WriteLine("BalanceDifferential3: Perform the swap");
                    strong.AddAPlayer(weakPlayer);
                    weak.AddAPlayer(strongPlayer);

                    strong.RemoveAPlayer(strongPlayer);
                    weak.RemoveAPlayer(weakPlayer);
                }
            }
            else if (strong.PlayerCount > weak.PlayerCount)
            {
                // Try to move a D player from strong to weak
                // If that can't happen, try to move a C player from strong to weak
                HockeyPlayer player = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);

                // Perform the swap if the players are available
                if (player == null)
                    player = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);

                // Perform the swap if a player is available
                if (player != null)
                {
                    weak.AddAPlayer(player);
                    strong.RemoveAPlayer(player);
                }
            }
        }

        // ==============================================================
        // This is the second pass final balancer.
        // It attempts to balance the teams based upon score and
        // numbers of players on each roster.
        // NOTE: This is pre-goalie. No goalies are added here.
        // ==============================================================
        public void FinalBalance(HomeTeam home, VisitorTeam visitor)
        // ==============================================================
        {
            int teamScoreDifferential = 0;

            if (home.TeamScore > visitor.TeamScore)
            {
                teamScoreDifferential = home.TeamScore - visitor.TeamScore;

                if (teamScoreDifferential == 3)
                {
                    BalanceDifferential3(home, visitor);
                }
                else if (teamScoreDifferential == 2)
                {
                    BalanceDifferential2(home, visitor);
                }
            }
            else if (visitor.TeamScore > home.TeamScore)
            {

                teamScoreDifferential = visitor.TeamScore - home.TeamScore;

                if (teamScoreDifferential == 3)
                {
                    BalanceDifferential3(visitor, home);
                }
                else if (teamScoreDifferential == 2)
                {
                    BalanceDifferential2(visitor, home);
                }
            }
        }

        // ==============================================================
        // This is the first pass coarse balancer.
        // It uses the difference in specific players' skill levels
        // to work out team rosters, attempting to balance based on
        // similar numbers of skilled players per team.
        // NOTE: Goalies are added after the final balance.
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

                //Console.WriteLine($"Home team player count: {home.PlayerCount}");
                //Console.WriteLine($"Visitor team player count: {visitor.PlayerCount}");

                // Find which team has fewer players and begin adding to that team.
                if (home.PlayerCount < visitor.PlayerCount)
                {
                    // The visiting team has more players.
                    // Add a player to the home team
                    if (visitor.TeamScore >= home.TeamScore)
                    {
                        // Added highest player available
                        addedToHome = AddHigherSkillPlayer(home, SkillDifference(visitor.TeamComposition, home.TeamComposition), visitor.TeamScore - home.TeamScore);
                    }
                    else // (home.TeamScore > visitor.TeamScore)
                    {
                        // Add lowest player available
                        addedToHome = AddLowerSkillPlayer(home, home.TeamScore - visitor.TeamScore);
                    }
                }
                else if (home.PlayerCount > visitor.PlayerCount)
                {
                    // The home team has more players.
                    // Add a player to the visitor team
                    if (home.TeamScore >= visitor.TeamScore)
                    {
                        // Added highest player available
                        addedToVisitor = AddHigherSkillPlayer(visitor, SkillDifference(home.TeamComposition, visitor.TeamComposition), home.TeamScore - visitor.TeamScore);
                    }
                    else // (visitor.TeamScore > home.TeamScore)
                    {
                        // Add lowest player available
                        addedToVisitor = AddLowerSkillPlayer(visitor, visitor.TeamScore - home.TeamScore);
                    }
                }
                else // (home.PlayerCount == visitor.PlayerCount)
                {
                    // Add a player to the home team
                    if (visitor.TeamScore > home.TeamScore)
                    {
                        // Added highest player available
                        addedToHome = AddHigherSkillPlayer(home, SkillDifference(visitor.TeamComposition, home.TeamComposition), visitor.TeamScore - home.TeamScore);
                    }
                    else if (home.TeamScore > visitor.TeamScore)
                    {
                        // Added highest player available
                        addedToHome = AddHigherSkillPlayer(visitor, SkillDifference(home.TeamComposition, visitor.TeamComposition), home.TeamScore - visitor.TeamScore);
                    }
                    else // (home.TeamScore == visitor.TeamScore)
                    {
                        // Add lowest players available
                        //Console.Write("Home team: ");
                        addedToHome = AddLowerSkillPlayer(home, 0);
                        //Console.Write("Visitor team: ");
                        addedToVisitor = AddLowerSkillPlayer(visitor, 0);
                    }

                }
                //Console.WriteLine();
                if (addedToHome == false && addedToVisitor == false)
                    moreAvailablePlayers = false;
            }

            //Console.WriteLine($"Control counter: {controlCounter}.");
            if (controlCounter > 20) throw (new TeamBalancerException("Error: Runaway Balance method."));

            // Now that a coarse balance has been done, do a final balance to attempt to even out the teams
            // if they aren't very even (scores are still off and/or player counts are off)
            Console.WriteLine($"Balance: Finished coarse balance.");
            Console.WriteLine($"Balance: home.PlayerCount    = {home.PlayerCount}");
            Console.WriteLine($"Balance: visitor.PlayerCount = {visitor.PlayerCount}");
            Console.WriteLine($"Balance: home.TeamScore    = {home.TeamScore}");
            Console.WriteLine($"Balance: visitor.TeamScore = {visitor.TeamScore}");

            if ((home.PlayerCount != visitor.PlayerCount) ||
                (home.TeamScore != visitor.TeamScore))
                FinalBalance(home, visitor);
            
            // Final balancing has been done. Now add goalies.
            AddGoalies(home, visitor);
        }
    }
}