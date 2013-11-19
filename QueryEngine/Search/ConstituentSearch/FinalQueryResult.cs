using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Search.ConstituentSearch
{
    public class FinalQueryResult
    {
        public Dictionary<Guid, BsonDocument> ResultMap { get; set; }
        public List<string> JsonResults { get; set; } 

        public FinalQueryResult()
        {
            ResultMap = new Dictionary<Guid, BsonDocument>();
            JsonResults = new List<string>();
        }
    }
}
