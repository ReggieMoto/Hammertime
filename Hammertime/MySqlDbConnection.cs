// ==============================================================
//
// class MySqlDbConnection
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
using MySql.Data.MySqlClient;
using System.IO;

namespace Hammertime
{
    // =====================================================
    public class MySqlDbHockeyPlayer : HockeyPlayer
    // =====================================================
    {
        public int PlayerId { get; set; }

        public MySqlDbHockeyPlayer(
            int player_id,
            string player_last_name,
            string player_first_name,
            PlayerSkill player_level,
            string player_position,
            bool player_goalie,
            char player_type,
            string player_team,
            string player_last_wk,
            bool captain,
            bool alt_captain) : base(
                player_last_name,
                player_first_name,
                player_level,
                player_position,
                player_goalie,
                player_type,
                player_team,
                player_last_wk,
                captain,
                alt_captain)
        {
            PlayerId = player_id; // MySql specific
        }

        // ==============================================================
        public MySqlDbHockeyPlayer(MySqlDbHockeyPlayer player) : base(player)
        // ==============================================================
        {
            PlayerId = player.PlayerId;
        }

        // ==============================================================
        public MySqlDbHockeyPlayer(HockeyPlayer player) : base(player)
        // ==============================================================
        {
            PlayerId = 0;
        }

    }

    public sealed class MySqlDbConnection : DbConnection
    {
        // The MySqlDbConnection class is a handle on the MySql database.
        // This class is used to perform the database operations.
        // The actual connection to the MySQl database is the MySqlConnection var below.
        private static MySqlConnection _connection;
        private static bool _connected;

        private static string _server = "localhost";
        private static string _database = "mondaynighthockey";

        // =====================================================
        public static MySqlDbConnection getInstance(string uid = null, string password = null)
        // =====================================================
        {
            if (_dbConnection == null)
            {
                Console.WriteLine("Using the MySql database.");
                _dbConnection = new MySqlDbConnection(uid, password);
            }

            return (MySqlDbConnection)_dbConnection;
        }

        // =====================================================
        //Constructor
        private MySqlDbConnection(string uid, string password)
        // =====================================================
        {
            string _connectionString;
            List<string> credentials = Credentials.getCredentials();

            _connectionString = "SERVER=" + _server + ";" + "DATABASE=" + _database + ";" + "UID=" + credentials[0] + ";" + "PASSWORD=" + credentials[1] + ";";
            _connectionString += "charset=utf8;convertzerodatetime=true;";
            _connection = new MySqlConnection(_connectionString);

            try
            {
                _connected = OpenConnection();
                if (Connected())
                    CloseConnection();
            }
            catch (MySqlException ex)
            {
                ParseException(ex);
                _connected = false;
            }
        }

        // =====================================================
        public override bool Connected()
        // =====================================================
        {
            return _connected;
        }

        // =====================================================
        //open connection to database
        public override bool OpenConnection()
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
        public override bool CloseConnection()
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

        // =====================================================
        //Insert statement
        public override bool Insert(HockeyPlayer player)
        // =====================================================
        {
            bool insertStatus = false;
            MySqlDbHockeyPlayer hockeyPlayer = new MySqlDbHockeyPlayer(player);

            if (OpenConnection() == true)
            {
                string firstName = hockeyPlayer.FirstName;
                string lastName = hockeyPlayer.LastName;
                string position = hockeyPlayer.PlayerPos;
                string playerType = hockeyPlayer.PlayerType.ToString();

                string skillLevel;
                if (hockeyPlayer.Level == HockeyPlayer.PlayerSkill.Level_A)
                    skillLevel = "A";
                else if (hockeyPlayer.Level == HockeyPlayer.PlayerSkill.Level_B)
                    skillLevel = "B";
                else if (hockeyPlayer.Level == HockeyPlayer.PlayerSkill.Level_C)
                    skillLevel = "C";
                else //(hockeyPlayer.Level == HockeyPlayer.PlayerSkill.Level_D)
                    skillLevel = "D";

                string goalie;
                if (hockeyPlayer.Goalie == true)
                    goalie = "Y";
                else
                    goalie = "N";

                string mySqlQuery = $"insert into mondaynighthockey.players (player_last_name, player_first_name, player_level, player_position, player_goalie, player_type, player_team, player_last_wk) values (\"{lastName}\", \"{firstName}\", '{skillLevel[0]}', \"{position}\", '{goalie[0]}', \"{playerType}\", \"Unaffiliated\", \"Zed\")";
                Console.WriteLine(mySqlQuery);

                //Create Command
                MySqlCommand cmd = new MySqlCommand(mySqlQuery, _connection);
                // Execute the INSERT
                int affectedRows = cmd.ExecuteNonQuery();
                // Finished with the command
                CloseConnection();

                if (affectedRows == 1)
                    insertStatus = true;
            }

            return insertStatus;
        }

        // =====================================================
        //Update statement
        public override bool Update(HockeyPlayer player)
        // =====================================================
        {
            bool updateStatus = false;

            string playerName = player.FirstName + " " + player.LastName;
            MySqlDbHockeyPlayer hockeyPlayer = (MySqlDbHockeyPlayer)Read(playerName);

            if (OpenConnection() == true)
            {
                if (hockeyPlayer != null)
                {
                    //Create Command
                    string mySqlQuery = $"update mondaynighthockey.players set player_last_wk=\"{hockeyPlayer.PlayerLastWeek}\" where player_id={hockeyPlayer.PlayerId}";
                    MySqlCommand cmd = new MySqlCommand(mySqlQuery, _connection);
                    // Execute the UPDATE
                    int affectedRows = cmd.ExecuteNonQuery();

                    if (affectedRows == 1)
                        updateStatus = true;
                }

                // Finished with the command
                CloseConnection();
            }

            return updateStatus;
        }

        // =====================================================
        //Delete statement
        public override bool Delete(HockeyPlayer player)
        // =====================================================
        {
            bool updateStatus = false;

            string playerName = player.FirstName + " " + player.LastName;
            MySqlDbHockeyPlayer hockeyPlayer = (MySqlDbHockeyPlayer)Read(playerName);

            if (OpenConnection() == true)
            {
                if (hockeyPlayer != null)
                {
                    //Create Command
                    string mySqlQuery = $"delete from mondaynighthockey.players where player_id={hockeyPlayer.PlayerId}";
                    MySqlCommand cmd = new MySqlCommand(mySqlQuery, _connection);
                    // Execute the DELETE
                    int affectedRows = cmd.ExecuteNonQuery();

                    if (affectedRows == 1)
                        updateStatus = true;
                }

                // Finished with the command
                CloseConnection();
            }

            return updateStatus;
        }

        // =====================================================
        // Read statement
        public override HockeyPlayer Read(string playerName)
        {
            return SelectPlayer(playerName);
        }

        public override List<HockeyPlayer> Read()
        {
            return SelectAllPlayers();
        }

        // =====================================================

        // =====================================================
        // Select Statement
        public HockeyPlayer SelectPlayer(string playerName)    // Expect "first last"
        // =====================================================
        {
            MySqlDbHockeyPlayer player = null;
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
                    var captain = dataReader["captain"] + "";
                    var altCaptain = dataReader["alt_captain"] + "";

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

                    bool isCaptain = false, isAltCaptain = false;
                    if (captain == "Y")
                        isCaptain = true;
                    if (altCaptain == "Y")
                        isAltCaptain = true;

                    player = new MySqlDbHockeyPlayer(playerId, lastName, firstName, skillLevel, position, canPlayGoalie, type[0], team, lastWeek, isCaptain, isAltCaptain);
                }

                CloseConnection();
            }

            return player;
        }

        // =====================================================
        // Select Statement
        public List<HockeyPlayer> SelectAllPlayers()
        // =====================================================
        {
            List<HockeyPlayer> playerList = new List<HockeyPlayer>();

            if (OpenConnection() == true)
            {
                string query = "SELECT * FROM players ORDER BY player_level";

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
                    var captain = dataReader["captain"] + "";
                    var altCaptain = dataReader["alt_captain"] + "";

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

                    bool isCaptain = false, isAltCaptain = false;
                    if (captain == "Y")
                        isCaptain = true;
                    if (altCaptain == "Y")
                        isAltCaptain = true;


                    playerList.Add(new MySqlDbHockeyPlayer(playerId, lastName, firstName, skillLevel, position, canPlayGoalie, type[0], team, lastWeek, isCaptain, isAltCaptain));
                }

                CloseConnection();
            }

            return playerList;
        }

        // =====================================================
        //Count statement
        public override int Count()
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

        private static string filenameSubdir = @"\MySQL 5.7\Backups\";
        private static string fileName = "backup.sql";

        // =====================================================
        //Backup
        public override bool Backup()
        // =====================================================
        {
            bool backupSuccess = false;

            //Open Connection
            if (OpenConnection() == true)
            {
                string filenamePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                filenamePath += filenameSubdir;
                string fullyQualifiedFilename = filenamePath + fileName;

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
                        File.Copy(fullyQualifiedFilename, filenamePath + date + fileName);
                    }

                    //Create and execute Mysql Backup Command
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = _connection;
                    MySqlBackup backup = new MySqlBackup(cmd);
                    backup.ExportToFile(fullyQualifiedFilename);

                    backupSuccess = true;
                }
                else
                {
                    Console.WriteLine($"Directory does not exist: {filenamePath}");
                }

                //close Connection
                CloseConnection();
            }

            return backupSuccess;
        }

        // =====================================================
        //Restore
        public override bool Restore()
        // =====================================================
        {
            bool restoreSuccess = false;
            string filenamePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            filenamePath += filenameSubdir;
            string fullyQualifiedFilename = filenamePath + fileName;

            if (Directory.Exists(filenamePath))
            {
                if (File.Exists(fullyQualifiedFilename))
                {
                    //Open Connection
                    if (OpenConnection() == true)
                    {
                        //Create and execute Mysql Backup Command
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = _connection;
                        MySqlBackup backup = new MySqlBackup(cmd);
                        backup.ImportFromFile(fullyQualifiedFilename);

                        restoreSuccess = true;

                        //close Connection
                        CloseConnection();
                    }
                }
                else
                {
                    Console.WriteLine($"File does not exist: {fullyQualifiedFilename}");
                }
            }
            else
            {
                Console.WriteLine($"Directory does not exist: {filenamePath}");
            }

            return restoreSuccess;
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
