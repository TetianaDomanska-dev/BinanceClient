// See https://aka.ms/new-console-template for more information

using CryptoClientLib;
using CryptoClientLib.Commons;

var publicKey = "";

var privateKey = "";
BinanceClient client = new BinanceClient(publicKey, privateKey);
var t = client.GetPrice(new Symbol(Asset.BTC, Asset.USDT));