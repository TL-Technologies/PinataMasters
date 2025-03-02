using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


namespace Modules.General.Utilities
{
    public static class HashUtilities
    {
        #region Methods

        public static string Md5Hash(string textData, string salt) =>
            Hash(Encoding.UTF8.GetBytes(textData), salt, MD5.Create());


        public static string Md5Hash(byte[] byteData, string salt) =>
            Hash(byteData, salt, MD5.Create());
        
        
        public static string Sha1Hash(string textData, string salt) =>
            Hash(Encoding.UTF8.GetBytes(textData), salt, SHA1.Create());
        

        public static string Sha1Hash(byte[] byteData, string salt) =>
            Hash(byteData, salt, SHA1.Create());


        public static string Hash(string textData, string salt, HashAlgorithm hashAlgorithm) =>
            Hash(Encoding.UTF8.GetBytes(textData), salt, hashAlgorithm);


        public static string Hash(byte[] byteData, string salt, HashAlgorithm hashAlgorithm)
        {
            byte[] bytes = null;
            if (salt.IsNullOrEmpty())
            {
                bytes = byteData;
            }
            else
            {
                List<byte> fullByteData = new List<byte>(byteData.Length + salt.Length * 4);
                fullByteData.AddRange(byteData);
                fullByteData.AddRange(Encoding.UTF8.GetBytes(salt));
                bytes = fullByteData.ToArray();
            }

            byte[] hash = hashAlgorithm.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }


        public static string SHA256HMAC(string textData, string key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(textData));

                return Convert.ToBase64String(hash);
            }
        }

        #endregion
    }
}

