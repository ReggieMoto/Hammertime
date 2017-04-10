// ==============================================================
//
// class MigrationMySql2MongoDb
//
// Migrate the MySQL database records to the MongoDb database
// This should only be required once.
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
using System.Threading.Tasks;

namespace Hammertime
{
    public class MigrationMySql2MongoDbException : System.Exception
    {
        string _message;

        // ==============================================================
        public MigrationMySql2MongoDbException(string message) : base(message)
        // ==============================================================
        {
            _message = message;
        }
    }

    public class MigrationMySql2MongoDb
    {
        private static MigrationMySql2MongoDb MigrationObj { get; set; }
        private static MongoDbConnection MongoConnection { get; set; }
        private static MySqlDbConnection MySqlConnection { get; set; }

        public static MigrationMySql2MongoDb getInstance()
        {
            if (MigrationObj == null)
                MigrationObj = new MigrationMySql2MongoDb();

            return MigrationObj;
        }

        private MigrationMySql2MongoDb()
        {
        }

        public bool PerformMigration()
        {
            bool migrationSuccess = false;
            MySqlConnection = MySqlDbConnection.getInstance();

            List<HockeyPlayer> hockeyPlayers = new List<HockeyPlayer>();
            hockeyPlayers = MySqlConnection.Read();
            Console.WriteLine($"Read {hockeyPlayers.Count} records out of the MySQL database.");

            MySqlConnection.ResetConnection();
            MongoConnection = MongoDbConnection.getInstance();

            int counter = 0;
            foreach (var player in hockeyPlayers)
            {
                bool insertSuccess = MongoConnection.Insert(player);
                if (insertSuccess == false)
                    throw new MigrationMySql2MongoDbException("Failed to insert player into MongoDB.");
                else
                    counter++;
            }

            Console.WriteLine($"Wrote {counter} records to the Monday database.");

            return migrationSuccess;
        }
    }
}
