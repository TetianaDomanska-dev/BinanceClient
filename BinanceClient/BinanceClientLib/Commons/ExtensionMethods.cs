using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoClientLib.Commons
{
	static class ExtensionMethods
	{
		public static String ToString(this Asset asset1)
		{
			switch (asset1)
			{
				case Asset.BTC:
					return "BTC";
				case Asset.USDT:
					return "USDT";
				default:
					return "Undefined";
			}
		}

		public static String ToString(this OrderType orderType)
		{
			switch (orderType)
			{
				case OrderType.LIMIT:
					return "LIMIT";
				case OrderType.MARKET:
					return "MARKET";
				default:
					return "Undefined";
			}
		}

		public static String ToString(this Side side)
		{
			switch (side)
			{
				case Side.BUY:
					return "BUY";
				case Side.SELL:
					return "SELL";
				default:
					return "Undefined";
			}
		}
	}
}
