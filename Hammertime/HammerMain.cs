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
using System.Collections;
using System.Text;

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

        public static ArrayList Credentials()
        {
            ArrayList credentials = new ArrayList();

            credentials.Add(Uid);
            credentials.Add(Password);
            credentials.Add(Server);
            credentials.Add(Database);

            return credentials;
        }

        // ==============================================================
        // Establishes the server, the database, the user ID, and the password.
        private static void SetCredentials()
        // ==============================================================
        {
            ConsoleKeyInfo cki;
            StringBuilder sb = new StringBuilder();

            // For now default to our local Teamopolis database
            Server = "localhost";
            Database = "mondaynighthockey";

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
        static void Main(string[] args)
        // ==============================================================
        {
            // Before all else set user credentials
            // Establishes the server, the database, the user ID, and the password.
            SetCredentials();

            try
            {
                // What does the user want to do?
                // Parse command line args
                // If method returns true then stop any further processing.
                CmdLineProcessor.getInstance().Parse(args);

                // Temporary
                //HammertimeServer.Instance.AsyncListener();

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
            catch (CmdLineProcessorException ex)
            {
                Console.WriteLine();
                Console.WriteLine($"{ex.Message}");
                Console.WriteLine();
            }
        }
    }
}
