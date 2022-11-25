using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using CryptoClientLib;
using CryptoClientLib.Commons;
using Newtonsoft.Json.Linq;

namespace CryptoClientIntegrationTests
{
	public class BinanceClientIntegrationTests
	{
		public readonly Uri uri = new Uri("https://api.binance.com");

		private BinanceClient binanceClient;
		private Symbol btcSymbol = new Symbol(Asset.BTC, Asset.USDT);

		// Use your own public and private keys
		// Never push them to repo 
		private readonly string publicKey = "";
		private readonly string privateKey = "";

		public BinanceClientIntegrationTests()
		{
			binanceClient = new BinanceClient(publicKey, privateKey);
		}

		[Fact]
		public async void GetPrice_BTCUSDT_ValidPrice()
		{
			// Act
			var price = await binanceClient.GetPrice(btcSymbol);

			// Assert
			Assert.True(price > 0);
		}

		[Fact]
		public async void GetPrice_BTCUSDT_CorrectBtcPriceWithTrashHold()
		{
			//Arrange
			var path = "api/v3/ticker/price?";
			var expectedResultWithTrashHold = await (Task.Factory.StartNew(async () =>
			{
				var uriReq = new UriBuilder(uri.ToString() + path + "symbol="+ btcSymbol.ToString());

				var request = new HttpRequestMessage()
				{
					RequestUri = uriReq.Uri,
					Method = HttpMethod.Get,
				};

				var httpClient = new HttpClient();
				var response = await httpClient.SendAsync(request);
				var httpContent = await response.Content.ReadAsStringAsync();
				return httpContent;
			}));
			var jsonExpectedResult = JObject.Parse(expectedResultWithTrashHold.Result);

			var epsilon = 10; 

			// Act
			var price = await binanceClient.GetPrice(btcSymbol);

			// Assert
			Assert.True(Math.Abs(price-Convert.ToDouble(jsonExpectedResult["price"])) <= epsilon);
		}

		[Fact]
		public async void GetPrice_InvalidSymbol_ReturnException()
		{
			// Arrange
			var invalidSymbol = new Symbol(Asset.Undefined, Asset.Undefined);

			// Act
			var exception = await Record.ExceptionAsync(() =>
				binanceClient.GetPrice(invalidSymbol));

			//Assert
			Assert.NotNull(exception);
			Assert.IsType<Exception>(exception);
		}
	}
}