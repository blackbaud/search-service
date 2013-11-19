using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Search.Utils;

namespace Search.Security
{
    public class ConstituentSecurity
    {
        public string SecurityHash { get; private set; }
        public Guid ConstituentId { get; private set; }
        public Guid AppUserID { get; private set; }

        public ConstituentSecurity(Guid constituentId, Guid appUserId)
        {
            SecurityHash = GenSecurityHash(constituentId, appUserId);
            ConstituentId = constituentId;
            AppUserID = appUserId;
        }


        public static string GenSecurityHash(Guid constituentId, Guid appUserId)
        {
           return HashMaker.MakeMD5Hash(string.Format("{0}{1}", constituentId, appUserId));
        }
    }
}
