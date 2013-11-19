using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Search.Utils
{
    public class HashMaker
    {
        public static string MakeMD5Hash(string value)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(value);
            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
