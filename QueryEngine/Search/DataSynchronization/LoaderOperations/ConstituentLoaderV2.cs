using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using Search.ConstituentSearch;
using Search.Utils;
using SolrNet;

namespace Search.DataSynchronization.Loaders
{
    public class ConstituentLoaderV2 :ILoader
    {
        public const string CacheDescription = "constituent";

        public ConstituentLoaderV2(MainContext context)
        {
            _context = context;
            _constitEntities = new Dictionary<string, string>();
        }

        

        private bool _sleep = true;
        private MainContext _context;
        private const int BatchSize = 100;
        private Dictionary<string,string> _constitEntities;
        private bool _syncOnly = true;
        private string _sql;
 
        
        public void LoadSolr()
        {
            ISolrOperations<ConstitEntity> solr = null;
            try
            {
                Startup.Init<ConstitEntity>(Properties.Settings.Default.solrUrl);
            }
            catch (Exception ex)
            {
                //todo nothing
            }
            finally
            {
                solr = ServiceLocator.Current.GetInstance<ISolrOperations<ConstitEntity>>();
            }
            
         
          
            var sw = Stopwatch.StartNew();

            var dbConn = new Utils.DBConnection
            {
                DataBaseName = Properties.Settings.Default.SQLDatabase,
                ServerName = Properties.Settings.Default.SQLServer
            };


            using (var sqlconn = new SqlConnection(dbConn.GetConnectionStringTrusted()))
            {
                sqlconn.Open();

                var cmd = new SqlCommand(
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
	INNER JOIN dbo.Country t4 ON t3.CountryId = t4.Id", sqlconn);

                cmd.CommandTimeout = 0;

                var index = 1;
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    BuildConstitEntity(reader);

                    if (index%BatchSize == 0)
                    {
                        //Console.WriteLine("{0} - records loaded.", index);
                        _context.EnqueueTask(new HttpPostIt(_constitEntities, "http://localhost:9200/constituents/constituent"));
                        _constitEntities = new Dictionary<string, string>();
                    }
                    index++;
                }

                while (_context._taskProcessor.GetQueueDepth() !=0)
                {
                    Thread.Sleep(10000);
                }

                //while (_sleep)
                //{
                //    solr.Commit();

                //    Thread.Sleep(10000);

                //    _sleep = false;
                //}

            }

            sw.Stop();

            Console.WriteLine("Total Load Time:{0}(ms)", sw.ElapsedMilliseconds);
        }

        public void BuildConstitEntity(SqlDataReader reader)
        {
            var constEntity = new ConstitEntity
            {
                Id = HashMaker.MakeMD5Hash(string.Format("{0}{1}", EvalData.EvalSqlDataReaderGuid(reader, "ConstitId"), EvalData.EvalSqlDataReaderGuid(reader, "AddressId"))),
                ConstituentId = EvalData.EvalSqlDataReaderGuid(reader, "ConstitId").ToString(),
                KeyName = EvalData.EvalSqlDataReaderString(reader, "KeyName"),
                FirstName = EvalData.EvalSqlDataReaderString(reader, "FirstName"),
                AddressBlock = (EvalData.EvalSqlDataReaderString(reader, "AddressBlock")),
                City = (EvalData.EvalSqlDataReaderString(reader, "City")),
                StateAbrv = (EvalData.EvalSqlDataReaderString(reader, "StateAbrv")),
                CountryAbrv = (EvalData.EvalSqlDataReaderString(reader, "CountryAbrv")),
                Tags =  EvalData.ReaderToList(reader)
            }; 

            _constitEntities[constEntity.Id] = JsonConvert.SerializeObject(constEntity);
        }

        public void Load()
        {
            LoadSolr();
        }

        public void Sync()
        {
            throw new NotImplementedException();
        }
    }
}
