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
using Newtonsoft.Json.Linq;

namespace CryptoClientLib
{
    public class BinanceClient : ICryptoClient
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

        public string GetTimestampByDate(DateTime date)
        {
	        return Convert.ToString(new DateTimeOffset(date).ToUnixTimeMilliseconds()); 
        }

        private UInt64 GetMillisecondsByInterval(string interval)
        {
	        UInt64 milisecInSec = 1000;
	        UInt64 secInMin = 60;
	        UInt64 minInHour = 60;

	        var candleInterval = interval.Remove(0, interval.Length - 1);
			var intervalCount = Convert.ToUInt64(interval.Remove(interval.Length - 1));
			switch (candleInterval)
	        {
				case "s":
					return intervalCount * milisecInSec;
				case "m":
					return intervalCount * secInMin * milisecInSec;
				case "h":
					return intervalCount * minInHour * secInMin* milisecInSec;
	        }

			return 0;
        }

        public async Task<double> GetPrice(Symbol symbol)
        {
	        var path = "api/v3/ticker/price?";
	        Dictionary<string, string> listOfPairs = new Dictionary<string, string>();
	        listOfPairs.Add("symbol", symbol.ToString());

	        var result = await SendRequest(path, BuildQuery(listOfPairs), HttpMethod.Get);
	        var jsonResult = JObject.Parse(result);

	        //TO DO logging in separate class
	        Console.WriteLine("GetPrice: ");
	        foreach (var j in jsonResult)
	        {
		        Console.WriteLine(j.Key + " " + j.Value);
	        }

	        Console.WriteLine("-----------------------------------------");

	        if (jsonResult["price"] != null)
	        {
		        return Convert.ToDouble(jsonResult["price"]);
	        }

	        throw new Exception(Convert.ToString(jsonResult["msg"]));
        }

        private async Task<string> SendRequest(string path, string query, HttpMethod httpMethod, bool hasHeader=false)
        {
	        var uriReq = new UriBuilder(uri.ToString() + path + query);

	        var request = new HttpRequestMessage()
	        {
		        RequestUri = uriReq.Uri,
		        Method = httpMethod,
	        };

	        if (hasHeader)
	        {
		        request.Headers.Add("X-MBX-APIKEY", publicKey);
            }

			var response = await client.SendAsync(request);
			var httpContent = await response.Content.ReadAsStringAsync();
			return httpContent;
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

        public async Task<List<KlineData>> GetKlines(Symbol symbol, string interval, UInt64 countOfCandles)
        {
	        var path = "api/v3/klines?";
	        UInt64 limitMax = 1000;
	        UInt64 limitDefault = 500;

			UInt64 milliseconds = limitDefault * GetMillisecondsByInterval(interval);
	        var endDate = Timestamp;

	        List<string> candlesResultsList = new List<string>();
	        List<KlineData> candlesResultsListInKlineDataFormat = new List<KlineData>();

			for (UInt64 i = 0; i < countOfCandles; i+=limitMax)
	        {
		        var startDate = Convert.ToString(Convert.ToUInt64(endDate) - milliseconds);

		        Dictionary<string, string> listOfPairs = new Dictionary<string, string>();
		        listOfPairs.Add("symbol", symbol.ToString());
		        listOfPairs.Add("interval", interval);
		        listOfPairs.Add("startTime", startDate);
				listOfPairs.Add("endTime", endDate);
		        listOfPairs.Add("limit", "1000");

		        var result = await SendRequest(path, BuildQuery(listOfPairs), HttpMethod.Get);
		        candlesResultsList.Add(result);
				endDate = startDate;
	        }

			foreach (var candle in candlesResultsList)
			{
				var jsonResult = JArray.Parse(candle);
				foreach (var j in jsonResult)
				{
					candlesResultsListInKlineDataFormat.Add(new KlineData()
					{
						OpenTime = Convert.ToString(j[0]),
						OpenPrice = Convert.ToDouble(j[1]),
						HighPrice = Convert.ToDouble(j[2]),
						LowPrice = Convert.ToDouble(j[3]),
						ClosePrice = Convert.ToDouble(j[4]),
						CloseTime = Convert.ToString(j[5])
					});
				}
			}

			//TO DO logging in separate class
			foreach (var candle in candlesResultsList)
			{
				var jsonResult = JArray.Parse(candle);
				foreach (var j in jsonResult)
				{
					Console.WriteLine("[");
					Console.WriteLine("Kline open time " + j[0]);
					Console.WriteLine("Open price " + j[1]);
					Console.WriteLine("High price " + j[2]);
					Console.WriteLine("Low price " + j[3]);
					Console.WriteLine("Close price " + j[4]);
					Console.WriteLine("],");
				}
			}
			Console.WriteLine("-----------------------------------------");

			return candlesResultsListInKlineDataFormat;
        }

        public async Task<OrderData> CreateOrder(OrderType orderType, Symbol symbol, Side side,
	        double quantity = 0, double expectedPrice = 0)
        {
	        var path = "api/v3/order?";

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
		        listOfPairs.Add("symbol", symbol.ToString());
		        listOfPairs.Add("side", ExtensionMethods.ToString(side));
		        listOfPairs.Add("type", ExtensionMethods.ToString(orderType));
		        listOfPairs.Add("timeInForce", "GTC");
		        listOfPairs.Add("quantity", "0.00001");
		        listOfPairs.Add("newOrderRespType", "RESULT");
		        listOfPairs.Add("recvWindow", "10000");
		        listOfPairs.Add("timestamp", Timestamp);
		        listOfPairs.Add("signature", "");
	        }

	        var result = await SendRequest(path, BuildQuery(listOfPairs), HttpMethod.Post, true);

	        List<OrderData> orderResults = new List<OrderData>();
	        var jsonResult = JObject.Parse(result);
	        //TO DO logging in separate class
	        Console.WriteLine("CreateOrder: ");
	        foreach (var j in jsonResult)
	        {
		        Console.WriteLine(j.Key + " " + j.Value);
	        }
	        Console.WriteLine("-----------------------------------------");

			return new OrderData()
	        {
		        Symbol = symbol,
		        OrderType = orderType,
		        Quantity = Convert.ToDouble(jsonResult["quantity"]),
		        Price = Convert.ToDouble(jsonResult["price"]),
		        Timestamp = Convert.ToString(jsonResult["timestamp"])
	        };
        }

        public async Task<OrderData> DeleteOrder(string orderId, Symbol symbol)
		{
			var path = "api/v3/order?";

			Dictionary<string, string> listOfPairs = new Dictionary<string, string>();
			listOfPairs.Add("symbol", symbol.ToString());
			listOfPairs.Add("orderId", "???");
			listOfPairs.Add("recvWindow", "5000");
			listOfPairs.Add("timestamp", Timestamp);

			var result = await SendRequest(path, BuildQuery(listOfPairs), HttpMethod.Post, true);

			return null;
		}
	}
}
