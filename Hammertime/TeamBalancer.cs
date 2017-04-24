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
        private void AddGoalies()
        // ==============================================================
        {
            HomeTeam home = HomeTeam.Instance;
            VisitorTeam visitor = VisitorTeam.Instance;

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
                Console.WriteLine("BalanceDifferential2: Try to move a D player from strong to weak");
                HockeyPlayer player = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);

                if (player == null)
                {
                    // If that can't happen, try to move a C player from strong to weak
                    Console.WriteLine("BalanceDifferential2: Try to move a C player from strong to weak");
                    player = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                }

                // Perform the swap if a player is available
                if (player != null)
                {
                    Console.WriteLine("BalanceDifferential2: Perform the swap");
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
                Console.WriteLine("BalanceDifferential3: Try to move a D player from strong to weak");
                HockeyPlayer player = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);

                // Perform the swap if the players are available
                if (player == null)
                {
                    Console.WriteLine("BalanceDifferential3: Try to move a C player from strong to weak");
                    player = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                }

                // Perform the swap if a player is available
                if (player != null)
                {
                    Console.WriteLine("BalanceDifferential3: Perform the swap");
                    weak.AddAPlayer(player);
                    strong.RemoveAPlayer(player);
                }
            }
        }

        // ==============================================================
        private void BalanceDifferential4(HockeyTeam strong, HockeyTeam weak)
        // ==============================================================
        {
            HockeyPlayer strongPlayer = null;
            HockeyPlayer weakPlayer = null;

            if (strong.PlayerCount == weak.PlayerCount) // The player counts are equal so swap one for one
            {
                // These swaps are for unaffiliated players only
                // Swap an A (4) with a C (2)
                Console.WriteLine("BalanceDifferential4: Try to swap an A with a C");
                strongPlayer = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                weakPlayer = weak.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);

                if (strongPlayer == null || weakPlayer == null)
                {
                    // If we can't do that try to swap a B (3) with a D (1)
                    Console.WriteLine("BalanceDifferential4: Try to swap a B with a D");
                    strongPlayer = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                    weakPlayer = weak.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                }

                // Perform the swap if the players are available
                if (strongPlayer != null && weakPlayer != null)
                {
                    Console.WriteLine("BalanceDifferential4: Perform the swap");
                    strong.AddAPlayer(weakPlayer);
                    weak.AddAPlayer(strongPlayer);

                    strong.RemoveAPlayer(strongPlayer);
                    weak.RemoveAPlayer(weakPlayer);
                }
            }
            else if (strong.PlayerCount > weak.PlayerCount)
            {
                // Try to move a C player from strong to weak
                // If that can't happen, try to move a D player from strong to weak
                Console.WriteLine("BalanceDifferential4: Try to move a C player from strong to weak");
                HockeyPlayer player = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);

                // Perform the swap if the players are available
                if (player == null)
                {
                    Console.WriteLine("BalanceDifferential4: Try to move a D player from strong to weak");
                    player = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                }

                // Perform the swap if a player is available
                if (player != null)
                {
                    Console.WriteLine("BalanceDifferential4: Perform the swap");
                    weak.AddAPlayer(player);
                    strong.RemoveAPlayer(player);
                }
            }
        }

        // ==============================================================
        private void BalanceDifferential5(HockeyTeam strong, HockeyTeam weak)
        // ==============================================================
        {
            HockeyPlayer strongPlayer = null;
            HockeyPlayer weakPlayer = null;

            if (strong.PlayerCount == weak.PlayerCount) // The player counts are equal so swap one for one
            {
                // These swaps are for unaffiliated players only
                // Swap an A (4) with a C (2)
                Console.WriteLine("BalanceDifferential5: Try to swap an A with a C");
                strongPlayer = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                weakPlayer = weak.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);

                if (strongPlayer == null || weakPlayer == null)
                {
                    // If we can't do that try to swap a B (3) with a D (1)
                    Console.WriteLine("BalanceDifferential5: Try to swap a B with a D");
                    strongPlayer = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                    weakPlayer = weak.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                }

                // Perform the swap if the players are available
                if (strongPlayer != null && weakPlayer != null)
                {
                    Console.WriteLine("BalanceDifferential5: Perform the swap");
                    strong.AddAPlayer(weakPlayer);
                    weak.AddAPlayer(strongPlayer);

                    strong.RemoveAPlayer(strongPlayer);
                    weak.RemoveAPlayer(weakPlayer);
                }
            }
            else if (strong.PlayerCount > weak.PlayerCount)
            {
                // Try to move a B player from strong to weak
                // If that can't happen, try to move a C player from strong to weak
                Console.WriteLine("BalanceDifferential5: Try to move a B player from strong to weak");
                HockeyPlayer player = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);

                // Perform the swap if the players are available
                if (player == null)
                {
                    Console.WriteLine("BalanceDifferential5: Try to move a C player from strong to weak");
                    player = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                }

                // Perform the swap if a player is available
                if (player != null)
                {
                    Console.WriteLine("BalanceDifferential5: Perform the swap");
                    weak.AddAPlayer(player);
                    strong.RemoveAPlayer(player);
                }
            }
        }

        // ==============================================================
        private void BalanceDifferential6(HockeyTeam strong, HockeyTeam weak)
        // ==============================================================
        {
            HockeyPlayer strongPlayer = null;
            HockeyPlayer weakPlayer = null;

            if (strong.PlayerCount == weak.PlayerCount) // The player counts are equal so swap one for one
            {
                // These swaps are for unaffiliated players only
                // Swap an A (4) with a D (1)
                Console.WriteLine("BalanceDifferential6: Try to swap an A with a D");
                strongPlayer = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                weakPlayer = weak.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);

                if (strongPlayer == null || weakPlayer == null)
                {
                    // If we can't do that try to swap an A (4) with a C (2)
                    Console.WriteLine("BalanceDifferential6: Try to swap an A with a C");
                    strongPlayer = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                    weakPlayer = weak.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                }

                if (strongPlayer == null || weakPlayer == null)
                {
                    // If we can't do that try to swap a B (3) with a D (1)
                    Console.WriteLine("BalanceDifferential6: Try to swap a B with a D");
                    strongPlayer = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                    weakPlayer = weak.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                }

                // Perform the swap if the players are available
                if (strongPlayer != null && weakPlayer != null)
                {
                    Console.WriteLine("BalanceDifferential6: Perform the swap");
                    strong.AddAPlayer(weakPlayer);
                    weak.AddAPlayer(strongPlayer);

                    strong.RemoveAPlayer(strongPlayer);
                    weak.RemoveAPlayer(weakPlayer);
                }
            }
            else if (strong.PlayerCount > weak.PlayerCount)
            {
                // Try to move a B player from strong to weak
                // If that can't happen, try to move a C player from strong to weak
                Console.WriteLine("BalanceDifferential6: Try to move a B player from strong to weak");
                HockeyPlayer player = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);

                // Perform the swap if the players are available
                if (player == null)
                {
                    Console.WriteLine("BalanceDifferential6: Try to move a C player from strong to weak");
                    player = strong.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                }

                // Perform the swap if a player is available
                if (player != null)
                {
                    Console.WriteLine("BalanceDifferential3: Perform the swap");
                    weak.AddAPlayer(player);
                    strong.RemoveAPlayer(player);
                }
            }
        }

        // ==============================================================
        // This is the team balancer.
        // It attempts to balance the teams based upon score and
        // numbers of players on each roster.
        // NOTE: Goalies are added after the balance.
        // ==============================================================
        public void BalanceTeams()
        // ==============================================================
        {
            HomeTeam home = HomeTeam.Instance;
            VisitorTeam visitor = VisitorTeam.Instance;

            Console.WriteLine($"Balancer: home.PlayerCount    = {home.PlayerCount}");
            Console.WriteLine($"Balancer: visitor.PlayerCount = {visitor.PlayerCount}");
            Console.WriteLine($"Balancer: home.TeamScore    = {home.TeamScore}");
            Console.WriteLine($"Balancer: visitor.TeamScore = {visitor.TeamScore}");

            int teamScoreDifferential = 0;
            int runawayCatch = 0;

            do
            {
                if (home.TeamScore > visitor.TeamScore)
                {
                    teamScoreDifferential = home.TeamScore - visitor.TeamScore;

                    do
                    {
                        if (teamScoreDifferential >= 6)
                            BalanceDifferential6(home, visitor);
                        else if (teamScoreDifferential == 5)
                            BalanceDifferential5(home, visitor);
                        else if (teamScoreDifferential == 4)
                            BalanceDifferential4(home, visitor);
                        else if (teamScoreDifferential == 3)
                            BalanceDifferential3(home, visitor);
                        else if (teamScoreDifferential == 2)
                            BalanceDifferential2(home, visitor);

                        teamScoreDifferential = home.TeamScore - visitor.TeamScore;

                    } while (teamScoreDifferential > 1 && runawayCatch++ < 5);

                }
                else if (visitor.TeamScore > home.TeamScore)
                {
                    teamScoreDifferential = visitor.TeamScore - home.TeamScore;

                    do
                    {
                        if (teamScoreDifferential >= 6)
                            BalanceDifferential6(visitor, home);
                        else if (teamScoreDifferential == 5)
                            BalanceDifferential5(visitor, home);
                        else if (teamScoreDifferential == 4)
                            BalanceDifferential4(visitor, home);
                        else if (teamScoreDifferential == 3)
                            BalanceDifferential3(visitor, home);
                        else if (teamScoreDifferential == 2)
                            BalanceDifferential2(visitor, home);

                        teamScoreDifferential = visitor.TeamScore - home.TeamScore;
                    }
                    while (teamScoreDifferential > 1 && runawayCatch++ < 5);

                }
            } while (teamScoreDifferential > 1 && runawayCatch++ < 5);

            if (runawayCatch == 3)
                Console.WriteLine("TeamBalancer: Caught a runaway do/while loop.");

            // Final balancing has been done. Now add goalies.
            AddGoalies();
        }
    }
}