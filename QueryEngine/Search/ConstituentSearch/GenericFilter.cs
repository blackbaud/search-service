using MongoDB.Driver;

namespace Search.ConstituentSearch
{
    public class GenericFilter
    {
        public string  Key { get; private set; }
        public IMongoQuery Query { get; private set; }

        public GenericFilter(string key, IMongoQuery mongoQuery)
        {
            Key = key;
            Query = mongoQuery;
        }
    }
}
