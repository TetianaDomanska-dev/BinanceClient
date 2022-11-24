using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoClientLib.Commons;

namespace CryptoClientLib
{
    public interface ICryptoClient
    {
	    public Task<OrderData> CreateOrder(OrderType orderType, Symbol symbol, Side side, 
		    double quantity = 0, double expectedPrice = 0);
	    public Task<double> GetPrice(Symbol symbol);

	    public Task<List<KlineData>> GetKlines(Symbol symbol, string interval, UInt64 countOfCandles);

    }
}
