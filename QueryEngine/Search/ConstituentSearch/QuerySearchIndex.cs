using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Search.Security;
using Search.Utils;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace Search.ConstituentSearch
{
    public class QuerySearchIndex
    {
        private Guid _appUserId;
        private bool _isAdmin;
        private string _searchTerm;

        private Dictionary<string, ConstitEntity> _securityHashMap;

        private List<string> _fullHashList; 
        private List<string> _securityCheckedHashList;

        public List<ConstitEntity> ViewableResultsList { get; private set; }
        public string JsonResults { get; private set; }
        public long QueryTimeMilliseconds { get; private set; }



        public QuerySearchIndex(Guid appUserId, string searchTerm)
        {
            _appUserId = appUserId;
            _securityHashMap = new Dictionary<string, ConstitEntity>();

            _fullHashList = new List<string>();
            _securityCheckedHashList = new List<string>();

            ViewableResultsList = new List<ConstitEntity>();

            _searchTerm = searchTerm;
        }

        public QuerySearchIndex()
        {
        }


        public void Execute()
        {
            var sw = Stopwatch.StartNew();

            ISolrOperations<ConstitEntity> solr = null;
            try
            {
                Startup.Init<ConstitEntity>(Properties.Settings.Default.solrUrl);
            }
            catch (Exception ex)
            {
                //todo nothing eat this
            }
            finally
            {
                solr = ServiceLocator.Current.GetInstance<ISolrOperations<ConstitEntity>>();
            }

            var terms = _searchTerm.Split(' ');
            var sb = new StringBuilder();
            for (var i = 0; i < terms.Length; i++)
            {
                sb.Append(string.Format("freeText:{0}", terms[i]));
                if (i < terms.Length -1)
                {
                    sb.Append(" AND ");
                }
            }

            var entity = solr.Query(new SolrQuery(sb.ToString()), new QueryOptions { OrderBy = new [] {new SortOrder("city", Order.ASC),SortOrder.Parse("city asc") }, Fields = new[] { "constituentId", "keyName", "firstName", "addressBlock", "city", "stateAbrv", "countryAbrv" }  });

            foreach (var doc in entity)
            {
                var hash = ConstituentSecurity.GenSecurityHash(new Guid(doc.ConstituentId),_appUserId);
                _securityHashMap[hash] = doc;
                _fullHashList.Add(hash);
            }
            ValidateSecurity();

            foreach (var hash in _securityCheckedHashList)
            {
                ViewableResultsList.Add(_securityHashMap[hash]);
            }

            JsonResults = JsonConvert.SerializeObject(ViewableResultsList);
            sw.Stop();
            QueryTimeMilliseconds = sw.ElapsedMilliseconds;

            Console.WriteLine("Total query results:{0} - Total Query Time:{1}(ms)", _securityCheckedHashList.Count, sw.ElapsedMilliseconds);
        }



        private void ValidateSecurity()
        {
            var dbConn = new DBConnection
            {
                DataBaseName = Properties.Settings.Default.MongoDatabase,
                ServerName = Properties.Settings.Default.MongoServer
            };


            var server = MongoServer.Create(dbConn.GetMongoDBConnectionString());
            var database = server.GetDatabase(dbConn.DataBaseName);

            var collection = database.GetCollection("ConstituentSecurity");
            var query = MongoDB.Driver.Builders.Query.In("_id", new BsonArray(_fullHashList));

            foreach (var result in collection.Find(query))
            {
                _securityCheckedHashList.Add(result.GetElement("_id").Value.AsString);
            }
        }
    }
}
