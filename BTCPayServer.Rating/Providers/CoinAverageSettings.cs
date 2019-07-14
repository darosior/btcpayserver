﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BTCPayServer.Services.Rates
{
    public class CoinAverageSettingsAuthenticator : ICoinAverageAuthenticator
    {
        CoinAverageSettings _Settings;
        public CoinAverageSettingsAuthenticator(CoinAverageSettings settings)
        {
            _Settings = settings;
        }
        public Task AddHeader(HttpRequestMessage message)
        {
            return _Settings.AddHeader(message);
        }
    }

    public class CoinAverageExchange
    {
        public CoinAverageExchange(string name, string display, string url)
        {
            Name = name;
            Display = display;
            Url = url;
        }
        public string Name { get; set; }
        public string Display { get; set; }
        public string Url
        {
            get;
            set;
        }
    }
    public class CoinAverageExchanges : Dictionary<string, CoinAverageExchange>
    {
        public CoinAverageExchanges()
        {
        }

        public void Add(CoinAverageExchange exchange)
        {
            if (!TryAdd(exchange.Name, exchange))
            {
                this.Remove(exchange.Name);
                this.Add(exchange.Name, exchange);
            }
        }
    }
    public class CoinAverageSettings : ICoinAverageAuthenticator
    {
        private static readonly DateTime _epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public (String PublicKey, String PrivateKey)? KeyPair { get; set; }
        public CoinAverageExchanges AvailableExchanges { get; set; } = new CoinAverageExchanges();

        public CoinAverageSettings()
        {
            //GENERATED BY:
            //StringBuilder b = new StringBuilder();
            //b.AppendLine("_coinAverageSettings.AvailableExchanges = new[] {");
            //foreach (var availableExchange in _coinAverageSettings.AvailableExchanges)
            //{
            //    b.AppendLine($"(DisplayName: \"{availableExchange.DisplayName}\", Name: \"{availableExchange.Name}\"),");
            //}
            //b.AppendLine("}.ToArray()");
            AvailableExchanges = new CoinAverageExchanges();
            foreach (var item in
             new[] {
                (DisplayName: "Idex", Name: "idex"),
                (DisplayName: "Coinfloor", Name: "coinfloor"),
                (DisplayName: "Okex", Name: "okex"),
                (DisplayName: "Bitfinex", Name: "bitfinex"),
                (DisplayName: "Bittylicious", Name: "bittylicious"),
                (DisplayName: "BTC Markets", Name: "btcmarkets"),
                (DisplayName: "Kucoin", Name: "kucoin"),
                (DisplayName: "IDAX", Name: "idax"),
                (DisplayName: "Kraken", Name: "kraken"),
                (DisplayName: "Bit2C", Name: "bit2c"),
                (DisplayName: "Mercado Bitcoin", Name: "mercado"),
                (DisplayName: "CEX.IO", Name: "cex"),
                (DisplayName: "Bitex.la", Name: "bitex"),
                (DisplayName: "Quoine", Name: "quoine"),
                (DisplayName: "Stex", Name: "stex"),
                (DisplayName: "CoinTiger", Name: "cointiger"),
                (DisplayName: "Poloniex", Name: "poloniex"),
                (DisplayName: "Zaif", Name: "zaif"),
                (DisplayName: "Huobi", Name: "huobi"),
                (DisplayName: "QuickBitcoin", Name: "quickbitcoin"),
                (DisplayName: "Tidex", Name: "tidex"),
                (DisplayName: "Tokenomy", Name: "tokenomy"),
                (DisplayName: "Bitcoin.co.id", Name: "bitcoin_co_id"),
                (DisplayName: "Kryptono", Name: "kryptono"),
                (DisplayName: "Bitso", Name: "bitso"),
                (DisplayName: "Korbit", Name: "korbit"),
                (DisplayName: "Yobit", Name: "yobit"),
                (DisplayName: "BitBargain", Name: "bitbargain"),
                (DisplayName: "Livecoin", Name: "livecoin"),
                (DisplayName: "Hotbit", Name: "hotbit"),
                (DisplayName: "Coincheck", Name: "coincheck"),
                (DisplayName: "Binance", Name: "binance"),
                (DisplayName: "Bit-Z", Name: "bitz"),
                (DisplayName: "Coinbase Pro", Name: "coinbasepro"),
                (DisplayName: "Rock Trading", Name: "rocktrading"),
                (DisplayName: "Bittrex", Name: "bittrex"),
                (DisplayName: "BitBay", Name: "bitbay"),
                (DisplayName: "Tokenize", Name: "tokenize"),
                (DisplayName: "Hitbtc", Name: "hitbtc"),
                (DisplayName: "Upbit", Name: "upbit"),
                (DisplayName: "Bitstamp", Name: "bitstamp"),
                (DisplayName: "Luno", Name: "luno"),
                (DisplayName: "Trade.io", Name: "tradeio"),
                (DisplayName: "LocalBitcoins", Name: "localbitcoins"),
                (DisplayName: "Independent Reserve", Name: "independentreserve"),
                (DisplayName: "Coinsquare", Name: "coinsquare"),
                (DisplayName: "Exmoney", Name: "exmoney"),
                (DisplayName: "Coinegg", Name: "coinegg"),
                (DisplayName: "FYB-SG", Name: "fybsg"),
                (DisplayName: "Cryptonit", Name: "cryptonit"),
                (DisplayName: "BTCTurk", Name: "btcturk"),
                (DisplayName: "bitFlyer", Name: "bitflyer"),
                (DisplayName: "Negocie Coins", Name: "negociecoins"),
                (DisplayName: "OasisDEX", Name: "oasisdex"),
                (DisplayName: "CoinMate", Name: "coinmate"),
                (DisplayName: "BitForex", Name: "bitforex"),
                (DisplayName: "Bitsquare", Name: "bitsquare"),
                (DisplayName: "FYB-SE", Name: "fybse"),
                (DisplayName: "itBit", Name: "itbit"),
                })
            {
                AvailableExchanges.TryAdd(item.Name, new CoinAverageExchange(item.Name, item.DisplayName, $"https://apiv2.bitcoinaverage.com/exchanges/{item.Name}"));
            }
            // Keep back-compat
            AvailableExchanges.Add(new CoinAverageExchange("gdax", string.Empty, $"https://apiv2.bitcoinaverage.com/exchanges/coinbasepro"));
        }

        public Task AddHeader(HttpRequestMessage message)
        {
            var signature = GetCoinAverageSignature();
            if (signature != null)
            {
                message.Headers.Add("X-signature", signature);
            }
            return Task.CompletedTask;
        }

        public string GetCoinAverageSignature()
        {
            var keyPair = KeyPair;
            if (!keyPair.HasValue)
                return null;
            if (string.IsNullOrEmpty(keyPair.Value.PublicKey) || string.IsNullOrEmpty(keyPair.Value.PrivateKey))
                return null;
            var timestamp = (int)((DateTime.UtcNow - _epochUtc).TotalSeconds);
            var payload = timestamp + "." + keyPair.Value.PublicKey;
            var digestValueBytes = new HMACSHA256(Encoding.ASCII.GetBytes(keyPair.Value.PrivateKey)).ComputeHash(Encoding.ASCII.GetBytes(payload));
            var digestValueHex = NBitcoin.DataEncoders.Encoders.Hex.EncodeData(digestValueBytes);
            return payload + "." + digestValueHex;
        }
    }
}