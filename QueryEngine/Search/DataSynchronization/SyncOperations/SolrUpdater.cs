using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Search.ConstituentSearch;
using Search.DataSynchronization.Loaders;
using Search.Utils;
using SolrNet;


namespace Search.DataSynchronization
{
    public class SolrUpdater :ITask
    {

        public SolrUpdater(List<SyncCache> caches, MainContext context)
        {
            _caches = caches;
            _context = context;
        }

        private List<SyncCache> _caches;
        private readonly MainContext _context;

        public void Execute()
        {
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
            foreach (var entity in GetData())
            {
                solr.Add(entity);
            }
            solr.Commit();
            RemoveCacheRecords();
        }

        private List<ConstitEntity> GetData()
        {
            var consitEntiies = new List<ConstitEntity>();

            using (var sqlconn = new SqlConnection(_context.DBConnection.GetConnectionStringTrusted()))
            {
                sqlconn.Open();
                using (var cmd = new SqlCommand(GenSyncSQL(GenIds()), sqlconn))
                {
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        //var entity = ConstituentLoaderV2.BuildConstitEntity(reader);
                        ////entity.SyncCacheIndexId = _syncCaches[entity.ConstituentId].Idx;

                        //consitEntiies.Add(entity);
                    }
                }
            }
            return consitEntiies;
        }

        private static string GenSyncSQL(string idInClause)
        {
            return string.Format(
                    @"SELECT --TOP 1000
	                        t1.ID AS ConstitId,
                            t2.ID AS AddressId,
	                        t1.KeyName,
	                        t1.FirstName,
	                        t1.Age,
	                        t2.AddressBlock,
	                        t2.CIty,
	                        t3.Abbreviation AS StateAbrv,
	                        t4.Abbreviation AS CountryAbrv
                        FROM
	                        dbo.Constituent t1
	                        INNER JOIN dbo.Address t2 ON t1.Id = t2.ConstituentId
	                        INNER JOIN dbo.State t3 ON t2.StateId = t3.Id
	                        INNER JOIN dbo.Country t4 ON t3.CountryId = t4.Id
                        WHERE
                            t1.ID IN ({0})", idInClause);
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
