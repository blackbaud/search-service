using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Search.DataSynchronization.Loaders
{
    public interface ILoader
    {
        void Load();
        void Sync();
    }
}
