using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MySearch.QueryProcessor
{
    public class QueryResult
    {
        public Dictionary<Guid, int> FilterIds { get; set; }
        public Dictionary<Guid, int> ConstitIds { get; set; }
    }
}
