using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoClientLib.Commons
{
    public class OrderData
    {
        public int OrderId { get; set; }
        public Symbol Symbol { get; set; }
        public double Price { get; set; }
        public OrderType OrderType { get; set; }
    }
}
