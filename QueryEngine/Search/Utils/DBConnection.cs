using System.Collections.Generic;
using System.Data.SqlClient;

namespace Search.Utils
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


        public List<string> GetTableColumns(string tableName)
        {
            var columns = new List<string>();

            using (var sqlConn = new SqlConnection(GetConnectionStringTrusted()))
            {
                sqlConn.Open();
                var cmd = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName;", sqlConn);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    columns.Add(EvalData.EvalSqlDataReaderString(reader, "COLUMN_NAME"));
                }
                return columns;
            }
        }
    }
}
