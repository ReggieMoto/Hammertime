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

            } while (cki.Key != ConsoleKey.Enter);

            Password = sb.ToString();

            Console.WriteLine();
            // Console.WriteLine($"uid: {Uid}");
            // Console.WriteLine($"password: {Password}");
            Console.WriteLine();
        }

        // ==============================================================
        private static void CommandLineHelp()
        // ==============================================================
        {
            Console.WriteLine("Hammertime command line help:");
            Console.WriteLine("\t--Help: These instructions.");
            Console.WriteLine("\t--ReadSurveyResults=true/false: default is true.");
            Console.WriteLine("\t--AddNewPlayer: Opens a command line dialog for adding a new player to the database.");
            Console.WriteLine("\t--SaveTeams: Writes team assignments to the database.");
        }

        // ==============================================================
        private static void ParseCmdLineArgs(string[] args)
        // ==============================================================
        {
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
                    }
                    else if (arg == "--AddNewPlayer")
                    {
                        Console.WriteLine();
                        if (HockeyPlayer.NewPlayer() == false)
                            throw (new HammerMainException("Error: Currently unsupported command."));
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
        }

        // ==============================================================
        static void Main(string[] args)
        // ==============================================================
        {
            // Parse command line args
            try
            {
                // What does the user want to do?
                ParseCmdLineArgs(args);

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
