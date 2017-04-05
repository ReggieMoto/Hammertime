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
using MongoDB.Bson;
using MongoDB.Bson.IO;
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
    public class MongoDbHockeyPlayer : HockeyPlayer
    // =====================================================
    {
        public ObjectId _id {get; set; }

        public string player_last_name
        {
            get { return LastName; }
            set { LastName = player_last_name; }
        }

        public string player_first_name
        {
            get { return FirstName; }
            set { FirstName = player_first_name; }
        }

        public PlayerSkill player_level
        {
            get { return Level; }
            set { Level = player_level; }
        }

        public string player_position     // Player's normal position
        {
            get { return PlayerPos; }
            set { PlayerPos = player_position; }
        }

        public bool player_goalie         // Also can play goalie
        {
            get { return Goalie; }
            set { Goalie = player_goalie; }
        }

        public char player_type           // Full time, Sub
        {
            get { return PlayerType; }
            set { PlayerType = player_type; }
        }

        public string player_team         // Ben, Barry, Unaffiliated
        {
            get { return PlayerTeam; }
            set { PlayerTeam = player_team; }
        }

        public string player_last_wk      // White, Black, Zed (Didn't play)
        {
            get { return PlayerLastWeek; }
            set { PlayerLastWeek = player_last_wk; }
        }

        public MongoDbHockeyPlayer(
            ObjectId player_id,
            string player_last_name,
            string player_first_name,
            PlayerSkill player_level,
            string player_position,
            bool player_goalie,
            char player_type,
            string player_team,
            string player_last_wk) : base (
                player_last_name,
                player_first_name,
                player_level,
                player_position,
                player_goalie,
                player_type,
                player_team,
                player_last_wk)
        {
            _id = player_id; // MongoDb specific
        }

        // ==============================================================
        public MongoDbHockeyPlayer(MongoDbHockeyPlayer player) : base (player)
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
            BsonClassMap.RegisterClassMap<MongoDbHockeyPlayer>();

            try
            {
                _mongoClient = new MongoClient(connectionString);

                if (_mongoClient != null)
                {
                    //Console.WriteLine("MongoDb client initialized.");
                    _mongoDb = _mongoClient.GetDatabase(_database);

                    if (_mongoDb != null)
                    {
                        //Console.WriteLine($"MongoDb database \"{_database}\" has been retrieved.");
                        _mongoCollection = _mongoDb.GetCollection<BsonDocument>(_collection);
                        
                        if (_mongoCollection != null)
                        {
                            //Console.WriteLine($"MongoDb collection \"{_collection}\" has been retrieved.");
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
        public override bool Insert(HockeyPlayer hockeyPlayer)
        // =====================================================
        {
            bool insertSuccess = false;

            if (Connected())
            {
                MongoDbHockeyPlayer player = new MongoDbHockeyPlayer(hockeyPlayer);

                var playerDoc = new BsonDocument();
                var bsonWriter = new BsonDocumentWriter(playerDoc);
                BsonSerializer.Serialize(bsonWriter, player);   // Serialize MongoDbHockeyPlayer to BsonDocument

                _mongoCollection.InsertOne(playerDoc); // Insert the BsonDocument into the database

                insertSuccess = true;
            }

            return insertSuccess;
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

                var filter = Builders<BsonDocument>.Filter.Eq("player_last_name", name[1]);
                if (name.Length == 3 && name[1] == "St.")
                    filter = Builders<BsonDocument>.Filter.Eq("player_last_name", name[1] + " " + name[2]);


                var results = _mongoCollection.Find(filter).ToList();

                foreach (BsonDocument player in results)
                {
                    hockeyPlayer = BsonSerializer.Deserialize<MongoDbHockeyPlayer>(player);

                    if (hockeyPlayer.FirstName == name[0])
                        break;
                }
            }

            return hockeyPlayer;
        }

        // =====================================================
        public override ArrayList Read()
        // =====================================================
        {
            throw new NotImplementedException();
        }

        // =====================================================
        public override bool Update(HockeyPlayer player)
        // =====================================================
        {
            throw new NotImplementedException();
        }

        // =====================================================
        public override bool Delete(HockeyPlayer player)
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
