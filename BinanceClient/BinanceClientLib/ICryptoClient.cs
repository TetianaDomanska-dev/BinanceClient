using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoClientLib.Commons;

namespace CryptoClientLib
{
    internal interface ICryptoClient
    {
	    public Task<OrderData> CreateOrder(OrderType orderType, Symbol symbol, Side side, 
		    double quantity = 0, double expectedPrice = 0);
	    public double GetPrice(Symbol symbol);
    }
}
