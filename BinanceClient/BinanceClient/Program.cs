// See https://aka.ms/new-console-template for more information

using CryptoClientLib;
using CryptoClientLib.Commons;

var publicKey = "anWWYf99W4QbJ75fW3EDGDX3tESnO1x5d9AQ0uePjNKroiT2BzFHHywdUJcgopAp";

var privateKey = "TW5JPW2nMiSA7y6JXpIq7hByHKmlFnAws7r3e4GBEOWGCQq3chlWUgkv12HBXB4J";
BinanceClient client = new BinanceClient(publicKey, privateKey);
var t = client.GetPrice(new Symbol(Asset.BTC, Asset.USDT));