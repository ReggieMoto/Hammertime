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
            UpdatePlayer,
            DeletePlayer,
            PlayerAttrs,
            BackupDb,
            RestoreDb,
            SaveTeams,
            ReadSurveyResults,
            Count,
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
            Console.WriteLine("\t--UpdatePlayer: Opens a command line dialog for modifying a player's attributes.");
            Console.WriteLine("\t--DeletePlayer: Opens a command line dialog for removing a player from the database.");
            Console.WriteLine("\t--PlayerAttrs: Opens a command line dialog to search for a player and return that player's attributes.");
            Console.WriteLine("\t--SaveTeams: Writes team assignments to the database.");
            Console.WriteLine("\t--Backup: Backup the database to the local disk.");
            Console.WriteLine("\t--Restore: Restore the database from the local disk.");
            Console.WriteLine("\t--Count: Count of players in database.");
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
                    else if (arg == "--UpdatePlayer")
                    {
                        Console.WriteLine();
                        if (UpdatePlayer() == false)
                            throw (new CmdLineProcessorException(CmdLineProcessorException.ExceptionID.UpdatePlayer, "Error: Couldn't find the new player nor update the attributes."));
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
                    else if (arg == "--Count")
                    {
                        int count = Count();

                        Console.WriteLine();
                        if (count == 0)
                            Console.WriteLine("There aren't any player records in the database.");
                        else if (count == 1)
                            Console.WriteLine("There is one player record in the database.");
                        else
                            Console.WriteLine($"There are {count} player records in the database.");

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
        private static int Count()
        // ==============================================================
        {
            int count = 0;

            DbConnection dbConnection = HammerMainDb.getInstance();

            if (dbConnection.Connected())
            {
                count = dbConnection.Count();
            }

            return count;
        }

        // ==============================================================
        private static bool RestoreDb()
        // ==============================================================
        {
            bool dbRestored = false;
            //ArrayList credentials = HammerMain.Credentials();

            // Log in to server
            DbConnection dbConnection = HammerMainDb.getInstance();

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
            //ArrayList credentials = HammerMain.Credentials();

            // Log in to server
            DbConnection dbConnection = HammerMainDb.getInstance();

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
            bool playerFound = false;
            //ArrayList credentials = HammerMain.Credentials();

            // Log in to server
            DbConnection dbConnection = HammerMainDb.getInstance();

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
                    Console.WriteLine();
                    Console.WriteLine($"Player {player}:");
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
            bool playerDeleted = false;
            //ArrayList credentials = HammerMain.Credentials();

            // Log in to server
            DbConnection dbConnection = HammerMainDb.getInstance();

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
                        playerDeleted = dbConnection.Delete(dbPlayer);
                        if (playerDeleted)
                        {
                            Console.WriteLine($"Player {player} was deleted from db.");
                        }
                        else
                            Console.WriteLine($"Player {player} was not deleted from db.");
                    }
                }
                else
                    Console.WriteLine($"Player {player} wasn't found in the db.");
            }

            return playerDeleted;
        }

        // ==============================================================
        private static bool UpdatePlayer()
        // ==============================================================
        {
            bool updateStatus = false;
            string position = null;
            string plyrSkillLevel = null;
            string canPlayGoalie = null;
            string playerType = null;

            bool goalie;
            HockeyPlayer.PlayerSkill skillLevel;

            // Log in to server
            DbConnection dbConnection = HammerMainDb.getInstance();

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
                    Console.WriteLine();
                    Console.WriteLine($"Player {player}:");
                    Console.WriteLine($"\tSkill level: {dbPlayer.Level}");
                    Console.WriteLine($"\tPosition: {dbPlayer.PlayerPos}");
                    Console.WriteLine($"\tCan play goalie: {dbPlayer.Goalie}");
                    Console.WriteLine($"\tFulltime or sub: {dbPlayer.PlayerType}");
                    Console.WriteLine($"\tTeam affiliation: {dbPlayer.PlayerTeam}");
                    Console.WriteLine($"\tLast week jersey color: {dbPlayer.PlayerLastWeek}");
                    Console.WriteLine();
                    Console.WriteLine($"Modify player's attributes.");

                    do
                    {
                        Console.Write("Player's skill level (A/B/C/D): ");
                        plyrSkillLevel = getKbdInput();
                    } while ((plyrSkillLevel != "A") &&
                             (plyrSkillLevel != "B") &&
                             (plyrSkillLevel != "C") &&
                             (plyrSkillLevel != "D"));

                    if (plyrSkillLevel == "A")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_A;
                    else if (plyrSkillLevel == "B")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_B;
                    else if (plyrSkillLevel == "C")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_C;
                    else // (plyrSkillLevel == "D")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_D;

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
                        canPlayGoalie = getKbdInput();
                    } while ((canPlayGoalie != "Y") &&
                             (canPlayGoalie != "N"));
                    if (canPlayGoalie == "Y")
                        goalie = true;
                    else
                        goalie = false;

                    do
                    {
                        Console.Write("Is the player full time or a sub? (F/S) ");
                        playerType = getKbdInput();
                    } while ((playerType != "F") &&
                             (playerType != "S"));

                    Console.WriteLine();
                    Console.WriteLine($"Updating player {player}'s attributes in the db.");
                    Console.WriteLine($"{firstName} is a level {plyrSkillLevel} player who plays {position}.");
                    Console.WriteLine();

                    HockeyPlayer hockeyPlayer = new HockeyPlayer(
                        lastName,
                        firstName,
                        skillLevel,
                        position,
                        goalie,
                        playerType[0],
                        "Unaffiliated",
                        "Zed");

                    updateStatus = dbConnection.Update(hockeyPlayer);
                    Console.WriteLine();
                }
                else
                    Console.WriteLine($"Player {player} wasn't found in the db.");
            }

            return updateStatus;
        }

        // ==============================================================
        private static bool AddNewPlayer()
        // ==============================================================
        {
            bool newPlayerAdded = true;
            string firstName = null;
            string lastName = null;
            string position = null;
            string plyrSkillLevel = null;
            string canPlayGoalie = null;
            string playerType = null;
            string anotherPlayer = null;

            bool goalie;
            HockeyPlayer.PlayerSkill skillLevel;

            // Log in to server
            DbConnection dbConnection = HammerMainDb.getInstance();

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
                        plyrSkillLevel = getKbdInput();
                    } while ((plyrSkillLevel != "A") &&
                             (plyrSkillLevel != "B") &&
                             (plyrSkillLevel != "C") &&
                             (plyrSkillLevel != "D"));

                    if (plyrSkillLevel == "A")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_A;
                    else if (plyrSkillLevel == "B")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_B;
                    else if (plyrSkillLevel == "C")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_C;
                    else // (plyrSkillLevel == "D")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_D;

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
                        canPlayGoalie = getKbdInput();
                    } while ((canPlayGoalie != "Y") &&
                             (canPlayGoalie != "N"));
                    if (canPlayGoalie == "Y")
                        goalie = true;
                    else
                        goalie = false;

                    do
                    {
                        Console.Write("Is the player full time or a sub? (F/S) ");
                        playerType = getKbdInput();
                    } while ((playerType != "F") &&
                             (playerType != "S"));

                    Console.WriteLine();
                    Console.WriteLine($"About to add new player {firstName} {lastName} to the db.");
                    Console.WriteLine($"{firstName} is a level {plyrSkillLevel} player who plays {position}.");
                    Console.WriteLine();

                    HockeyPlayer player = new HockeyPlayer(
                        lastName,
                        firstName,
                        skillLevel,
                        position,
                        goalie,
                        playerType[0],
                        "Unaffiliated",
                        "Zed");

                    newPlayerAdded = dbConnection.Insert(player);
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
