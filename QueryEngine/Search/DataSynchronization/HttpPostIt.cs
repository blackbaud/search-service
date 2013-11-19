using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Net.Http;
using Search.Utils;

namespace Search.DataSynchronization
{
    public class HttpPostIt : ITask
    {
        private Dictionary<string,string> _dataToLoad;
        private string _url;

        public HttpPostIt(Dictionary<string, string> dataToLoad, string url)
        {
            _dataToLoad = dataToLoad;
            _url = url;
        }

        public void Execute()
        {
            var sw = Stopwatch.StartNew();

            foreach (var entity in _dataToLoad)
            {
                using (var client = new WebClient())
                {
                    client.UploadString(string.Format("{0}/{1}", _url, entity.Key), entity.Value);
                }
            }

            sw.Stop();
            Console.WriteLine("Total Records written into Elastic Search {0} - Total Time:{1}(ms)",_dataToLoad.Count,sw.ElapsedMilliseconds);
        }
    }
}
