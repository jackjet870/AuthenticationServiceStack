using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthCenter
{
    public static class ClaimsAbbreviationDictionary
    {
        public const string get = "http://mallconsoleapp/crud/get";
        public const string post = "http://mallconsoleapp/crud/post";
        public const string put = "http://mallconsoleapp/crud/put";
        public const string delete = "http://mallconsoleapp/crud/delete";
        public const string resource = "http://mallconsoleapp/resource";
        public static readonly Dictionary<string, string> dict = new Dictionary<string, string>
        {
            {"get",get},{"post",post},{"put",put},{"delete",delete},{"role",System.Security.Claims.ClaimTypes.Role}
        };
    }

}