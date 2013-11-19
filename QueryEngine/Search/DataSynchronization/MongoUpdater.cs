using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Search.DataSynchronization
{
    public class MongoUpdater :IDataUpdater
    {
        private MainContext _context;
       
        public MongoUpdater(MainContext context)
        {
            _context = context;
        }

        public void Update()
        {
           
            //var server = MongoServer.Create(_context.MongoDBConnection.GetMongoDBConnectionString());
            //var database = server.GetDatabase(_context.MongoDBConnection.DataBaseName);

            foreach (var desc in _context.CacheMap)
            {
                //foreach (var id in desc.Value)
                //{
                //    Upsert(GetData(desc.Key, id), id);
                //}
            }
        }


        private Dictionary<string, object> GetData(string sourceTable, Guid id)
        {
            using (var sqlConn = new SqlConnection(_context.DBConnection.GetConnectionStringTrusted()))
            {
                sqlConn.Open();
                var cmd = new SqlCommand(string.Format("SELECT * FROM {0} WHERE ID = @id", sourceTable),sqlConn);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();

                return ConvertReader(reader);
            }
        }

        private static Dictionary<string, object> ConvertReader(SqlDataReader reader)
        {
            var objectMap = new Dictionary<string, object>();
            while (reader.Read())
            {
                var fieldCount = reader.FieldCount;

                for (var i = 0; i < fieldCount; i++)
                {
                    objectMap[ConvertColumnName(reader.GetName(i))] = reader[i];
                }
            }
            return objectMap;
        }

        private void Upsert(Dictionary<string, object> objectMap, Guid id)
        {

            var server = MongoServer.Create(_context.MongoDBConnection.GetMongoDBConnectionString());
            var database = server.GetDatabase(_context.MongoDBConnection.DataBaseName);
            var collection = database.GetCollection("ConstitProfile");

            var newDoc = objectMap.ToBsonDocument();
            Console.WriteLine(newDoc);

            collection.Save(newDoc.ToBsonDocument());

            var ids = new Guid[] {id};

            var query = Query.In("_id", new BsonArray(ids));

            foreach (var doc in collection.Find(query))
            {
                Console.WriteLine(doc);
                Console.Read();
            }
        }

        private static string ConvertColumnName(string columnName)
        {
            return columnName == "ID" ? "_id" : columnName;
        }
    }
}
