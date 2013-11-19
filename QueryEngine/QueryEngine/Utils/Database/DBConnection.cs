using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEngine.Utils.Database
{
    public class DBConnection
    {

        public string ServerName { get; set; }
        public string DataBaseName { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string AppDesc { get; set; }


        public DBConnection(string DbName)
        {
            DataBaseName = DbName;
        }

        public DBConnection()
        {

        }

        public string GetConnectionStringTrusted()
        {
            return string.Format(@"Data Source={0};Initial Catalog={1};Integrated Security=SSPI;App={2}", ServerName, DataBaseName, AppDesc);
        }
        public string GetConnectionStringStandard()
        {
            return string.Format(@"Data Source={0};Initial Catalog={1};UserID={2};Password={3}; App={4}", ServerName, DataBaseName, UserID, Password, AppDesc);
        }

        public string GetMongoDBConnectionString()
        {
            return string.Format(@"mongodb://{0}/?safe=true", ServerName);
        }

        public void ExecuteSQL(SqlCommand cmd)
        {
            using (cmd.Connection)
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }

        }
    }
}
