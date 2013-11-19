using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using bb.api.Models;
using bb.api.Services;

namespace bb.api.Controllers
{
    public class SearchController : ApiController
    {
        public ConstituentSearch Get()
        {
            var query = new BasicSearch();
            return query.GetSearch();
        }
    }
}
