using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MySearch.QueryProcessor;
using Search.Utils;

namespace Search.ConstituentSearch
{
    public class QueryProcessor
    {
        private readonly string _queryRequest;
        public DBConnection MongoDBConnection { get; private set; }
        public List<GenericSelection> Selections { get; private set; }
        public List<IMongoQuery> Filters { get; private set; }
        public GenericFilter Filter { get; private set; }
        
        public QueryProcessor(string query)
        {
            _queryRequest = query;
            Selections = GetSelections();
            Filters = GetFilters();
            MongoDBConnection = new DBConnection
            {
                ServerName = Properties.Settings.Default.MongoServer,
                DataBaseName = Properties.Settings.Default.MongoDatabase
            };
        }

        public FinalQueryResult Execute()
        {
           

            var query = new GenericQuery(this);

            return query.Execute();
        }

        private List<GenericSelection> GetSelections()
        {
            var genericSelections = new List<GenericSelection>();

            var queryRequest = new JavaScriptSerializer().Deserialize<dynamic>(_queryRequest);
            var selections = (queryRequest["Selections"]);

            foreach (var selection in selections)
            {
                genericSelections.Add(new GenericSelection(selection["CollectionName"],  selection["FieldName"]));
            }

            return genericSelections;
        }

        private List<IMongoQuery> GetFilters()
        {
            var genericFilters = new List<GenericFilter>();
            var queryRequest = new JavaScriptSerializer().Deserialize<dynamic>(_queryRequest);
            var filters = (queryRequest["Filters"]);
            var filterBuilder = new List<string>{"{"};

            var queryList = new List<IMongoQuery>();

            
            foreach (var filter in filters)
            {
                var sb = new StringBuilder();
                string filterOperator = filter["Operator"];
                switch (filterOperator)
                {
                    case "IN":
                        //sb = new StringBuilder();
                        //genericFilters.Add(new GenericFilter(filter["FilterName"], MongoDB.Driver.Builders.Query.In(filter["FilterName"], new BsonArray(BuildFilterValues(filter["Key"]).ToArray()))));

                        queryList.Add( MongoDB.Driver.Builders.Query.In(filter["FilterName"], new BsonArray(BuildFilterValues(filter["Key"]).ToArray())));

                        //filterBuilder.Add(filter["FilterName"]);
                        //sb.Append(filter["FilterName"]);
                        //sb.Append(":");
                        //sb.Append("{$in [");
                        //sb.Append(string.Join(",", BuildFilterValues(filter["key"])));
                        //sb.Append("}");
                        //filterBuilder.Add(sb.ToString());
                        break;
                    case "LIKE":
                        //sb = new StringBuilder();

                        //var query = MongoDB.Driver.Builders.Query.Matches(filter["FilterName"], "^" + GetFilterValue(filter["Key"]));
                        //genericFilters.Add(new GenericFilter(filter["FilterName"], query));

                        queryList.Add(MongoDB.Driver.Builders.Query.Matches(filter["FilterName"], "^" + filter["Key"]));

                        //sb.Append(filter["FilterName"]);
                        //sb.Append("^");
                        //sb.Append(GetFilterValue(filter["Key"]));
                        //filterBuilder.Add(sb.ToString());
                        break;
                    default:
                        Console.WriteLine("Please provide a valid Filter Operator.");
                        break;
                }
                //genericFilter.Add(new GenericFilter(filter["FilterName"], BuildFilterValues(filter["Key"])));
            }
            //filterBuilder.Add("}");


            return queryList;
        }


        private static string GetCollectionName(string value)
        {
            return value.Substring(0, value.IndexOf("."));
        }

        private static string GetKeyName(string value)
        {
            var startingIndex = value.IndexOf(".") + 1;

            return value.Substring(startingIndex, value.Length - startingIndex );
        }

        private static List<BsonValue> BuildFilterValues(object[] values)
        {
            var returnValues = new List<BsonValue>();

            for (var i = 0; i < values.Length; i++)
            {
                returnValues.Add(BsonValue.Create(values[i]));
            }
            return returnValues;
        }

        private static string GetFilterValue(object[] value)
        {
            return value[0].ToString();
        }
    }
}
