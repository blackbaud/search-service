using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Search.DataSynchronization.LoaderOperations;
using Search.DataSynchronization.Loaders;
using Search.DataSynchronization.SyncOperations;
using Search.Utils;

namespace Search.DataSynchronization
{
    public class MainContext
    {

        public static MainContext Context;

        private List<ILoader> _Loader; 
        private Task _pollerThread;
        private Task _task;

        private CancellationTokenSource _tokenSource;
        public TaskProcessor _taskProcessor;
        public bool SyncOnly { get; set; }

        public DBConnection DBConnection { get; private set; }
        public DBConnection MongoDBConnection { get; private set; }
        public Dictionary<string, List<SyncCache>> CacheMap { get; set; }

        private Dictionary<string, Dictionary<string,SyncCache>> _dataToSync; 

        public MainContext(bool syncOnly)
        {
            DBConnection = new DBConnection
            {
                DataBaseName = Properties.Settings.Default.SQLDatabase,
                ServerName = Properties.Settings.Default.SQLServer
            };

            MongoDBConnection = new DBConnection
            {
                DataBaseName = Properties.Settings.Default.MongoDatabase,
                ServerName = Properties.Settings.Default.MongoServer
            };

            CacheMap = new Dictionary<string, List<SyncCache>>();
            _dataToSync = new Dictionary<string, Dictionary<string, SyncCache>>();

            _Loader = new List<ILoader> {new ConstituentLoaderV2(this), new ConstituentSecurityLoader(this)};

            SyncOnly = syncOnly;
        }

        public static void Start(bool syncOnly)
        {
            Context = new MainContext(syncOnly);
            Context.InternalStart();
        }

        private void InternalStart()
        {
            _task = new Task(BeginProcess);
            _task.Start();
        }

        private void BeginProcess()
        {
            _taskProcessor = new TaskProcessor(Properties.Settings.Default.ThreadCount, 500);
            _taskProcessor.Start();

            _tokenSource = new CancellationTokenSource();
            _pollerThread = SyncOnly ? new Task(SyncData, _tokenSource.Token) : new Task(LoadData, _tokenSource.Token);
            _pollerThread.Start();
            Console.WriteLine("Poller Thread now started - {0}", DateTime.Now);
        }

        private void SyncData()
        {
            while (!IsCancellationRequested())
            {
                CacheMap =  new Dictionary<string, List<SyncCache>>();
                SwitchCache();
                
                Thread.Sleep(Properties.Settings.Default.SyncSleep);
                
                var cacheCollector = new CacheCollector(this);
                cacheCollector.Execute();

                var caches = new List<SyncCache>();
                var index = 1;
                //Update Solr
                foreach (var cacheDesc in CacheMap)
                {
                    if (cacheDesc.Key.ToLower() == "constituent")
                    {
                        caches = new List<SyncCache>();
                        foreach (var cache in cacheDesc.Value)
                        {

                            caches.Add(cache);

                            if (index%100 == 0)
                            {
                                EnqueueTask(new SolrUpdater(caches, this));
                                caches = new List<SyncCache>();
                            }

                            index++;
                        }
                        if (caches.Count > 0)
                        {
                            EnqueueTask(new SolrUpdater(caches, this));
                        }
                    }
                    else if(cacheDesc.Key.ToLower() == "constitsecurity")
                    {
                        caches = new List<SyncCache>();
                        foreach (var cache in cacheDesc.Value)
                        {

                            caches.Add(cache);

                            if (index % 1 == 0)
                            {
                                EnqueueTask(new MongoDbUpdater(caches,this));
                                caches = new List<SyncCache>();
                            }

                            index++;
                        }
                        if (caches.Count > 0)
                        {
                            EnqueueTask(new SolrUpdater(caches, this));
                        }
                    }
                }
            }
        }

        private bool IsCancellationRequested()
        {
            var token = _tokenSource.Token;
            return token.IsCancellationRequested;
        }

        private void SwitchCache()
        {
            using (var sqlConn = new SqlConnection(DBConnection.GetConnectionStringTrusted()))
            {
                sqlConn.Open();
                var cmd = new SqlCommand("dbo.DataServiceSync", sqlConn) {CommandType = CommandType.StoredProcedure};

                var sqlParam = new SqlParameter("@ReturnValue", SqlDbType.Int) {Direction = ParameterDirection.Output};

                cmd.Parameters.Add(sqlParam);
                cmd.ExecuteNonQuery();
            }
        }


        private void LoadData()
        {
            foreach (var loader in _Loader)
            {
                loader.Load();
            } 
        }
        
        public void EnqueueTask(Utils.ITask task)
        {
            _taskProcessor.Enqueue(task);
        }


        
    }
}
