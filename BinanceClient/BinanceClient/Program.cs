using BinanceClient;
using CryptoClientLib;
using CryptoClientLib.Commons;

ICryptoCurrencyCreator cryptoClientCreator = new BinanceClientCreator();

// Use your own public and private keys instead of empty strings
// Never push them to repo 
string publicKey = string.Empty;
string privateKey = string.Empty;

var cryptoClient = cryptoClientCreator.CreateCryptoClient(publicKey, privateKey);


// Examples of using methods from lib below

//var price = cryptoClient.GetPrice(new Symbol(Asset.BTC, Asset.USDT));
//var createOrder = cryptoClient.CreateOrder(OrderType.LIMIT, new Symbol(Asset.BTC, Asset.USDT), Side.BUY);
var klines = cryptoClient.GetKlines(new Symbol(Asset.BTC, Asset.USDT), "15m", 2000);

// You can debug and track asyncronous processes with using of variable and loop below
// START!!! ONLY FOR DEBUGGING
int i = 0;

while (true)
{
	i++;
}

// END!!! ONLY FOR DEBUGGING