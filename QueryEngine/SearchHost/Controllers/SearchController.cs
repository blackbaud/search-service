using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Search.ConstituentSearch;

namespace SearchHost.Controllers
{
    public class SearchController : ApiController
    {
        public IEnumerable<ConstitEntity> Get(string searchTerm, string appuserId)
        {
            var search = new QuerySearchIndex(new Guid(appuserId), searchTerm);
            search.Execute();

            return search.ViewableResultsList;
        }

    }
}
