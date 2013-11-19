using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Search.DataSynchronization
{
    public class SyncCache
    {
        public int Idx { get; set; }
        public Guid Id { get; set; }
        public string SourceDesc { get; set; }
        public DateTime LockDate { get; set; }

    }
}
