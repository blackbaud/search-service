using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Search.DataSynchronization;

namespace SyncEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == "sync")
                {
                    MainContext.Start(true);
                    Console.Read();
                }
            }
        }
    }
}
