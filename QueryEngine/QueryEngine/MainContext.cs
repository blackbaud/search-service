using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QueryEngine.Utils.Database;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using QueryEngine.Utils.Threading;

namespace QueryEngine
{
    public class MainContext
    {
        private const int IdBatchSize = 1000;

        public static MainContext Context;

        private DBConnection _dbConnection;
        private Task _task;
        private TaskProcessor _taskProcessor;

        public SchemaInfo ObjectsBeingUsed { get; private set; }
        public List<Guid> MainIdList { get; private set; }
        public List<string> ObjectsToInclude { get; private set; } 

        public static void Start()
        {
            Context = new MainContext();
            Context.InternalStart();
        }

        private void InternalStart()
        {
            _dbConnection = new DBConnection
                {
                    DataBaseName = Properties.Settings.Default.SourceDatabase, 
                    ServerName = Properties.Settings.Default.SourceServer, 
                    AppDesc = Properties.Settings.Default.AppDescription
                };

            _task = new Task(()=> Context.BeginProcess());
            _task.Start();
        }

        private void BeginProcess()
        {
            _taskProcessor = new TaskProcessor(Properties.Settings.Default.ThreadCount, Properties.Settings.Default.QueueCapacity);
            _taskProcessor.Start();

            ObjectsToInclude = Properties.Settings.Default.SourceObjects.Replace(" ", "").Split(',').ToList();

            ObjectsBeingUsed = new SchemaInfo(_dbConnection, ObjectsToInclude);
            ObjectsBeingUsed.Build();

            LoadIdList();
            var startingIndex = 0;
            for (var i = 0; i < MainIdList.Count; i++ )
            {
                if (i%IdBatchSize != 0 || i ==0) continue;

                _taskProcessor.Enqueue(new DocumentBuilder(this, IdBatchSize,startingIndex));
                startingIndex = i;
            }
        }


        private void LoadIdList()
        {
            var sw = Stopwatch.StartNew();

            MainIdList = new List<Guid>();

            using (var sqlConn = new SqlConnection(_dbConnection.GetConnectionStringTrusted()))
            {
                sqlConn.Open();
                var cmd = new SqlCommand("SELECT ID FROM dbo.Constituent WITH (NOLOCK)", sqlConn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    MainIdList.Add(reader.GetGuid(reader.GetOrdinal("ID")));
                }
            }

            sw.Stop();
            Console.WriteLine("Main ID List Loaded - Total Records Loaded:{1} - Total Time Ellapsed:{0}(ms)", sw.ElapsedMilliseconds, MainIdList.Count);

        }       
    }
}
