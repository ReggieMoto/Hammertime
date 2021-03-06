﻿// ==============================================================
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
                    _dbConnection = MySqlDbConnection.getInstance();
                else if (Server == DbConnection.Server.MongoDb)
                    _dbConnection = MongoDbConnection.getInstance();
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
        static void Main(string[] args)
        // ==============================================================
        {
            try
            {
                // What does the user want to do?
                CmdLineProcessor.getInstance().Parse(args);

                try
                {
                    // Get the available players and assemble two teams
                    TeamBuilder.Instance.BuildTeams();

                    try
                    {
                        TeamBalancer.Instance.Balance();

                        HomeTeam.Instance.PrintRoster();
                        VisitorTeam.Instance.PrintRoster();

                        if (SaveTeams == true && TeamBuilder.SaveTeams() == false)
                            throw (new HammerMainException("Error: Unable to update database with this week's team assignments."));
                    }
                    catch (TeamBalancerException ex)
                    {
                        Console.WriteLine($"Error running TeamBalancer: {ex.Message}");
                    }
                }
                catch (TeamBuilderException ex)
                {
                    Console.WriteLine($"Error running TeamBuilder: {ex.Message}");
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
