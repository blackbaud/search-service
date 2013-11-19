using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using bb.api.Models;
using Search.ConstituentSearch;

namespace bb.api.Services
{
    public class BasicSearch
    {
        public ConstituentSearch GetSearch()
        {
            try
            {
                var context = HttpContext.Current;

                var queryString = context.Request.QueryString.GetValues("q");

                //Console.WriteLine(queryString);

                var queryProcessor = new QueryProcessor(queryString[0]);
                var output = queryProcessor.Execute();

                var constitSearch = new ConstituentSearch { SearchResult = output };


                return constitSearch;
            }
            catch (Exception exception)
            {
                return null;
            }
        }
    }
}