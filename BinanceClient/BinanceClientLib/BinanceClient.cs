using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CryptoClientLib.Commons;
using Newtonsoft.Json;

namespace CryptoClientLib
{
    public  class BinanceClient : ICryptoClient
    {
        private string publicKey;
        private string privateKey;
        public readonly Uri uri = new Uri("https://api.binance.com");
        private HttpClient client;

        public BinanceClient(string publicKey, string privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = privateKey;
            client = new HttpClient();
            client.BaseAddress = uri;
        }

        public string Timestamp
        {
	        get { return Convert.ToString(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()); }
        }

        public double GetPrice(Symbol symbol)
        {
	        var path = "api/v3/ticker/price?";
	        //var query = "symbol="+ ExtensionMethods.ToString(symbol);

	        //var result = SendRequest(path, query);

	        symbol.ToString();

	        return 0;
        }

        private string SendRequest(string path, string query, string signature = "", bool hasHeader=false)
        {
	        var uriReq = new UriBuilder(uri.ToString() + path + query + signature);

	        var request = new HttpRequestMessage()
	        {
		        RequestUri = uriReq.Uri,
		        Method = HttpMethod.Post,
	        };

	        if (hasHeader)
	        {
		        request.Headers.Add("X-MBX-APIKEY", publicKey);
            }

			var response = client.SendAsync(request);
			response.Wait();
			var httpContent = response.Result.Content.ReadAsStringAsync();
			httpContent.Wait();

			return httpContent.Result;
		}

        private string BuildQuery(Symbol symbol, Side side = Side.Undefined,
        string quantity = "", string expectedPrice = "", OrderType orderType = OrderType.Undefined)
        {
			//ListOfPairs strings
	        StringBuilder query = new StringBuilder("symbol=", 300);
	        //query.Append(ExtensionMethods.ToString(symbol));
	        if (side != Side.Undefined)
	        {
		        query.Append("&side=");
		        ExtensionMethods.ToString(side);
	        }

	        if (orderType != OrderType.Undefined)
	        {
		        query.Append("&type=");
		        ExtensionMethods.ToString(orderType);

		        query.Append("&timeInForce=GTC");

		        query.Append("&quantity=");
		        query.Append(quantity);

		        if (orderType == OrderType.LIMIT)
		        {
			        query.Append("&price=");
			        query.Append(expectedPrice);
				}

		        query.Append("&newOrderRespType=RESULT&recvWindow=8000");
		        query.Append("&timestamp=");
		        query.Append(Timestamp);
	        }

	        return query.ToString();
        }

        private string GetSignature(string query)
        {
            return "&signature=" + StringUtilities.ComputeHMacSha256(query, privateKey); //addtolistofpairs
        }

        public Task<OrderData> CreateOrder(OrderType orderType, Symbol symbol, Side side, 
	        double quantity = 0, double expectedPrice = 0)
        {
	        var path = "api/v3/order?";
	        string query = "";

	        Dictionary<string, string> listOfPairs = new Dictionary<string, string>();

	        if (orderType == OrderType.LIMIT)
	        {
		        query = BuildQuery(symbol, side, "0.00001", "10.00000", orderType);
		        listOfPairs.Add("symbol", symbol.ToString());

			}
	        else
	        {
		        
	        }

	        var signature = GetSignature(query);

	        var result = SendRequest(path, query, signature, true);

	        return null;
        }
    }
}
