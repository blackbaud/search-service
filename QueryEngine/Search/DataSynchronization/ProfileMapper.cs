using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Search.DataSynchronization
{
    public class ProfileMapper
    {
        private MainContext _context;
        //private Dictionary<Guid, List<Guid>> _profileMap;
        private readonly Dictionary<string, Dictionary<Guid, Dictionary<string, List<Guid>>>> _profileMap;
        private Dictionary<Guid, Dictionary<string, object>> _baseMap;
        private List<BsonDocument> _docsToLoad; 
        private List<string> _sourceCollections;


        private Dictionary<string, object> _objects; 

        public ProfileMapper(MainContext context)
        {
            _context = context;
            _sourceCollections = new List<string> { "Address", "FinancialTransaction" };
            //_profileMap = new Dictionary<Guid, List<Guid>>();
            _profileMap = new Dictionary<string, Dictionary<Guid, Dictionary<string, List<Guid>>>>();
            _docsToLoad = new List<BsonDocument>();

            _baseMap = new Dictionary<Guid, Dictionary<string, object>>();
        }

        public void Execute()
        {
            var sw = Stopwatch.StartNew();

            SeedProfileMapper();

            foreach (var sourceCollection in _sourceCollections)
            {
                BuildProfileMap(sourceCollection);
            }

            LoadProfileMap();

            sw.Stop();

            Console.WriteLine("Profile Map created successfully. Total Time:{0}(ms).", sw.ElapsedMilliseconds);
        }

        private void LoadProfileMap()
        {
            var server = MongoServer.Create(_context.MongoDBConnection.GetMongoDBConnectionString());
            var database = server.GetDatabase(_context.MongoDBConnection.DataBaseName);
            var collection = database.GetCollection("ProfileMap");

            foreach (var constit in _baseMap)
            {
                collection.Insert(new BsonDocument(constit.Value));
            }
        }

        private void BuildProfileMap(string collectionName)
        {
            var sw = Stopwatch.StartNew();

            var server = MongoServer.Create(_context.MongoDBConnection.GetMongoDBConnectionString());
            var database = server.GetDatabase(_context.MongoDBConnection.DataBaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);

            var collectionResults = collection.FindAll().SetFields("_id", "CONSTITUENTID").ToList();

            var index = 1;

            foreach (var document in collectionResults)
            {

                try
                {
                    var constitId = (Guid) document["CONSTITUENTID"];
                    var filterId = (Guid) document["_id"];

                    if (!_baseMap[constitId].ContainsKey(collectionName))
                    {
                        _baseMap[constitId][collectionName] = new List<Guid> {filterId};
                    }
                    else
                    {
                        var tempList = _baseMap[constitId][collectionName] as List<Guid>;
                        tempList.Add(filterId);
                    }
                    index++;
                }
                catch (Exception exception)
                {
                    //todo add logging
                    //Console.WriteLine(exception.StackTrace);
                }

            }
            sw.Stop();

            Console.WriteLine("{0} collection added to the Profile Map. Total Results:{1}. Total Time: {2}(ms).", collectionName, index, sw.ElapsedMilliseconds);
        }

        private void SeedProfileMapper()
        {
            var sw = Stopwatch.StartNew();

            var server = MongoServer.Create(_context.MongoDBConnection.GetMongoDBConnectionString());
            var database = server.GetDatabase(_context.MongoDBConnection.DataBaseName);
            var collection = database.GetCollection<BsonDocument>("Constituent");

            var collectionResults = collection.FindAll().SetFields("_id").ToList();
            var index = 1;
            foreach (var doc in collectionResults)
            {
                var consitId = (Guid) doc["_id"];
                if (!_baseMap.ContainsKey(consitId))
                {
                    _baseMap[consitId] = new Dictionary<string, object>();
                    _baseMap[consitId]["_id"] = consitId;
                }
            }
            sw.Stop();

            Console.WriteLine("Profile Map seeded successfully. Total Results:{0}. Total Time: {1}(ms).", index, sw.ElapsedMilliseconds);
        }
    }
}
