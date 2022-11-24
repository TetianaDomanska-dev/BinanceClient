using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoClientLib.Commons
{
	public class KlineData
	{
		public string? OpenTime { get; set; }
		public double OpenPrice { get; set; }
		public double HighPrice { get; set; }
		public double LowPrice { get; set; }
		public double ClosePrice { get; set; }
		public string? CloseTime { get; set; }

	}
}
