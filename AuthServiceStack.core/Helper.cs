using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceStack.core
{
    public class Helper
    {
        public static string GetHash(string input)
        {
            System.Security.Cryptography.HashAlgorithm hashAlgorithm = new System.Security.Cryptography.SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
        public static int AuthorEditReputation = 10;
        public static int PublicEditReputaion = 40;
        public static int PublicProtectReputation = 100;
        public static int PublicCloseReputation = 1000;
    }
}
