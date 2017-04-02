// ==============================================================
//
// class CmdLineProcessor
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
using System.Text.RegularExpressions;

namespace Hammertime
{
    public class CmdLineProcessorException : System.Exception
    {
        string _message;

        public enum ExceptionID
        {
            Benign,
            TooManyArgs,
            DbUnavailable,
            AddNewPlayer,
            DeletePlayer,
            PlayerAttrs,
            BackupDb,
            RestoreDb,
            SaveTeams,
            ReadSurveyResults,
            UnrecognizedArgs
        };

        // ==============================================================
        public CmdLineProcessorException(ExceptionID exceptionId, string message) : base(message)
        // ==============================================================
        {
            _message = message;

            if (exceptionId == ExceptionID.TooManyArgs)
                CmdLineProcessor.getInstance().Help();
        }
    }

    class CmdLineProcessor
    {
        private static CmdLineProcessor _cmdLineProcessor = null;

        private static string Uid { get; set; }
        private static string Password { get; set; }
        private static string Server { get; set; }
        private static string Database { get; set; }

        // =====================================================
        public static CmdLineProcessor getInstance()
        // =====================================================
        {
            if (_cmdLineProcessor == null)
            {
               _cmdLineProcessor = new CmdLineProcessor();
            }
            // Console.WriteLine("Exiting DB connection constructor.");
            return _cmdLineProcessor;
        }

        // =====================================================
        // Constructor
        private CmdLineProcessor()
        // =====================================================
        {
            // This is the default DB Server.
            // This could get modified during argument parsing.
            HammerMainDb.Server = DbConnection.Server.MySql;
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
        public void Help()
        // ==============================================================
        {
            Console.WriteLine("Hammertime command line help:");
            Console.WriteLine("\t--Help: These instructions.");
            Console.WriteLine("\t--MySQL: Using MySQL Server.");
            Console.WriteLine("\t--MongoDb: Using MongoDb Server.");
            Console.WriteLine("\t--ReadSurveyResults=true/false: default is true.");
            Console.WriteLine("\t--AddNewPlayer: Opens a command line dialog for adding a new player to the database.");
            Console.WriteLine("\t--DeletePlayer: Opens a command line dialog for removing a player from the database.");
            Console.WriteLine("\t--PlayerAttrs: Opens a command line dialog to search for a player and return that player's attributes.");
            Console.WriteLine("\t--SaveTeams: Writes team assignments to the database.");
            Console.WriteLine("\t--Backup: Backup the database to the local disk.");
            Console.WriteLine("\t--Restore: Restore the database from the local disk.");
        }

        // ==============================================================
        public void Parse(string[] args)
        // ==============================================================
        {
            bool cmdLineHalt = false;

            if (args.Length == 0)
            {
                // Just run the program
                HammerMain.ReadSurveyResults = true;   // Default
            }
            else if (args.Length > 2)
            {
                throw (new CmdLineProcessorException(CmdLineProcessorException.ExceptionID.TooManyArgs, "Error: Only one command line argument allowed."));
            }
            else
            {
                foreach (string arg in args)
                {
                    if (arg == "--Help" || arg == "--help")
                    {
                        Console.WriteLine();
                        Help();
                        cmdLineHalt = true;
                    }
                    else if (arg == "--MongoDb")
                    {
                        HammerMainDb.Server = DbConnection.Server.MongoDb;
                    }
                    else if (arg == "--MySql")
                    {
                        // Already initialized as default
                        HammerMainDb.Server = DbConnection.Server.MySql;
                    }
                    else if (arg == "--AddNewPlayer")
                    {
                        Console.WriteLine();
                        if (AddNewPlayer() == false)
                            throw (new CmdLineProcessorException(CmdLineProcessorException.ExceptionID.AddNewPlayer, "Error: Couldn't add the new player."));
                        cmdLineHalt = true;
                    }
                    else if (arg == "--DeletePlayer")
                    {
                        Console.WriteLine();
                        if (DeletePlayer() == false)
                            throw (new CmdLineProcessorException(CmdLineProcessorException.ExceptionID.DeletePlayer, "Error: Couldn't delete the player."));
                        cmdLineHalt = true;
                    }
                    else if (arg == "--PlayerAttrs")
                    {
                        Console.WriteLine();
                        if (PlayerAttrs() == false)
                            throw (new CmdLineProcessorException(CmdLineProcessorException.ExceptionID.PlayerAttrs, "Error: Couldn't retrieve the player's attributes."));
                        cmdLineHalt = true;
                    }
                    else if (arg == "--SaveTeams")
                    {
                        Console.WriteLine();
                        HammerMain.ReadSurveyResults = false;
                        HammerMain.SaveTeams = true;
                    }
                    else if (arg == "--Backup")
                    {
                        Console.WriteLine();
                        if (BackupDb() == false)
                            throw (new CmdLineProcessorException(CmdLineProcessorException.ExceptionID.BackupDb, "Error: Couldn't backup the database."));
                        cmdLineHalt = true;
                    }
                    else if (arg == "--Restore")
                    {
                        Console.WriteLine();
                        if (RestoreDb() == false)
                            throw (new CmdLineProcessorException(CmdLineProcessorException.ExceptionID.RestoreDb, "Error: Couldn't restore the database."));
                        cmdLineHalt = true;
                    }
                    else
                    {
                        string pattern = "=";
                        string[] substrings = Regex.Split(arg, pattern);

                        if (substrings[0] == "--ReadSurveyResults" && (substrings[1] == "true" || substrings[1] == "false"))
                        {
                            if (substrings[1] == "true")
                                HammerMain.ReadSurveyResults = true;
                            else
                                HammerMain.ReadSurveyResults = false;
                        }
                        else throw (new CmdLineProcessorException(CmdLineProcessorException.ExceptionID.UnrecognizedArgs, "Error: Unsupported command line argument."));
                    }
                }
            }

            if (cmdLineHalt == true)
                throw (new CmdLineProcessorException(CmdLineProcessorException.ExceptionID.Benign, ""));
        }

        // ==============================================================
        private static bool RestoreDb()
        // ==============================================================
        {
            bool dbRestored = false;

            // Log in to server
            DbConnection dbConnection = HammerMainDb.getInstance(Server, Database, Uid, Password);

            if (dbConnection.Connected())
            {
                dbRestored = dbConnection.Restore();
            }

            return dbRestored;
        }

        // ==============================================================
        private static bool BackupDb()
        // ==============================================================
        {
            bool dbBackedUp = false;

            // Log in to server
            DbConnection dbConnection = HammerMainDb.getInstance(Server, Database, Uid, Password);

            if (dbConnection.Connected())
            {
                dbBackedUp = dbConnection.Backup();
            }

            return dbBackedUp;
        }

        // ==============================================================
        private static bool PlayerAttrs()
        // ==============================================================
        {
            // delete from mondaynighthockey.players where player_id=;

            bool playerFound = false;

            // Log in to server
            DbConnection dbConnection = HammerMainDb.getInstance(Server, Database, Uid, Password);

            if (dbConnection.Connected())
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

                HockeyPlayer dbPlayer = dbConnection.Read(player);
                if (dbPlayer != null)
                {
                    playerFound = true;
                    Console.WriteLine($"Player {player}:");
                    Console.WriteLine($"\tDB ID: {dbPlayer.PlayerID}");
                    Console.WriteLine($"\tSkill level: {dbPlayer.Level}");
                    Console.WriteLine($"\tPosition: {dbPlayer.PlayerPos}");
                    Console.WriteLine($"\tCan play goalie: {dbPlayer.Goalie}");
                    Console.WriteLine($"\tFulltime or sub: {dbPlayer.PlayerType}");
                    Console.WriteLine($"\tTeam affiliation: {dbPlayer.PlayerTeam}");
                    Console.WriteLine($"\tLast week jersey color: {dbPlayer.PlayerLastWeek}");
                }
                else
                    Console.WriteLine($"Player {player} wasn't found in the db.");
            }

            return playerFound;
        }

        // ==============================================================
        private static bool DeletePlayer()
        // ==============================================================
        {
            // delete from mondaynighthockey.players where player_id=;

            bool playerDeleted = false;

            // Log in to server
            DbConnection dbConnection = HammerMainDb.getInstance(Server, Database, Uid, Password);

            if (dbConnection.Connected())
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

                HockeyPlayer dbPlayer = dbConnection.Read(player);
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

            // Log in to server
            DbConnection dbConnection = HammerMainDb.getInstance(Server, Database, Uid, Password);

            if (dbConnection.Connected())
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
    }
}
