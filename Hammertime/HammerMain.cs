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
    // =====================================================
    public class HammerMainException : System.Exception
    // =====================================================
    {
        string _message;

        // ==============================================================
        public HammerMainException(string message) : base(message)
        // ==============================================================
        {
            _message = message;
        }
    }

    // =====================================================
    // Establish the database for program. THIS IS THE PLACE!
    // Invoked in both the CmdLineProcessor constructor and
    // the CmdLineProcessor Parser.
    //
    public class HammerMainDb
    // =====================================================
    {
        private static DbConnection _dbConnection = null;

        public static DbConnection.Server Server { get; set; }

        // =====================================================
        public static DbConnection getInstance(string uid = null, string password = null)
        // =====================================================
        {
            if (_dbConnection == null)
            {
                if (Server == DbConnection.Server.MySql)
                    _dbConnection = MySqlDbConnection.getInstance(uid, password);
                else if (Server == DbConnection.Server.MongoDb)
                {
                    _dbConnection = MongoDbConnection.getInstance(uid, password);
                    //throw new HammerMainException("Error: MongoDb Server not implemented yet.");
                }
                else
                    throw new HammerMainException("Error: DB Server unavailable.");
            }
            
            return _dbConnection;
        }
    }

    // =====================================================
    class HammerMain
    // =====================================================
    {
        // ==============================================================
        public static bool ReadSurveyResults { get; set; }
        public static bool SaveTeams { get; set; }
        // ==============================================================
        private static string Uid { get; set; }
        private static string Password { get; set; }
        // ==============================================================

        /*
        // =====================================================
        public static ArrayList Credentials()
        // =====================================================
        {
            ArrayList credentials = new ArrayList();

            credentials.Add(Uid);
            credentials.Add(Password);
            return credentials;
        }
        */

        // ==============================================================
        // Establishes the server, the database, the user ID, and the password.
        private static void SetCredentials()
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
        static void Main(string[] args)
        // ==============================================================
        {
            // Before all else set user credentials
            // Establishes the user ID, and the password.
            SetCredentials();

            // Next establish the default database.

            try
            {
                // What does the user want to do?
                // Parse command line args
                CmdLineProcessor.getInstance().Parse(args);

                // Temporary
                //HammertimeServer.Instance.AsyncListener();

                try
                {
                    // Log in to server
                    DbConnection dbConnection = HammerMainDb.getInstance(Uid, Password);

                    if (dbConnection.Connected())
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
