using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Search.Utils;
using SolrNet;
using SolrNet.Attributes;

namespace Search.ConstituentSearch
{
    public class ConstitEntity
    {


        public ConstitEntity()
        {
            Tags = new List<string>();
        }

        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("constituentId")]
        public string ConstituentId { get; set; }

        [SolrField("keyName")]
        public string KeyName { get; set; }
        
        [SolrField("firstName")]
        public string  FirstName { get; set; }

        [SolrField("age")]
        public int Age { get; set; }
        
        [SolrField("addressBlock")]
        public string AddressBlock { get; set; }
        
        [SolrField("city")]
        public string City { get; set; }
        
        [SolrField("stateAbrv")]
        public string StateAbrv { get; set; }
        
        [SolrField("countryAbrv")]
        public string CountryAbrv { get; set; }

        [SolrField("freeText")]
        public List<string> Tags { get; set; } 

        //[SolrField("appUsers")]
        //public List<string> AppUsers { get; set; }
    }
}
