using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoClientLib
{
	public class BinanceClientCreator : ICryptoCurrencyCreator
	{
		public ICryptoClient CreateCryptoClient(string publicKey, string privateKey)
		{
			return new BinanceClient(publicKey, privateKey);
		}
	}
}
