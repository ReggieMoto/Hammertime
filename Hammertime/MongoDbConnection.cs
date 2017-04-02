// ==============================================================
//
// class MongoDbConnection
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

namespace Hammertime
{
    // =====================================================
    public class MongoDbException : System.Exception
    // =====================================================
    {
        string _message;

        // ==============================================================
        public MongoDbException(string message) : base(message)
        // ==============================================================
        {
            _message = message;
        }
    }

    class MongoDbConnection : DbConnection
    {
        private static MongoDbConnection _connection;
        private static string _server, _database, _uid;
        private static string _connectionString;
        private static bool _connected;

        // =====================================================
        public static MongoDbConnection getInstance(string server = null, string database = null, string uid = null, string password = null)
        // =====================================================
        {
            if (_dbConnection == null)
            {
                //Console.WriteLine("Setting up DbConnection for the first time.");

                _server = server;
                _database = database;
                _uid = uid;

                _connectionString = "SERVER=" + server + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";" + "DATABASE=" + database + ";";
                _connectionString += "charset=utf8;convertzerodatetime=true;";
                _dbConnection = new MongoDbConnection();
            }
            // Console.WriteLine("Exiting DB connection constructor.");
            return (MongoDbConnection)_dbConnection;
        }

        // =====================================================
        //Constructor
        private MongoDbConnection()
        // =====================================================
        {
        }

        // =====================================================
        public override bool Connected()
        // =====================================================
        {
            return _connected;
        }

        // =====================================================
        public override bool OpenConnection()
        // =====================================================
        {
            throw new NotImplementedException();
        }

        // =====================================================
        public override bool CloseConnection()
        // =====================================================
        {
            throw new NotImplementedException();
        }

        // =====================================================
        public override bool Insert(string cmd)
        // =====================================================
        {
            throw new NotImplementedException();
        }

        // =====================================================
        public override HockeyPlayer Read(string cmd)
        // =====================================================
        {
            throw new NotImplementedException();
        }

        // =====================================================
        public override ArrayList Read()
        // =====================================================
        {
            throw new NotImplementedException();
        }

        // =====================================================
        public override bool Update(string cmd)
        // =====================================================
        {
            throw new NotImplementedException();
        }

        // =====================================================
        public override bool Delete(string cmd)
        // =====================================================
        {
            throw new NotImplementedException();
        }

        // =====================================================
        public override int Count()
        // =====================================================
        {
            throw new NotImplementedException();
        }

        // =====================================================
        public override bool Backup()
        // =====================================================
        {
            throw new NotImplementedException();
        }

        // =====================================================
        public override bool Restore()
        // =====================================================
        {
            throw new NotImplementedException();
        }
    }
}
