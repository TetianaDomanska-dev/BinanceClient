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
        public const string uri = "https://api.binance.com";
        private HttpClient client;

        public BinanceClient(string publicKey, string privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = privateKey;
            client = new HttpClient();
            //client.BaseAddress = uri;
        }

        public string Timestamp
        {
	        get { return Convert.ToString(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()); }
        }

        public double GetPrice(Symbol symbol)
        {
	        var path = "api/v3/ticker/price?";
	        List<Tuple<string, string>> queryPairs = new List<Tuple<string, string>>();
	        queryPairs.Add(new Tuple<string, string>("symbol", symbol.ToString()));

	        var result = SendRequest(path, queryPairs, HttpMethod.Get);

	        return 0;
        }

        private string SendRequest(string path, List<Tuple<string, string>> queryPairs, HttpMethod method, 
	        bool needSignature = false, bool hasHeader = false) //signature depends on header
        {
	        var uriReq = new UriBuilder(uri + path + BuildQuery(queryPairs, needSignature));

	        var request = new HttpRequestMessage()
	        {
		        RequestUri = uriReq.Uri,
		        Method = method,
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

        private string BuildQuery(List<Tuple<string, string>> queryPairs, bool needSignature)
        {
			StringBuilder query = new StringBuilder();

			foreach (var pair in queryPairs)
			{
				query.Append("&");
				query.Append(pair.Item1);
				query.Append("=");
				query.Append(pair.Item2);
			}
			query.Remove(0, 1);

			if (needSignature)
			{
				var signature = ComputeSignature(query.ToString());
				query.Append("&");
				query.Append("signature");
				query.Append("=");
				query.Append(signature);
			}

			return query.ToString();
        }

        private string ComputeSignature(string query)
        {
            return StringUtilities.ComputeHMacSha256(query, privateKey);
        }

        public Task<OrderData> CreateOrder(OrderType orderType, Symbol symbol, Side side, 
	        double quantity = 0, double expectedPrice = 0)
        {
	        var path = "api/v3/order?";
	        string query = "";

	        List<Tuple<string, string>> queryPairs = new List<Tuple<string, string>>();

	        if (orderType == OrderType.LIMIT)
	        {
		        queryPairs.Add(new Tuple<string, string>("symbol", symbol.ToString()));
		        queryPairs.Add(new Tuple<string, string>("side", ExtensionMethods.ToString(side)));
		        queryPairs.Add(new Tuple<string, string>("type", ExtensionMethods.ToString(orderType)));
		        queryPairs.Add(new Tuple<string, string>("timeInForce", "GTC"));
				queryPairs.Add(new Tuple<string, string>("quantity", "0.00001"));
				queryPairs.Add(new Tuple<string, string>("price", "10.00000"));
				queryPairs.Add(new Tuple<string, string>("newOrderRespType", "RESULT"));
				queryPairs.Add(new Tuple<string, string>("recvWindow", "5000"));
				queryPairs.Add(new Tuple<string, string>("timestamp", Timestamp));

	        }
	        else
	        {
		        
	        }

	        var result = SendRequest(path, queryPairs, HttpMethod.Post, true, true);

			return null;
        }
    }
}
