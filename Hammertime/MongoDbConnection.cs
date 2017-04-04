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
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace Hammertime
{
    /*
    BsonDocument document = new BsonDocument
    {
        { "address" , new BsonDocument
            {
                { "street", "2 Avenue" },
                { "zipcode", "10075" },
                { "building", "1480" },
                { "coord", new BsonArray { 73.9557413, 40.7720266 } }
            }
        }
    };
    */

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

    sealed class MongoDbConnection : DbConnection
    {
        // The MongpDbConnection class is a handle on the MongoDB database.
        // This class is used to perform the database operations.
        // The actual connection to the MongoDB database is the MongoClient var below.
        private static IMongoClient _mongoClient = null;
        private static IMongoDatabase _mongoDb = null;
        private static IMongoCollection<BsonDocument> _mongoCollection = null;

        // mongodb://[username:password@]host1[:port1]
        private static string _server = @"mongodb://localhost:27017";
        private static string _database = "Hammertime";
        private static string _collection = "mondaynighthockey";

        private static bool _connected;

        // =====================================================
        public static MongoDbConnection getInstance(string uid = null, string password = null)
        // =====================================================
        {
            if (_dbConnection == null)
            {
                _dbConnection = new MongoDbConnection();
            }

            return (MongoDbConnection)_dbConnection;
        }

        // =====================================================
        //Constructor
        private MongoDbConnection()
        // =====================================================
        {
            string connectionString = _server + "/" + _database;
            BsonClassMap.RegisterClassMap<HockeyPlayer>();

            try
            {
                _mongoClient = new MongoClient(connectionString);

                if (_mongoClient != null)
                {
                    Console.WriteLine("MongoDb client initialized.");
                    _mongoDb = _mongoClient.GetDatabase(_database);

                    if (_mongoDb != null)
                    {
                        Console.WriteLine($"MongoDb database \"{_database}\" has been retrieved.");
                        _mongoCollection = _mongoDb.GetCollection<BsonDocument>(_collection);
                        
                        if (_mongoCollection != null)
                        {
                            Console.WriteLine($"MongoDb collection \"{_collection}\" has been retrieved.");
                            _connected = true;
                        }
                        else
                            Console.WriteLine($"Failed to retrieve MongoDb collection \"{_collection}\".");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to retrieve MongoDb database \"{_database}\".");
                    }

               }
                else
                {
                    Console.WriteLine("Failed to initialize the MongoDb Client.");
                }
            }
            catch (MongoConfigurationException ex)
            {
                Console.WriteLine();
                Console.WriteLine($"{ex.Message}");
                Console.WriteLine();
            }
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
        public override bool Insert(HockeyPlayer player)
        // =====================================================
        {
            bool insertSuccess = false;

            if (Connected())
            {
                var playerDoc = new BsonDocument
                {
                    { "player_last_name", player.LastName },
                    { "player_first_name", player.FirstName },
                    { "player_level", player.Level },
                    { "player_position", player.PlayerPos },
                    { "player_goalie", player.Goalie },
                    { "player_type", player.PlayerType },
                    { "player_team", "Unaffiliated" },
                    { "player_last_wk", "Zed" }
                };

                _mongoCollection.InsertOne(playerDoc);
                insertSuccess = true;
            }

            return insertSuccess;
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
