 using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MongoDB.Bson;
using MongoDB.Driver;
using Search.DataSynchronization.Loaders;
using Search.Utils;

namespace Search.DataSynchronization
{
    public class MongoDbLoader :Utils.ITask
    {
        private MainContext _context;
        private List<BsonDocument> _docsToLoad;
        private List<Dictionary<string, object>> _dataToload;
        private string _collectionName;
        private SyncCache _cashMap;

        public MongoDbLoader(List<Dictionary<string, object>> dataToLoad, MainContext context, string collectionName)
        {
            _context = context;
            _dataToload = dataToLoad;
            _docsToLoad = new List<BsonDocument>();
            _collectionName = collectionName;
        }
            
        public void Execute()
        {
            var sw = Stopwatch.StartNew();

            LoadBsonDocs();

            var server = MongoServer.Create(_context.MongoDBConnection.GetMongoDBConnectionString());
            var database = server.GetDatabase(_context.MongoDBConnection.DataBaseName);
            var collection = database.GetCollection(_collectionName);
            if (_context.SyncOnly)
            {
                foreach (var doc in _docsToLoad)
                {
                    collection.Save(doc);

                    var cacheCollector = new CacheCollector(_context);
                    cacheCollector.RemoveCacheRecord(_cashMap.Idx);
                }
            }
            else
            {
                collection.InsertBatch(_docsToLoad);
            }

            sw.Stop();

            Console.WriteLine("{3} - Inserting {0} documents into the {1} collection. Total Time:{2}(ms)", _dataToload.Count, _collectionName, sw.ElapsedMilliseconds, DateTime.Now);

        }

        private void LoadBsonDocs()
        {
            foreach (var doc in _dataToload)
            {
                _docsToLoad.Add(doc.ToBsonDocument());
            }
        }  
      
    }
}
