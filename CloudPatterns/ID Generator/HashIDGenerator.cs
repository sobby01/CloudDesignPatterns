using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.ID_Generator
{
    public class HashIDGenerator
    {
        public string GenerateDynamoIdentifier(string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(keyBytes);

                // Extract the first 16 bytes (128-bit) of the MD5 hash as the identifier
                byte[] identifierBytes = new byte[16];
                Array.Copy(hashBytes, identifierBytes, 16);

                // Convert the identifier bytes to a hexadecimal string representation
                string identifier = BitConverter.ToString(identifierBytes).Replace("-", "").ToLower();

                return identifier;
            }
        }

        public string Generate64BitDynamoIdentifier(string key)
        {
            //first get the byte array
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            using(MD5 md5 = MD5.Create())
            {
                //then get the hash value
                byte[] hashBytes = md5.ComputeHash(keyBytes);
                long bit64 = BitConverter.ToInt64(hashBytes, 0);
                return bit64.ToString();
            }
        }
    }
}
