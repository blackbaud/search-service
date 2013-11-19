using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MySearch.QueryProcessor
{
    public class GenericResultset
    {
        [BsonId]
        public Guid Id { get; set; }
    }
}
