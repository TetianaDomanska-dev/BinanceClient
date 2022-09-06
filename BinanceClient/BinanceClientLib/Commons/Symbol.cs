using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoClientLib.Commons
{
    public class Symbol
    {
        public Asset Base { get; set; }
        public Asset Quote { get; set; }

        public Symbol(Asset bbase, Asset quote)
        {
	        this.Base = bbase;
            this.Quote = quote;
        }

        public override string ToString()
        {
	        return this.Base.ToString() + this.Quote.ToString();
        }
    }
}