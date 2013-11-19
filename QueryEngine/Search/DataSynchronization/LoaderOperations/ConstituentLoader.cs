using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Search.Utils;

namespace Search.DataSynchronization.Loaders
{
    public class ConstituentLoader
    {
//        private MainContext _context;
//        //public List<Dictionary<string, object>> DataToload { get; set; }
//        public List<SourceObjectDefinition> ChildSourceObjectDefinitions { get; set; }
//        public SourceObjectDefinition ParentSourceObjectDefinition { get;  set; }
//        public bool SyncOnly { get; private set; }


//        private const int BatchSize = 500;

//        public ConstituentLoader(MainContext context, bool syncOnly)
//        {
//            _context = context;
//            SyncOnly = syncOnly;
//            ChildSourceObjectDefinitions = new List<SourceObjectDefinition>();

//            ParentSourceObjectDefinition = new SourceObjectDefinition
//            {
//                CollectionName = "Constituent",
//                SqlToRun = SyncOnly ? @"SELECT ID, KeyName, FirstName, MiddleName, Age FROM dbo.Constituent WITH (NOLOCK) WHERE ID = @ID" : 
//                                         @"SELECT ID, KeyName, FirstName, MiddleName, Age FROM dbo.Constituent WITH (NOLOCK)",
//                IsParent = true
//            };
        
//        ChildSourceObjectDefinitions.Add(new SourceObjectDefinition
//            {
//                CollectionName = "Address",
//                SqlToRun =
// @"SELECT
//    a.ID,
//    a.IsPrimary,
//	a.CONSTITUENTID,
//	ADDRESSBLOCK, 
//	CITY, 
//	POSTCODE,
//	s.ABBREVIATION AS StateAbrv,
//    c.Description AS CountryDesc, 
//    c.Abbreviation AS CountryAbrv
//FROM 
//	dbo.ADDRESS a WITH (NOLOCK)
//	INNER JOIN dbo.State s ON a.STATEID = s.ID AND a.COUNTRYID = s.COUNTRYID
// 	INNER JOIN dbo.Country c ON a.CountryId = c.Id
//WHERE
//    ConstituentID = @ID",
//                IsParent = false
//            });
//        }

//        public void Load()
//        {

//            using (var sqlconn = new SqlConnection(_context.DBConnection.GetConnectionStringTrusted()))
//            {
//                sqlconn.Open();
//                var cmd = new SqlCommand(ParentSourceObjectDefinition.SqlToRun, sqlconn);

//                var reader = cmd.ExecuteReader();
                
//                var index = 0;
//                var dataToload = new List<Dictionary<string, object>>();

//                while (reader.Read())
//                {
//                    dataToload.Add(EvalData.ReaderToDictionary(reader));
//                    index++;

//                    if (index%BatchSize == 0)
//                    {
//                        _context.EnqueueTask(new MongoDbLoader(this, dataToload, _context));
//                        dataToload = new List<Dictionary<string, object>>();
//                    }
//                }
//                _context.EnqueueTask(new MongoDbLoader(this, dataToload, _context));
//            }
//        }

//        public void Sync()
//        {
//            using (var sqlconn = new SqlConnection(_context.DBConnection.GetConnectionStringTrusted()))
//            {
//                sqlconn.Open();
                
//                foreach (var cacheMap in _context.CacheMaps)
//                {
//                    using (var cmd = new SqlCommand(ParentSourceObjectDefinition.SqlToRun, sqlconn))
//                    {
//                        cmd.Parameters.AddWithValue("ID", cacheMap.Id);

//                        using (var reader = cmd.ExecuteReader())
//                        {
//                            var index = 0;
//                            var dataToload = new List<Dictionary<string, object>>();

//                            while (reader.Read())
//                            {
//                                dataToload.Add(EvalData.ReaderToDictionary(reader));
//                                index++;

//                                if (index%BatchSize == 0)
//                                {
//                                    _context.EnqueueTask(new MongoDbLoader(this, dataToload, _context));
//                                    dataToload = new List<Dictionary<string, object>>();
//                                }
//                                _context.EnqueueTask(new MongoDbLoader(this, dataToload, _context,cacheMap));
//                            }
//                        }
//                    }
//                }
//            }
//        }
    }
}
