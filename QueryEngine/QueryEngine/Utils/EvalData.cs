using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEngine.Utils
{
    public class EvalData
    {
        public static string EvalSqlDataReaderString(SqlDataReader reader, string colName)
        {
            return reader[colName] == DBNull.Value ? string.Empty : reader.GetString(reader.GetOrdinal(colName));
        }

        public static Guid EvalSqlDataReaderGuid(SqlDataReader reader, string colName)
        {
            return reader[colName] == DBNull.Value ? Guid.Empty : reader.GetGuid(reader.GetOrdinal(colName));
        }

        public static DateTime EvalSqlDataReaderDateTime(SqlDataReader reader, string colName)
        {
            return reader[colName] == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal(colName));
        }
    }
}
