using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEngine.Utils.Database
{
    public class SchemaInfo
    {
        private const string sql = @"
SELECT 
    COLUMN_NAME 
FROM 
    INFORMATION_SCHEMA.COLUMNS WITH (NOLOCK) 
WHERE 
    TABLE_NAME = @TableName";

        public Dictionary<string, List<string>> ColumnMap { get; private set; }


        private readonly DBConnection _dbConnection;

        private List<string> _objects;

        public SchemaInfo(DBConnection dbConnection, List<string> objects )
        {
            _dbConnection = dbConnection;
            _objects = objects;
            ColumnMap = new Dictionary<string, List<string>>();
        }


        public void Build()
        {
            using (var sqlconn = new SqlConnection(_dbConnection.GetConnectionStringTrusted()))
            {
                sqlconn.Open();

                foreach (var tableName in _objects)
                {
                    using (var cmd = new SqlCommand(sql, sqlconn))
                    {
                        cmd.Parameters.AddWithValue("@TableName", tableName);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!ColumnMap.ContainsKey(tableName))
                                {
                                    ColumnMap[tableName] = new List<string> { reader.GetString(reader.GetOrdinal("COLUMN_NAME")) };
                                }
                                else
                                {
                                    ColumnMap[tableName].Add(reader.GetString(reader.GetOrdinal("COLUMN_NAME")));
                                }
                            }
                        }
                    }
                }

            }
        }
    }
}
