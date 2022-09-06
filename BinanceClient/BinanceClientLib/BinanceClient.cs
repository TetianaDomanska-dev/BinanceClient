using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
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
	        Dictionary<string, string> listOfPairs = new Dictionary<string, string>();
	        listOfPairs.Add("symbol", symbol.ToString());

	        var result = SendRequest(path, BuildQuery(listOfPairs));

	        return 0;
        }

        private string SendRequest(string path, string query, bool hasHeader=false)
        {
	        var uriReq = new UriBuilder(uri.ToString() + path + query);

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

        private string BuildQuery(Dictionary<string, string> listOfPairs)
        {
			StringBuilder query = new StringBuilder();

			bool needSignature = false;
			foreach (var pair in listOfPairs)
			{
				if (pair.Key == "signature")
				{
					needSignature = true;
				}
				else
				{
					query.Append("&");
					query.Append(pair.Key);
					query.Append("=");
					query.Append(pair.Value);
				}
			}

			query.Remove(0, 1);
			query.Remove(query.Length - 1, 1);

			if (needSignature)
			{
				var signature = GetSignature(query.ToString());
				query.Append("&");
				query.Append("signature");
				query.Append("=");
				query.Append(signature);
			}

			return query.ToString();
        }

        private string GetSignature(string query)
        {
            return StringUtilities.ComputeHMacSha256(query, privateKey);
        }

        public Task<OrderData> CreateOrder(OrderType orderType, Symbol symbol, Side side, 
	        double quantity = 0, double expectedPrice = 0)
        {
	        var path = "api/v3/order?";
	        string query = "";

	        Dictionary<string, string> listOfPairs = new Dictionary<string, string>();

	        if (orderType == OrderType.LIMIT)
	        {
		        listOfPairs.Add("symbol", symbol.ToString());
		        listOfPairs.Add("side", ExtensionMethods.ToString(side));
		        listOfPairs.Add("type", ExtensionMethods.ToString(orderType));
		        listOfPairs.Add("timeInForce", "GTC");
				listOfPairs.Add("quantity", "0.00001");
				listOfPairs.Add("price", "10.00000");
				listOfPairs.Add("newOrderRespType", "RESULT");
				listOfPairs.Add("recvWindow", "5000");
				listOfPairs.Add("timestamp", Timestamp);
				listOfPairs.Add("signature", "");
	        }
	        else
	        {
		        
	        }

	        var result = SendRequest(path, BuildQuery(listOfPairs), true);

			return null;
        }
    }
}
