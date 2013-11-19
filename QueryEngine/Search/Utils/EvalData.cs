using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Search.Utils
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

        public static Dictionary<string, object> ReaderToDictionary(SqlDataReader reader)
        {
            var objectMap = new Dictionary<string, object>();

            var fieldCount = reader.FieldCount;

            for (var i = 0; i < fieldCount; i++)
            {
                objectMap[ConvertColumnName(reader.GetName(i))] = reader[i];
            }
            return objectMap;
        }

        public static Dictionary<string, object> ReaderToArray(SqlDataReader reader)
        {
            var objectArray = new Dictionary<string, object>();

            var fieldCount = reader.FieldCount;

            for (var i = 0; i < fieldCount; i++)
            {
                objectArray[ConvertColumnName(reader.GetName(i))] = reader[i];
            }
            return objectArray;
        }

        public static List<string> ReaderToList(SqlDataReader reader)
        {
            var list = new List<string>();

            var fieldCount = reader.FieldCount;

            for (var i = 0; i < fieldCount; i++)
            {
                list.Add(reader[i].ToString());
            }
            return list;
        }


        private static string ConvertColumnName(string columnName)
        {
            return columnName == "ID" ? "_id" : columnName;
        }
    }
}
