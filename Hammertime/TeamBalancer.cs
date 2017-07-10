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

            //Console.WriteLine($"AddLowerSkillPlayer: scoreDifferential = {scoreDifferential}");
            //Console.WriteLine();

            if (scoreDifferential <= 1)
            {
                //Console.WriteLine("AddLowerSkillLevelPlayer: scoreDifferential <= 1");
                addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
            }
            else if (scoreDifferential == 2)
            {
                //Console.WriteLine("AddLowerSkillLevelPlayer: scoreDifferential == 2");
                addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
            }
            else if (scoreDifferential == 3)
            {
                //Console.WriteLine("AddLowerSkillLevelPlayer: scoreDifferential == 3");
                addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
            }
            else // (scoreDifferential == 4)
            {
                //Console.WriteLine("AddLowerSkillLevelPlayer: scoreDifferential == 4");
                addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
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
            //HockeyTeam teamInstance = null;

            //if (team.Location == HockeyTeam.Residence.Home)
                //teamInstance = HomeTeam.Instance;
            //else
                //teamInstance = VisitorTeam.Instance;

            //Console.WriteLine($"AddHigherSkillPlayer: scoreDifferential = {scoreDifferential}");
            //Console.WriteLine($"AddHigherSkillPlayer: {teamCompositionDiff[0]}.{teamCompositionDiff[1]}.{teamCompositionDiff[2]}.{teamCompositionDiff[3]}");
            //Console.WriteLine();

            // Check for Level A
            if (teamCompositionDiff[0] != 0) // Looking for an 'A' level player
            {
                //Console.WriteLine($"AddHighSkillLevelPlayer: teamCompositionDiff[A] == {teamCompositionDiff[0]}");
                addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
            }

            // Check for Level B
            else if (teamCompositionDiff[1] != 0) // Looking for a 'B' level or better player
            {
                //Console.WriteLine($"AddHighSkillLevelPlayer: teamCompositionDiff[B] == {teamCompositionDiff[1]}");
                addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
            }

            // Check for Level C
            else if (teamCompositionDiff[2] != 0) // Looking for a 'C' level or better player
            {
                //Console.WriteLine($"AddHighSkillLevelPlayer: teamCompositionDiff[C] == {teamCompositionDiff[2]}");
                addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
            }

            // Check for Level D
            else // (teamCompositionDiff[3] != 0)
            {
                //Console.WriteLine($"AddHighSkillLevelPlayer: teamCompositionDiff[D] == {teamCompositionDiff[3]}");
                addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                if (addedPlayerToRoster == false)
                    addedPlayerToRoster = team.AddASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
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
        private void Balance(HockeyTeam strongTeam, HockeyTeam weakTeam)
        // ==============================================================
        {
            HockeyPlayer strongPlayer = null;
            HockeyPlayer weakPlayer = null;

            int teamScoreDifferential = strongTeam.TeamScore - weakTeam.TeamScore;

            if (strongTeam.PlayerCount == weakTeam.PlayerCount) // The player counts are equal so swap one for one
            {
                switch (teamScoreDifferential / 2)
                {
                    case 5: // No swap; move an A
                        Console.WriteLine("BalanceDifferential11/10: Try to move an A player from strong to weak");
                        HockeyPlayer player = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);

                        // Perform the swap if a player is available
                        if (player != null)
                        {
                            Console.WriteLine("BalanceDifferential11/10: Perform the move");
                            weakTeam.AddAPlayer(player);
                            strongTeam.RemoveAPlayer(player);
                        }
                        break;

                    case 4: // No swap; move an A
                        Console.WriteLine("BalanceDifferential9/8: Try to move an A player from strong to weak");
                        player = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);

                        // Perform the swap if a player is available
                        if (player != null)
                        {
                            Console.WriteLine("BalanceDifferential9/8: Perform the move");
                            weakTeam.AddAPlayer(player);
                            strongTeam.RemoveAPlayer(player);
                        }
                        break;

                    case 3: // Swap an A for a D
                        Console.WriteLine("BalanceDifferential7/6: Try to swap an A with a D");
                        strongPlayer = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                        weakPlayer = weakTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                        break;

                    case 2: // Swap an A for a C or a B for a D
                        Console.WriteLine("BalanceDifferential5/4: Try to swap an A with a C");
                        strongPlayer = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                        weakPlayer = weakTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);

                        if (strongPlayer == null || weakPlayer == null)
                        {
                            // If we can't do that try to swap a B (3) with a D (1)
                            Console.WriteLine("BalanceDifferential5/4: Try to swap a B with a D");
                            strongPlayer = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                            weakPlayer = weakTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                        }
                        break;

                    case 1:  // Swap an A for a B, a B for a C,  or a C for a D
                                // Try to swap an A with a B
                        Console.WriteLine("BalanceDifferential3/2: Try to swap an A with a B");
                        strongPlayer = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                        weakPlayer = weakTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);

                        if (strongPlayer == null || weakPlayer == null)
                        {
                            // If we can't do that try to swap a B with a C
                            Console.WriteLine("BalanceDifferential3/2: Try to swap a B with a C");
                            strongPlayer = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                            weakPlayer = weakTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                        }

                        if (strongPlayer == null || weakPlayer == null)
                        {
                            // If we can't do that try to swap a C with a D
                            Console.WriteLine("BalanceDifferential3/2: Try to swap a C with a D");
                            strongPlayer = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                            weakPlayer = weakTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                        }
                        break;
                }

                // Perform the swap if the players are available
                if (strongPlayer != null && weakPlayer != null)
                {
                    Console.WriteLine("BalanceTeams: Perform the swap");
                    strongTeam.AddAPlayer(weakPlayer);
                    weakTeam.AddAPlayer(strongPlayer);

                    strongTeam.RemoveAPlayer(strongPlayer);
                    weakTeam.RemoveAPlayer(weakPlayer);
                }
            }
            else // (strongTeam.PlayerCount > weakTeam.PlayerCount)  The strong team player count is greater so move a player
            {
                HockeyPlayer player = null;

                switch (teamScoreDifferential / 2)
                {
                    case 5:
                        Console.WriteLine("BalanceDifferential11/10: Try to move an A player from strong to weak");
                        player = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                        if (player == null)
                        {
                            Console.WriteLine("BalanceDifferential11/10: Try to move a B player from strong to weak");
                            player = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                        }
                        break;

                    case 4:
                        Console.WriteLine("BalanceDifferential9/8: Try to move an A player from strong to weak");
                        player = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_A);
                        if (player == null)
                        {
                            Console.WriteLine("BalanceDifferential9/8: Try to move a B player from strong to weak");
                            player = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                        }
                        break;

                    case 3:
                        Console.WriteLine("BalanceDifferential7/6: Try to move a B player from strong to weak");
                        player = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_B);
                        if (player == null)
                        {
                            Console.WriteLine("BalanceDifferential7/6: Try to move a C player from strong to weak");
                            player = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                        }
                        break;

                    case 2:
                        Console.WriteLine("BalanceDifferential5/4: Try to move a C player from strong to weak");
                        player = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_C);
                        if (player == null)
                        {
                            Console.WriteLine("BalanceDifferential5/4: Try to move a D player from strong to weak");
                            player = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                        }
                        break;

                    case 1:
                        Console.WriteLine("BalanceDifferential3/2: Try to move a D player from strong to weak");
                        player = strongTeam.GetASkillPlayer(HockeyPlayer.PlayerSkill.Level_D);
                        break;
                }

                // Perform the swap if a player is available
                if (player != null)
                {
                    Console.WriteLine("BalanceTeams: Perform the move");
                    weakTeam.AddAPlayer(player);
                    strongTeam.RemoveAPlayer(player);
                }
            }
        }

        // ==============================================================
        // This is the team balancer.
        // It will balance the teams based upon score and
        // numbers of players on each roster.
        // NOTE: Goalies are added after the balance.
        // ==============================================================
        public void Balance()
        // ==============================================================
        {
            HomeTeam home = HomeTeam.Instance;
            VisitorTeam visitor = VisitorTeam.Instance;

            Console.WriteLine($"Balancer: home.PlayerCount    = {home.PlayerCount}");
            Console.WriteLine($"Balancer: visitor.PlayerCount = {visitor.PlayerCount}");
            Console.WriteLine($"Balancer: home.TeamScore    = {home.TeamScore}");
            Console.WriteLine($"Balancer: visitor.TeamScore = {visitor.TeamScore}");

            int teamScoreDiff = 0;
            int runaway = 0;

            do
            {
                if (home.TeamScore >= visitor.TeamScore)
                    Balance(home, visitor);
                else
                    Balance(visitor, home);

                if (home.TeamScore >= visitor.TeamScore)
                    teamScoreDiff = home.TeamScore - visitor.TeamScore;
                else
                    teamScoreDiff = visitor.TeamScore - home.TeamScore;

            } while (teamScoreDiff > 1 && runaway++ < 5);

            if (runaway >= 5)
                Console.WriteLine("Runaway halted.");

            Console.WriteLine("Select goalies.");
            if (home.TeamScore >= visitor.TeamScore)
            {
                home.AddAGoalie(true); // Strong team
                visitor.AddAGoalie(false);
            }
            else
            {
                visitor.AddAGoalie(true); // Strong team
                home.AddAGoalie(false);
            }
        }
    }
}