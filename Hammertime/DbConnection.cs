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
using System.Collections;
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

        // =====================================================
        public static DbConnection getInstance(string server = null, string database = null, string uid = null, string password = null)
        // =====================================================
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

        // =====================================================
        //Constructor
        private DbConnection()
        // =====================================================
        {
            _connection = new MySqlConnection(_connectionString);

            try
            {
                _connected = OpenConnection();
                if (Connected)
                    CloseConnection();
            }
            catch (MySqlException ex)
            {
                ParseException(ex);
                _connected = false;
            }
        }

        // =====================================================
        public bool Connected
        // =====================================================
        {
            get { return _connected; }
        }

        // =====================================================
        //open connection to database
        public bool OpenConnection()
        // =====================================================
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

        // =====================================================
        //Close connection
        public bool CloseConnection()
        // =====================================================
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

        // =====================================================
        //Update statement
        public bool Update(string mySqlQuery)
        // =====================================================
        {
            bool updateStatus = false;

            if (OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(mySqlQuery, _connection);
                // Execute the UPDATE
                int affectedRows = cmd.ExecuteNonQuery();
                // Finished with the command
                CloseConnection();

                if (affectedRows == 1)
                    updateStatus = true;
            }

            return updateStatus;
        }

        //Delete statement
        public void Delete()
        {
        }

        // =====================================================
        // Select Statement
        public HockeyPlayer SelectPlayer(string playerName)    // Expect "first last"
        // =====================================================
        {
            HockeyPlayer player = null;
            string query = null;

            if (OpenConnection() == true)
            {
                // Parse the string into last_name/first_name
                string[] name = playerName.Split(' ');
                if (name.Length == 3 && name[1] == "St.")
                    query = $"SELECT * FROM players WHERE player_last_name = \"" + name[1] + " " + name[2] + "\" AND player_first_name = \"" + name[0] + "\"";
                else
                    query = $"SELECT * FROM players WHERE player_last_name = \"" + name[1] + "\" AND player_first_name = \"" + name[0] + "\"";

                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, _connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    var index = dataReader["player_id"] + "";
                    var lastName = dataReader["player_last_name"] + "";
                    var firstName = dataReader["player_first_name"] + "";
                    var level = dataReader["player_level"] + "";
                    var position = dataReader["player_position"] + "";
                    var goalie = dataReader["player_goalie"] + "";
                    var type = dataReader["player_type"] + "";
                    var team = dataReader["player_team"] + "";
                    var lastWeek = dataReader["player_last_wk"] + "";

                    int playerId;
                    int.TryParse(index, out playerId);

                    HockeyPlayer.PlayerSkill skillLevel;
                    if (level == "D")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_D;
                    else if (level == "C")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_C;
                    else if (level == "B")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_B;
                    else // (level == "A")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_A;

                    bool canPlayGoalie = false;
                    if (goalie == "Y")
                        canPlayGoalie = true;

                    player = new HockeyPlayer(playerId, lastName, firstName, skillLevel, position, canPlayGoalie, type[0], team, lastWeek);
                }

                CloseConnection();
            }

            return player;

        }

        // =====================================================
        // Select Statement
        public ArrayList SelectAllPlayers()
        // =====================================================
        {
            ArrayList playerList = new ArrayList();

            if (OpenConnection() == true)
            {
                string query = "SELECT * FROM players ORDER BY player_level";

                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, _connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    var index     = dataReader["player_id"] + "";
                    var lastName  = dataReader["player_last_name"] + "";
                    var firstName = dataReader["player_first_name"] + "";
                    var level     = dataReader["player_level"] + "";
                    var position  = dataReader["player_position"] + "";
                    var goalie    = dataReader["player_goalie"] + "";
                    var type      = dataReader["player_type"] + "";
                    var team      = dataReader["player_team"] + "";
                    var lastWeek  = dataReader["player_last_wk"] + "";

                    int playerId;
                    int.TryParse(index, out playerId);

                    HockeyPlayer.PlayerSkill skillLevel;
                    if (level == "D")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_D;
                    else if (level == "C")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_C;
                    else if (level == "B")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_B;
                    else // (level == "A")
                        skillLevel = HockeyPlayer.PlayerSkill.Level_A;

                    bool canPlayGoalie = false;
                    if (goalie == "Y")
                        canPlayGoalie = true;

                    playerList.Add(new HockeyPlayer(playerId, lastName, firstName, skillLevel, position, canPlayGoalie, type[0], team, lastWeek));
                }

                CloseConnection();
            }

            return playerList;
        }

        // =====================================================
        //Count statement
        public int Count()
        // =====================================================
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

        // =====================================================
        // Parse Exceptions
        private void ParseException(MySqlException ex)
        // =====================================================
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
