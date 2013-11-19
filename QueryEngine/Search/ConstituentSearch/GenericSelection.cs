using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySearch.QueryProcessor
{
    public class GenericSelection
    {
        public string ObjectName { get; private set; }
        public string Key { get; private set; }

        public GenericSelection(string objectName, string key)
        {
            ObjectName = objectName;
            Key = key;
        }
    }
}
