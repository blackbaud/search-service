using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Search.Utils;

namespace Search.DataSynchronization
{
    public class CacheCollector
    {
        public int CacheCount { get; private set; }

        private const string SQL =
@"DECLARE @Table TABLE (
	idx INT,
	Id UNIQUEIDENTIFIER,
	SourceDesc SYSNAME,
	LockDate DATETIME)
UPDATE dbo.DataSync_Cache
	SET LockDate = GETDATE()
OUTPUT
	inserted.idx,
	inserted.Id,
	inserted.SourceDesc,
	inserted.LockDate
INTO @Table
WHERE LockDate IS NULL

SELECT * FROM @Table";

       

        private MainContext _context;

        public CacheCollector(MainContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            LoadMap();
        }

        private void LoadMap()
        {
            using (var sqlConn = new SqlConnection(_context.DBConnection.GetConnectionStringTrusted()))
            {
                sqlConn.Open();
                var cmd = new SqlCommand(SQL, sqlConn);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var desc = EvalData.EvalSqlDataReaderString(reader, "SourceDesc");

                    if (!_context.CacheMap.ContainsKey(desc))
                    {
                        _context.CacheMap[desc] = new List<SyncCache>
                        {
                            new SyncCache
                            {
                                Idx = reader.GetInt32(reader.GetOrdinal("idx")),
                                Id = EvalData.EvalSqlDataReaderGuid(reader, "Id"),
                                SourceDesc = EvalData.EvalSqlDataReaderString(reader, "SourceDesc"),
                                LockDate = EvalData.EvalSqlDataReaderDateTime(reader, "LockDate")
                            }
                        };
                    }
                    else
                    {
                        _context.CacheMap[desc].Add(new SyncCache
                        {
                            Idx = reader.GetInt32(reader.GetOrdinal("idx")),
                            Id = EvalData.EvalSqlDataReaderGuid(reader, "Id"),
                            SourceDesc = EvalData.EvalSqlDataReaderString(reader, "SourceDesc"),
                            LockDate = EvalData.EvalSqlDataReaderDateTime(reader, "LockDate")
                        });
                    }

                    CacheCount++;
                }
            }
        }

        public void RemoveCacheRecord(int index)
        {
            using (var sqlconn = new SqlConnection(_context.DBConnection.GetConnectionStringTrusted()))
            {
                sqlconn.Open();

                var cmd = new SqlCommand("DELETE FROM dbo.DataSync_Cache WHERE idx = @idx", sqlconn);
                cmd.Parameters.AddWithValue("idx", index);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
