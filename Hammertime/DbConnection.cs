// ==============================================================
//
// class DbConnection
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
using System.Linq;
using System.Text;
//Add MySql Library
using MySql.Data.MySqlClient;

namespace Hammertime
{
    public class DbConnection
    {
        private static DbConnection _dbConnection;
        private static MySqlConnection _connection;
        private static string _server, _database, _uid;
        private static string _connectionString;
        private static bool _connected;

        public static DbConnection getInstance(string server=null, string database = null, string uid = null, string password = null)
        {
            if (_dbConnection == null)
            {
                //Console.WriteLine("Setting up DbConnection for the first time.");

                _server = server;
                _database = database;
                _uid = uid;

                _connectionString = "SERVER=" + server + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";" + "DATABASE=" + database + ";";
                _dbConnection = new DbConnection();
            }
            // Console.WriteLine("Exiting DB connection constructor.");
            return _dbConnection;
        }

        //Constructor
        private DbConnection()
        {
            try
            {
                //Console.WriteLine("Logging in to MySqlConnection.");
                _connection = new MySqlConnection(_connectionString);
                _connected = true;
                //Console.WriteLine($"Connected to {_server}:{_database} as {_uid}.");
            }
            catch (MySqlException ex)
            {
                ParseException(ex);
                _connected = false;
                //Console.WriteLine($"Connection to {_server}:{_database} failed.");
            }
        }

        public bool Connected
        {
            get { return _connected; }
        }

        //open connection to database
        public bool OpenConnection()
        {
            try
            {
                //Console.WriteLine($"Opening connection to {_server}:{_database} as {_uid}.");
                _connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //Console.WriteLine($"Opening connection to {_server}:{_database} failed.");
                ParseException(ex);
                return false;
            }
        }

        //Close connection
        public bool CloseConnection()
        {
            try
            {
                //Console.WriteLine($"Closing connection to {_server}:{_database}.");
                _connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                // MessageBox.Show(ex.Message);
                //Console.WriteLine($"Closing connection to {_server}:{_database} failed.");
                ParseException(ex);
                return false;
            }
        }

        //Insert statement
        public void Insert()
        {
        }

        //Update statement
        public void Update()
        {
        }

        //Delete statement
        public void Delete()
        {
        }

        //Select statement
        public List<string>[] Select()
        {
            // To execute a Select statement, we add a few more steps, and we use the ExecuteReader method that will return a dataReader object to read and store the data or records.

            // Open connection to the database.
            // Create a MySQL command.
            // Assign a connection and a query to the command.This can be done using the constructor, or using the Connection and the CommandText methods in the MySqlCommand class.
            // Create a MySqlDataReader object to read the selected records/data.
            // Execute the command.
            // Read the records and display them or store them in a list.
            // Close the data reader.
            // Close the connection.

            string query = "SELECT * FROM players";

            //Create a list to store the result
            List<string>[] list = new List<string>[8];
            list[0] = new List<string>();   // List of player IDs
            list[1] = new List<string>();   // List of player last names
            list[2] = new List<string>();   // List of player first names
            list[3] = new List<string>();   // List of player levels (A, B, C, D)
            list[4] = new List<string>();   // List of player positions
            list[5] = new List<string>();   // List of player types (F/S)
            list[6] = new List<string>();   // List of player teams (Barry, Ben, Unaffiliated)
            list[7] = new List<string>();   // List of player team assignments for last week

            //Open connection
            if (OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, _connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["player_id"] + "");
                    list[1].Add(dataReader["player_last_name"] + "");
                    list[2].Add(dataReader["player_first_name"] + "");
                    list[3].Add(dataReader["player_level"] + "");
                    list[4].Add(dataReader["player_position"] + "");
                    list[5].Add(dataReader["player_type"] + "");
                    list[6].Add(dataReader["player_team"] + "");
                    list[7].Add(dataReader["player_last_wk"] + "");
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                CloseConnection();

                //return list to be displayed
                return list;
            }
            else
            {
                return list;
            }
        }

        //Count statement
        public int Count()
        {
            string query = "SELECT Count(*) FROM players";
            int Count = 0;

            //Open Connection
            if (OpenConnection() == true)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, _connection);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar() + "");

                //close Connection
                CloseConnection();
            }

            return Count;
        }

        //Backup
        public void Backup()
        {
        }

        //Restore
        public void Restore()
        {
        }

        // Parse Exceptions
        private void ParseException(MySqlException ex)
        {
            //When handling errors, you can your application's response based 
            //on the error number.
            //The two most common error numbers when connecting are as follows:
            //0: Cannot connect to server.
            //1045: Invalid user name and/or password.
            switch (ex.Number)
            {
                case 0:
                    // MessageBox.Show("Cannot connect to server.  Contact administrator");
                    Console.WriteLine("Cannot connect to server. Contact administrator");
                    break;

                case 1045:
                    // MessageBox.Show("Invalid username/password, please try again");
                    Console.WriteLine("Invalid username/password. Please try again");
                    break;

                default:
                    Console.WriteLine("Unknown DB server Error. Contact Administrator.");
                    break;
            }
        }
    }
}
