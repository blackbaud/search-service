using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using Search.ConstituentSearch;
using Search.DataSynchronization.Loaders;
using Search.Security;
using Search.Utils;

namespace Search.DataSynchronization.SyncOperations
{
    public class MongoDbUpdater :ITask
    {
        private const string MongoDBCollectionName = "ConstituentSecurity";

        public MongoDbUpdater(List<SyncCache> caches, MainContext context)
        {
            _caches = caches;
            _context = context;
        }

        private List<SyncCache> _caches;
        private readonly MainContext _context;

        public void Execute()
        {
            var sw = Stopwatch.StartNew();

           
            var server = MongoServer.Create(_context.MongoDBConnection.GetMongoDBConnectionString());
            var database = server.GetDatabase(_context.MongoDBConnection.DataBaseName);
            var collection = database.GetCollection(MongoDBCollectionName);

            var docsToLoad = GetData();

            foreach (var cache in _caches)
            {
                collection.Remove(MongoDB.Driver.Builders.Query.EQ("AppUserID", cache.Id));
            }

            collection.InsertBatch(docsToLoad);

            RemoveCacheRecords();
            
            sw.Stop();

            Console.WriteLine("{3} - Inserting {0} documents into the {1} collection. Total Time:{2}(ms)", docsToLoad.Count, MongoDBCollectionName, sw.ElapsedMilliseconds, DateTime.Now);
        }

        private List<BsonDocument> GetData()
        {
            var docsToLoad = new List<BsonDocument>();

            using (var sqlconn = new SqlConnection(_context.DBConnection.GetConnectionStringTrusted()))
            {
                sqlconn.Open();
                using (var cmd = new SqlCommand(GenSyncSQL(GenIds()), sqlconn))
                {
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var dataToLoad = EvalData.ReaderToDictionary(reader);

                        dataToLoad["_id"] = ConstituentSecurity.GenSecurityHash(EvalData.EvalSqlDataReaderGuid(reader, "ConstituentID"), EvalData.EvalSqlDataReaderGuid(reader, "AppUserID"));
                        docsToLoad.Add(dataToLoad.ToBsonDocument());
                    }
                }
            }
            return docsToLoad;
        }

        private static string GenSyncSQL(string idInClause)
        {
            return string.Format(
                    @"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
                    SELECT DISTINCT
                        NULL AS _id,
	                    t5.ConstituentID,
	                    t1.ID AS AppUserID	
                    FROM 
	                    dbo.AppUser t1
	                    INNER JOIN dbo.SYSTEMROLEAPPUSER t2 ON t1.ID = t2.AppUSerID
	                    INNER JOIN dbo.SYSTEMROLEAPPUSERCONSTITUENTSECURITY t3 ON t2.ID = t3.SystemRoleAppUserID
	                    INNER JOIN dbo.CONSTIT_SECURITY_ATTRIBUTE t4 ON t3.ConstituentSecurityAttributeId = t4.Id
	                    INNER JOIN dbo.CONSTIT_SECURITY_ATTRIBUTE_ASSIGNMENT t5 ON t4.ID = t5.Constit_Security_AttributeID
                        WHERE
                            t1.ID IN ({0})
                    ORDER BY 1, 2", idInClause);
        }

        private string GenIds()
        {
            var ids = new List<string>();

            foreach (var cache in _caches)
            {
                ids.Add(string.Format("'{0}'", cache.Id));
            }

            return string.Join(",", ids);
        }

        private void RemoveCacheRecords()
        {
            var cacheCollector = new CacheCollector(_context);

            foreach (var cache in _caches)
            {
                cacheCollector.RemoveCacheRecord(cache.Idx);
            }
        }
    }
}
