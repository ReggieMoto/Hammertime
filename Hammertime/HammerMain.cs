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

namespace Hammertime
{
    class HammerMain
    {
        static void getCredentials(out string uid, out string password)
        {
            ConsoleKeyInfo cki;
            StringBuilder sb = new StringBuilder();

            // Get the user
            Console.WriteLine();
            Console.Write("Login: ");
            uid = Console.ReadLine();

            // Get the password
            Console.Write("password (0-9, A-Z, a-z, +, -, _, %, ^): ");

            do
            {
                cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.Escape)
                {
                    password = null;
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

            password = sb.ToString();

            Console.WriteLine();
            // Console.WriteLine($"uid: {uid}");
            // Console.WriteLine($"password: {password}");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            string server = "localhost";
            string database = "mondaynighthockey";
            string uid = null;
            string password = null;

            // Get user credentials
            getCredentials(out uid, out password);

            // Log in to server
            DbConnection dbConnection = DbConnection.getInstance(server, database, uid, password);

            if (dbConnection.Connected)
            {
                HomeTeam white = HomeTeam.Instance;
                white.printHomeTeamRoster();

                VisitorTeam dark = VisitorTeam.Instance;
                dark.printVisitingTeamRoster();
            }
        }
    }
}
