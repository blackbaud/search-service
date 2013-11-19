using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Search.Utils;

namespace Search.Contexts
{
    public class QueryContext
    {
        public DBConnection MongoDBConnection { get; private set; }

        public QueryContext()
        {
            MongoDBConnection = new DBConnection
            {
                ServerName = Properties.Settings.Default.MongoServer,
                DataBaseName = Properties.Settings.Default.MongoDatabase
            };
        }
    }
}
