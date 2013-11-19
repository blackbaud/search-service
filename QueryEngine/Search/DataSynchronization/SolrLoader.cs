using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Search.ConstituentSearch;
using Search.DataSynchronization.Loaders;
using Search.Utils;
using SolrNet;

namespace Search.DataSynchronization
{
    public class SolrLoader :ITask
    {

        public SolrLoader(List<ConstitEntity> constitEntities)
        {
            _constitEntities = constitEntities;
        }

        private MainContext _context;
        private List<ConstitEntity> _constitEntities;

        public void Execute()
        {
            var sw = Stopwatch.StartNew();

            //BuildConsitInClause();

            //BuildAppUsers();
            //AddAppUsersToConsitEntity();
            UpdateSolr();
            

            sw.Stop();

            Console.WriteLine("{0}-{1} - documents loaded. Total Time:{2}(ms)",DateTime.Now, _constitEntities.Count, sw.ElapsedMilliseconds);
        }

        
        private void UpdateSolr()
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
            //var batchSize = 10;
            //var index = 1;
            foreach (var entity in _constitEntities)
            {
                solr.Add(entity);
            }
           // solr.Commit();
        }
    }
}
