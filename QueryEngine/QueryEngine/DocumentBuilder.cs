using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryEngine.Utils.Threading;

namespace QueryEngine
{
    public class DocumentBuilder:ITask
    {

        private MainContext _context;

        private int _batchSize;
        private int _startingLocation;


        public DocumentBuilder(MainContext context, int batchSize, int startingLocation)
        {
            _context = context;
            _batchSize = batchSize;
            _startingLocation = startingLocation;
        }

        public void Execute()
        {
            for (var i = 0; i < _batchSize; i++)
            {
                Console.WriteLine();
            }
        }
    }
}
