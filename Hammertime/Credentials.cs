// ==============================================================
//
// class Credentials
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
using System.Text;

namespace Hammertime
{
    class Credentials
    {
        // ==============================================================
        private static string Uid { get; set; }
        private static string Password { get; set; }
        // ==============================================================

        private static Credentials _credentials = null;

        // =====================================================
        public static List<string> getCredentials()
        // =====================================================
        {
            List<string> credentials = new List<string>();

            if (_credentials == null)
            {
                _credentials = new Credentials();
                credentials.Add(Uid);
                credentials.Add(Password);
            }
            // Console.WriteLine("Exiting DB connection constructor.");
            return credentials;
        }

        // ==============================================================
        // Establishes the user ID, and the password.
        private Credentials()
        // ==============================================================
        {
            ConsoleKeyInfo cki;
            StringBuilder sb = new StringBuilder();

            // Get the user
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
            Console.WriteLine();
        }
    }
}
