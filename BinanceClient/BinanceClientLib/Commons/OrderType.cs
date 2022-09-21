using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoClientLib.Commons
{
	public enum OrderType
	{
		LIMIT,
		MARKET,
		STOP_LOSS,
		TAKE_PROFIT,
		Undefined
	}
}
