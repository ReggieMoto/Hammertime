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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        private static string _availablePlayersFilename = "availablePlayers.txt";

        // ==============================================================
        // 1. Does the file exist?
        // 2. If so, open it for reading.
        // 3. If not throw an exception.
        // ==============================================================
        public ArrayList ReadAvailablePlayers()
        // ==============================================================
        {
            ArrayList availablePlayers = new ArrayList();

            string filenamePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullyQualifiedFilename = filenamePath + "\\" + _availablePlayersFilename;


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

            return availablePlayers;
        }

        // ==============================================================
        public void WriteAvailablePlayers(ArrayList availablePlayers)
        // ==============================================================
        {
            string filenamePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullyQualifiedFilename = filenamePath + "\\" + _availablePlayersFilename;

            if (File.Exists(fullyQualifiedFilename))
            {
                // Rename the old file
                if (File.Exists(fullyQualifiedFilename + ".old"))
                    File.Delete(fullyQualifiedFilename + ".old");

                File.Copy(fullyQualifiedFilename, fullyQualifiedFilename+".old");
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
    }
}