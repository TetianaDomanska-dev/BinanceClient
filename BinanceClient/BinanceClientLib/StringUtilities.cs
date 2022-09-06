using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoClientLib
{
	public static  class StringUtilities
	{
		public static string ComputeHMacSha256(string text, string key)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			byte[] keyBytes = Encoding.UTF8.GetBytes(key);
			using (var hmac = new HMACSHA256(keyBytes))
			{
				hmac.ComputeHash(bytes);
				return Convert.ToHexString(hmac.Hash);
			}
		}
    }
}
