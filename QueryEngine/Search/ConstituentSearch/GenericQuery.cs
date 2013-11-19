using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MySearch.QueryProcessor;

namespace Search.ConstituentSearch
{
    public class GenericQuery
    {
        private QueryProcessor _queryProcessor;
        private FinalQueryResult _queryResults; 
     
        public GenericQuery(QueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
            _queryResults = new FinalQueryResult();
        }

        private void RunQuery()
        {
            var sw = Stopwatch.StartNew();

            var resultCount = 0;

            var server = MongoServer.Create(_queryProcessor.MongoDBConnection.GetMongoDBConnectionString());
            var database = server.GetDatabase(_queryProcessor.MongoDBConnection.DataBaseName);
            var collection = database.GetCollection("Constituent");
            var fields = (from info in _queryProcessor.Selections.ToArray() select info.Key).ToArray();
            var queryToRun = MongoDB.Driver.Builders.Query.And(_queryProcessor.Filters);
          
            foreach (var genericQueryResult in collection.Find(queryToRun).SetFields(fields))
            {
                _queryResults.JsonResults.Add(genericQueryResult.ToJson());
                //var id = genericQueryResult.GetElement("_id");
               

                //_queryResults.ResultMap.Add(id.Value.AsGuid, genericQueryResult);
                resultCount++;  
            }
            
            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Total results:{0} - Total Execution time {1}ms.", resultCount, sw.ElapsedMilliseconds);
            Console.ForegroundColor = ConsoleColor.Gray;

        }

        public FinalQueryResult Execute()
        {

            RunQuery();
            return _queryResults;

        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}
