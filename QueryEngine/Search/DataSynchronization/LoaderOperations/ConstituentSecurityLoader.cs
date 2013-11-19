using System.Collections.Generic;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Search.DataSynchronization.Loaders;
using Search.Security;
using Search.Utils;

namespace Search.DataSynchronization.LoaderOperations
{
    public class ConstituentSecurityLoader :ILoader
    {
        public const string CacheDescription = "constitsecurity";

        public ConstituentSecurityLoader(MainContext context)
        {
            _context = context;
            _dataToLoad = new Dictionary<string, string>();
        }

        private MainContext _context;
        private Dictionary<string, string> _dataToLoad;

        private const string MongoDBCollectionName = "ConstituentSecurity";
        private const int BatchSize = 100;
        private const string ConstitSecuritySQL =
@"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
;WITH baseCte AS (
SELECT DISTINCT
   -- NULL AS _id,
	--t5.ConstituentID,
	t1.ID AS AppUserID,
	t4.ID
FROM 
	dbo.AppUser t1
	INNER JOIN dbo.SYSTEMROLEAPPUSER t2 ON t1.ID = t2.AppUSerID
	INNER JOIN dbo.SYSTEMROLEAPPUSERCONSTITUENTSECURITY t3 ON t2.ID = t3.SystemRoleAppUserID
	INNER JOIN dbo.CONSTIT_SECURITY_ATTRIBUTE t4 ON t3.ConstituentSecurityAttributeId = t4.Id
	--INNER JOIN dbo.CONSTIT_SECURITY_ATTRIBUTE_ASSIGNMENT t5 ON t4.ID = t5.Constit_Security_AttributeID
)
SELECT
	t2.AppUserID, 
	t1.ConstituentID
FROM 
	dbo.CONSTIT_SECURITY_ATTRIBUTE_ASSIGNMENT t1
	INNER JOIN baseCte t2 ON t1.Constit_Security_AttributeID = t2.ID
--WHERE
--	t1.ConstituentID = 'D7B74CB1-164E-45F9-98E4-2C0A7038F0B7'
";



        public void Load()
        {
            using (var sqlConn = new SqlConnection(_context.DBConnection.GetConnectionStringTrusted()))
            {
                sqlConn.Open();

                var cmd = new SqlCommand(ConstitSecuritySQL, sqlConn);
                cmd.CommandTimeout = 0;
                var reader = cmd.ExecuteReader();

                var index = 1;
                while (reader.Read())
                {
                    var constitSecurity = new ConstituentSecurity(EvalData.EvalSqlDataReaderGuid(reader, "ConstituentID"), EvalData.EvalSqlDataReaderGuid(reader, "AppUserID"));
                    _dataToLoad[constitSecurity.SecurityHash] = JsonConvert.SerializeObject(constitSecurity);

                    if (index%BatchSize == 0)
                    {
                        _context.EnqueueTask(new HttpPostIt(_dataToLoad, "http://localhost:9200/constituentsecurity/hash"));
                        _dataToLoad = new Dictionary<string, string>();
                    }
                    index++;
                }
            }
        }

        public void Sync()
        {
           
        }
    }
}
