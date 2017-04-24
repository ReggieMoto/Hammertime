// ==============================================================
//
// class AvailablePlayerFileIO
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
using System.IO;

namespace Hammertime
{
    public class AvailablePlayerFileIOException : System.Exception
    {
        string _message;

        // ==============================================================
        public AvailablePlayerFileIOException(string message) : base(message)
        // ==============================================================
        {
            _message = message;
        }
    }

    public class AvailablePlayerFileIO
    {
        // ==============================================================
        public static AvailablePlayerFileIO Instance
        // ==============================================================
        {
            get
            {
                if (_instance == null)
                    _instance = new AvailablePlayerFileIO();
                return _instance;
            }
        }

        private static AvailablePlayerFileIO _instance;
        private static string filenameSubdir = @"\Hammertime\";
        private static string _availablePlayersFilename = "availablePlayers.txt";

        // ==============================================================
        // 1. Does the file exist?
        // 2. If so, open it for reading.
        // 3. If not throw an exception.
        // ==============================================================
        public List<string> ReadAvailablePlayers()
        // ==============================================================
        {
            List<string> availablePlayers = new List<string>();

            string filenamePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            filenamePath += filenameSubdir;
            string fullyQualifiedFilename = filenamePath + _availablePlayersFilename;

            if (Directory.Exists(filenamePath))
            {
                if (File.Exists(fullyQualifiedFilename))
                {
                    try
                    {
                        // Create an instance of StreamReader to read from a file.
                        // The using statement also closes the StreamReader.
                        using (StreamReader sr = new StreamReader(fullyQualifiedFilename))
                        {
                            String line;
                            // Read and display lines from the file until the end of
                            // the file is reached.
                            while ((line = sr.ReadLine()) != null)
                                availablePlayers.Add(line);
                        }
                    }
                    catch (Exception e)
                    {
                        // Let the user know what went wrong.
                        Console.Write("Error: The file could not be read: ");
                        Console.WriteLine(e.Message);
                    }
                }
                else throw (new AvailablePlayerFileIOException("File does not exist."));
            }
            else
            {
                Console.WriteLine($"Directory does not exist: {filenamePath}");
            }

            return availablePlayers;
        }

        // ==============================================================
        public void WriteAvailablePlayers(List<string> availablePlayers)
        // ==============================================================
        {
            string filenamePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            filenamePath += filenameSubdir;
            string fullyQualifiedFilename = filenamePath + _availablePlayersFilename;
            DateTime localDate = DateTime.Now;

            string year = Convert.ToString(localDate.Year);
            string month = Convert.ToString(localDate.Month);
            if (month.Length == 1)
                month = "0" + month;

            string day = Convert.ToString(localDate.Day);
            if (day.Length == 1)
                day = "0" + day;

            string hour = Convert.ToString(localDate.Hour);
            if (hour.Length == 1)
                hour = "0" + hour;

            string minute = Convert.ToString(localDate.Minute);
            if (minute.Length == 1)
                minute = "0" + minute;

            string second = Convert.ToString(localDate.Second);
            if (second.Length == 1)
                second = "0" + second;

            string date = year + month + day + "_" + hour + minute + second + "_";

            if (Directory.Exists(filenamePath))
            {
                if (File.Exists(fullyQualifiedFilename))
                {
                    // Rename the old file
                    File.Copy(fullyQualifiedFilename, filenamePath + date + _availablePlayersFilename);
                }

                try
                {
                    // Write the list of available players to the file
                    using (StreamWriter outputFile = new StreamWriter(fullyQualifiedFilename))
                    {
                        foreach (string line in availablePlayers)
                            outputFile.WriteLine(line);
                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.Write("Error: The file could not be read: ");
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine($"Directory does not exist: {filenamePath}");
            }
        }
    }
}