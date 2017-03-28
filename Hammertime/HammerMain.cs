// ==============================================================
//
// class HammerMain
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
using System.Text;
using System.Text.RegularExpressions;

namespace Hammertime
{
    public class HammerMainException : System.Exception
    {
        string _message;

        // ==============================================================
        public HammerMainException(string message) : base(message)
        // ==============================================================
        {
            _message = message;
        }
    }

    class HammerMain
    {
        // ==============================================================
        public static bool ReadSurveyResults { get; set; }
        public static bool SaveTeams { get; set; }
        // ==============================================================
        private static string Uid { get; set; }
        private static string Password { get; set; }
        private static string Server { get; set; }
        private static string Database { get; set; }
        // ==============================================================

        // ==============================================================
        private static void GetCredentials()
        // ==============================================================
        {
            ConsoleKeyInfo cki;
            StringBuilder sb = new StringBuilder();

            // Get the user
            Console.WriteLine();
            Console.Write("Login: ");
            Uid = Console.ReadLine();

            // Get the password
            Console.Write("password (0-9, A-Z, a-z, +, -, _, %, ^): ");

            do
            {
                cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.Escape)
                {
                    Password = null;
                    Console.WriteLine();
                    Console.WriteLine();
                    return;
                }

                if ((cki.KeyChar >= '0' && cki.KeyChar <= '9') ||
                    (cki.KeyChar >= 'A' && cki.KeyChar <= 'Z') ||
                    (cki.KeyChar >= 'a' && cki.KeyChar <= 'z') ||
                    (cki.KeyChar >= '+') ||
                    (cki.KeyChar >= '-') ||
                    (cki.KeyChar >= '_') ||
                    (cki.KeyChar >= '%') ||
                    (cki.KeyChar >= '^'))
                {
                    sb.Append(cki.KeyChar);
                }

                if ((cki.Key == ConsoleKey.Backspace) && (sb.Length >= 1))
                    sb.Length--;

            } while (cki.Key != ConsoleKey.Enter);

            Password = sb.ToString();

            Console.WriteLine();
            // Console.WriteLine($"uid: {Uid}");
            // Console.WriteLine($"password: {Password}");
            Console.WriteLine();
        }

        // ==============================================================
        private static string getKbdInput()
        // ==============================================================
        {
            ConsoleKeyInfo cki;
            StringBuilder sb = new StringBuilder();
            string inputString = null;

            do
            {
                cki = Console.ReadKey(false);

                if (cki.Key == ConsoleKey.Escape)
                {
                    inputString = null;
                    Console.WriteLine();
                    Console.WriteLine();
                    return inputString;
                }

                if ((cki.KeyChar >= 'A' && cki.KeyChar <= 'Z') ||
                    (cki.KeyChar >= 'a' && cki.KeyChar <= 'z'))
                {
                    sb.Append(cki.KeyChar);
                }

                if ((cki.Key == ConsoleKey.Backspace) && (sb.Length >= 1))
                    sb.Length--;

            } while (cki.Key != ConsoleKey.Enter);

            inputString = sb.ToString();

            Console.WriteLine();

            return inputString;
        }

        // ==============================================================
        private static bool DeletePlayer()
        // ==============================================================
        {
            // delete from mondaynighthockey.players where player_id=;

            bool playerDeleted = false;

            // Get user credentials
            GetCredentials();

            // Temporary
            //HammertimeServer.Instance.AsyncListener();

            // For now default to our Teamopolis database
            Server = "localhost";
            Database = "mondaynighthockey";

            // Log in to server
            DbConnection dbConnection = DbConnection.getInstance(Server, Database, Uid, Password);

            if (dbConnection.Connected)
            {
                string firstName = null;
                string lastName = null;

                do
                {
                    Console.Write("Player's first name: ");
                    firstName = getKbdInput();
                } while (firstName == null);

                do
                {
                    Console.Write("Player's last name: ");
                    lastName = getKbdInput();
                } while (lastName == null);

                string player = firstName + " " + lastName;

                HockeyPlayer dbPlayer = dbConnection.SelectPlayer(player);
                if (dbPlayer != null)
                {
                    Console.WriteLine($"Player {player} found in the db.");
                    string deleteThisPlayer = null;
                    do
                    {
                        Console.Write("Delete this player? (Y/N) ");
                        deleteThisPlayer = getKbdInput();
                    } while ((deleteThisPlayer != "Y") &&
                             (deleteThisPlayer != "N"));

                    if (deleteThisPlayer == "Y")
                    {
                        Console.WriteLine($"delete from mondaynighthockey.players where player_id={dbPlayer.PlayerID};");
                        Console.WriteLine($"Player {player} deleted from db.");
                        playerDeleted = true;
                    }
                }
                else
                    Console.WriteLine($"Player {player} wasn't found in the db.");
            }

            return playerDeleted;
        }

        // ==============================================================
        private static bool AddNewPlayer()
        // ==============================================================
        {
            bool newPlayerAdded = true;
            string firstName = null;
            string lastName = null;
            string position = null;
            string skillLevel = null;
            string goalie = null;
            string playerType = null;
            string anotherPlayer = null;

            // Get user credentials
            GetCredentials();

            // Temporary
            //HammertimeServer.Instance.AsyncListener();

            // For now default to our Teamopolis database
            Server = "localhost";
            Database = "mondaynighthockey";

            // Log in to server
            DbConnection dbConnection = DbConnection.getInstance(Server, Database, Uid, Password);

            if (dbConnection.Connected)
            {
                do
                {
                    do
                    {
                        Console.Write("Player's first name: ");
                        firstName = getKbdInput();
                    } while (firstName == null);

                    do
                    {
                        Console.Write("Player's last name: ");
                        lastName = getKbdInput();
                    } while (lastName == null);

                    do
                    {
                        Console.Write("Player's skill level (A/B/C/D): ");
                        skillLevel = getKbdInput();
                    } while ((skillLevel != "A") &&
                             (skillLevel != "B") &&
                             (skillLevel != "C") &&
                             (skillLevel != "D"));

                    do
                    {
                        Console.Write("Player's position (D, F, G): ");
                        position = getKbdInput();
                    } while ((position != "D") &&
                             (position != "F") &&
                             (position != "G"));

                    if (position == "D") position = "Defense";
                    else if (position == "F") position = "Forward";
                    else position = "Goalie";

                    do
                    {
                        Console.Write("If not a goalie can the player also play as a goalie? (Y/N) ");
                        goalie = getKbdInput();
                    } while ((goalie != "Y") &&
                             (goalie != "N"));

                    do
                    {
                        Console.Write("Is the player full time or a sub? (F/S) ");
                        playerType = getKbdInput();
                    } while ((playerType != "F") &&
                             (playerType != "S"));

                    Console.WriteLine();
                    Console.WriteLine($"About to add new player {firstName} {lastName} to the db.");
                    Console.WriteLine($"{firstName} is a level {skillLevel} player who plays {position}.");
                    Console.WriteLine();
                    string mySqlQuery = $"insert into mondaynighthockey.players (player_last_name, player_first_name, player_level, player_position, player_goalie, player_type, player_team, player_last_wk) values (\"{lastName}\", \"{firstName}\", '{skillLevel[0]}', \"{position}\", '{goalie[0]}', \"{playerType}\", \"Unaffiliated\", \"Zed\")";
                    Console.WriteLine(mySqlQuery);
                    //newPlayerAdded = dbConnection.Insert(mySqlQuery);
                    Console.WriteLine();

                    if (newPlayerAdded)
                    {
                        do
                        {
                            Console.Write("Add another player? (Y/N) ");
                            anotherPlayer = getKbdInput();
                        } while ((anotherPlayer != "Y") &&
                                 (anotherPlayer != "N"));
                    }

                    Console.WriteLine();

                } while ((newPlayerAdded) && (anotherPlayer == "Y"));

                Console.WriteLine();
            }

            return newPlayerAdded;
        }

        // ==============================================================
        private static void CommandLineHelp()
        // ==============================================================
        {
            Console.WriteLine("Hammertime command line help:");
            Console.WriteLine("\t--Help: These instructions.");
            Console.WriteLine("\t--ReadSurveyResults=true/false: default is true.");
            Console.WriteLine("\t--AddNewPlayer: Opens a command line dialog for adding a new player to the database.");
            Console.WriteLine("\t--DeletePlayer: Opens a command line dialog for removing a player from the database.");
            Console.WriteLine("\t--SaveTeams: Writes team assignments to the database.");
        }

        // ==============================================================
        private static bool ParseCmdLineArgs(string[] args)
        // ==============================================================
        {
            bool cmdLineHalt = false;

            if (args.Length == 0)
            {
                // Just run the program
                ReadSurveyResults = true;   // Default
            }
            else if (args.Length > 1)
            {
                throw (new HammerMainException("Error: Only one command line argument allowed."));
            }
            else
            {
                foreach (string arg in args)
                {
                    if (arg == "--Help" || arg == "--help")
                    {
                        Console.WriteLine();
                        CommandLineHelp();
                        cmdLineHalt = true;
                    }
                    else if (arg == "--AddNewPlayer")
                    {
                        Console.WriteLine();
                        if (AddNewPlayer() == false)
                            throw (new HammerMainException("Error: Couldn't add the new player."));
                        cmdLineHalt = true;
                    }
                    else if (arg == "--DeletePlayer")
                    {
                        Console.WriteLine();
                        if (DeletePlayer() == false)
                            throw (new HammerMainException("Error: Couldn't delete the player."));
                        cmdLineHalt = true;
                    }
                    else if (arg == "--SaveTeams")
                    {
                        Console.WriteLine();
                        ReadSurveyResults = false;
                        SaveTeams = true;
                    }
                    else
                    {
                        string pattern = "=";
                        string[] substrings = Regex.Split(arg, pattern);

                        if (substrings[0] == "--ReadSurveyResults" && (substrings[1] == "true" || substrings[1] == "false"))
                        {
                            if (substrings[1] == "true")
                                ReadSurveyResults = true;
                            else
                                ReadSurveyResults = false;
                        }
                        else throw (new HammerMainException("Error: Unsupported command line argument."));
                    }
                }
            }

            return cmdLineHalt;
        }

        // ==============================================================
        static void Main(string[] args)
        // ==============================================================
        {
            // Parse command line args
            try
            {
                // What does the user want to do?
                if (ParseCmdLineArgs(args) == true)
                    return;

                // Get user credentials
                GetCredentials();

                // Temporary
                //HammertimeServer.Instance.AsyncListener();

                // For now default to our Teamopolis database
                Server   = "localhost";
                Database = "mondaynighthockey";

                // Log in to server
                DbConnection dbConnection = DbConnection.getInstance(Server, Database, Uid, Password);

                if (dbConnection.Connected)
                {
                    HomeTeam white = HomeTeam.Instance;
                    VisitorTeam dark = VisitorTeam.Instance;
                    TeamBalancer balancer = TeamBalancer.Instance;

                    try
                    {
                        balancer.Balance(white, dark);

                        white.PrintHomeTeamRoster();
                        dark.PrintVisitingTeamRoster();

                        if (SaveTeams == true && HockeyTeam.SaveTeams() == false)
                            throw (new HammerMainException("Error: Unable to update database with this week's team assignments."));
                    }
                    catch (TeamBalancerException ex)
                    {
                        Console.WriteLine($"Error running TeamBalancer: {ex.Message}");
                    }
                }
            }
            catch (HammerMainException ex)
            {
                Console.WriteLine();
                Console.WriteLine($"{ex.Message}");
                Console.WriteLine();
                CommandLineHelp();
            }
        }
    }
}
