using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Search.DataSynchronization
{
    public class SourceObjectDefinition
    {
        public string CollectionName { get; set; }
        public string SqlToRun { get; set; }  
        public  bool IsParent { get; set; }
    }
}
