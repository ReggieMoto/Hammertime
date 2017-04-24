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
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Hammertime
{
    // =====================================================
    [BsonIgnoreExtraElements]
    public class MongoDbHockeyPlayer : HockeyPlayer
    // =====================================================
    {
        public ObjectId _id { get; set; }

        public MongoDbHockeyPlayer(
            ObjectId player_id,
            string classType,
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
            _id = player_id;    // MongoDb specific
        }

        // ==============================================================
        public MongoDbHockeyPlayer(MongoDbHockeyPlayer player) : base(player)
        // ==============================================================
        {
            _id = player._id;
        }

        // ==============================================================
        public MongoDbHockeyPlayer(HockeyPlayer player) : base(player)
        // ==============================================================
        {
            _id = ObjectId.Empty;
        }

        // ==============================================================
        public MongoDbHockeyPlayer() : base()
        // ==============================================================
        {
            _id = ObjectId.Empty;
        }
    }

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
        private static string _server = @"mongodb://";
        private static string _host = "localhost:27017";
        private static string _database = "Hammertime";
        private static string _collection = "mondaynighthockey";

        private static bool _connected;

        // =====================================================
        public static MongoDbConnection getInstance()
        // =====================================================
        {
            if (_dbConnection == null)
            {
                Console.WriteLine("Using the MongoDb database.");
                _dbConnection = new MongoDbConnection();
            }

            return (MongoDbConnection)_dbConnection;
        }

        // =====================================================
        // Constructor
        // Auth string connectionString = _server + credentials[0] + ":" + credentials[1] + "@" + _host + "/" + _database;
        private MongoDbConnection()
        // =====================================================
        {
            List<string> credentials = Credentials.getCredentials();
            string connectionString = _server + _host + "/" + _database;

            BsonClassMap.RegisterClassMap<MongoDbHockeyPlayer>(cm => {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            try
            {
                _mongoClient = new MongoClient(connectionString);

                if (_mongoClient != null)
                {
                    //Console.WriteLine("MongoDb client initialized.");
                    _mongoDb = _mongoClient.GetDatabase(_database);

                    if (_mongoDb != null)
                    {
                        _mongoCollection = _mongoDb.GetCollection<BsonDocument>(_collection);
                        
                        if (_mongoCollection != null)
                        {
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
        // Read a player out of the database
        // Expect "first last"
        public override HockeyPlayer Read(string playerName)
        // =====================================================
        {
            MongoDbHockeyPlayer hockeyPlayer = null;

            if (Connected())
            {
                // Parse the string into last_name/first_name
                string[] name = playerName.Split(' ');

                var builder = Builders<BsonDocument>.Filter;
                var filterFN = builder.Eq("FirstName", name[0]);
                var filterLN = Builders<BsonDocument>.Filter.Eq("LastName", name[1]);
                if (name.Length == 3 && name[1] == "St.")
                    filterLN = Builders<BsonDocument>.Filter.Eq("LastName", name[1] + " " + name[2]);
                var filter = filterFN & filterLN;

                var player = _mongoCollection.Find(filter).FirstOrDefault();
                if (player != null)
                    hockeyPlayer = BsonSerializer.Deserialize<MongoDbHockeyPlayer>(player);
            }

            return hockeyPlayer;
        }

        // =====================================================
        public override List<HockeyPlayer> Read()
        // =====================================================
        {
            List<HockeyPlayer> hockeyPlayers = new List<HockeyPlayer>();
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("_t", "MongoDbHockeyPlayer");
            var playerDocs = _mongoCollection.Find(filter).ToList();

            foreach (BsonDocument playerDoc in playerDocs)
            {
                var hockeyPlayer = new MongoDbHockeyPlayer();
                hockeyPlayer = BsonSerializer.Deserialize<MongoDbHockeyPlayer>(playerDoc);
                hockeyPlayers.Add(hockeyPlayer);
            }

            return hockeyPlayers;
        }

        // =====================================================
        public override bool Insert(HockeyPlayer hockeyPlayer)
        // =====================================================
        {
            bool insertSuccess = false;
            if (Connected())
            {
                var builder = Builders<BsonDocument>.Filter;
                var filterFN = builder.Eq("FirstName", hockeyPlayer.FirstName);
                var filterLN = builder.Eq("LastName", hockeyPlayer.LastName);
                var filter = filterFN & filterLN;

                // Check to see if the player is already in the database
                if (_mongoCollection.Find(filter).FirstOrDefault() == null)
                {
                    // If the player is not then insert him
                    var player = new MongoDbHockeyPlayer(hockeyPlayer);
                    var playerDoc = new BsonDocument();
                    var bsonWriter = new BsonDocumentWriter(playerDoc);
                    BsonSerializer.Serialize(bsonWriter, player);   // Serialize MongoDbHockeyPlayer to BsonDocument

                    _mongoCollection.InsertOne(playerDoc); // Insert the BsonDocument into the database
                    insertSuccess = true;
                }
                else
                    Console.WriteLine("Player is already in the database.");
            }

            return insertSuccess;
        }

        // =====================================================
        public override bool Update(HockeyPlayer hockeyPlayer)
        // =====================================================
        {
            bool updateSuccess = false;

            if (Connected())
            {
                // First retrieve the player's current document in the database
                // Build a query filter. This filter will be used to both find and replace
                var builder = Builders<BsonDocument>.Filter;
                var filterFN = builder.Eq("FirstName", hockeyPlayer.FirstName);
                var filterLN = builder.Eq("LastName", hockeyPlayer.LastName);
                var filter = filterFN & filterLN;

                // Find the player in the database
                var currentPlayerDoc = _mongoCollection.Find(filter).FirstOrDefault();
                var currentPlayer = new MongoDbHockeyPlayer();
                currentPlayer = BsonSerializer.Deserialize<MongoDbHockeyPlayer>(currentPlayerDoc);

                // CurrentPlayer is the player as represented in the existing BsonDocument
                // Need to transfer this player's _id to the new player's _id

                var newPlayer = new MongoDbHockeyPlayer(hockeyPlayer);
                newPlayer._id = currentPlayer._id;

                var newPlayerDoc = new BsonDocument();
                var bsonWriter = new BsonDocumentWriter(newPlayerDoc);
                BsonSerializer.Serialize(bsonWriter, newPlayer);   // Serialize the new MongoDbHockeyPlayer to BsonDocument

                var options = new UpdateOptions();
                options.IsUpsert = true;
                try
                {
                    var result = _mongoCollection.ReplaceOne(filter, newPlayerDoc, options); // Replace the existing BsonDocument with the new one

                    updateSuccess = true;
                }
                catch (BsonSerializationException ex)
                {
                    Console.WriteLine();
                    Console.WriteLine($"MongoDbConnection Update failed with error: {ex.Message}");
                    Console.WriteLine();
                }
            }

            return updateSuccess;
        }

        // =====================================================
        public override bool Delete(HockeyPlayer player)
        // =====================================================
        {
            bool removeSuccess = false;

            var builder = Builders<BsonDocument>.Filter;
            var filterFN = builder.Eq("FirstName", player.FirstName);
            var filterLN = builder.Eq("LastName", player.LastName);
            var filter = filterFN & filterLN;

            var result = _mongoCollection.DeleteOne(filter);

            if (result.DeletedCount == 1)
                removeSuccess = true;

            return removeSuccess;
        }

        // =====================================================
        public override int Count()
        // =====================================================
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("_t", "MongoDbHockeyPlayer");
            return (int)_mongoCollection.Find(filter).Count();
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
