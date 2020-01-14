using System;
using Newtonsoft.Json;

namespace HuobiPro_Demo
{
    public struct Currency
    {
        public string BrifName;
        public string FullName;
        public Guid Id;
    }

    internal struct CurrencyJson
    {
        public string status;
        public string[] data;
    }

    public struct PairSettings
    {
        public Pair Pair;
        public decimal MinQuantity;
        public decimal MaxQuantity;
        public decimal MinPrice;
        public decimal MaxPrice;
        public decimal MinAmount;
        public decimal MaxAmount;
    }

    public struct Pair
    {
        public Currency Currency1;
        public Currency Currency2;
        public string Name;
        public Guid Id;

        public string GetSystemName(string splitter)
        {
            return Currency1.BrifName + splitter + Currency2.BrifName;
        }
    }

    internal struct PairSetting
    {
        public string status;
        public Datas[] data;

        internal struct Datas
        {
            public string symbol;
            public string state;
            [JsonProperty("base-currency")] public string baseCurrency;
            [JsonProperty("quote-currency")] public string quoteCurrency;
            [JsonProperty("price-precision")] public decimal pricePrecision;
            [JsonProperty("amount-precision")] public decimal amountPrecision;
            [JsonProperty("symbol-partition")] public string symbolPartition;
            [JsonProperty("value-precision")] public decimal valuePrecision;
            [JsonProperty("min-order-amt")] public decimal minOrderAmt;
            [JsonProperty("max-order-amt")] public decimal maxOrderAmt;
            [JsonProperty("min-order-value")] public decimal minOrderValue;
            [JsonProperty("leverage-ratio")] public decimal leverageRatio;
        }
    }

    internal struct TradeJson
    {
        public string status;
        public string ch;
        public long ts;
        public Datas[] data;

        internal struct Datas
        {
            public long id;
            public long ts;
            public Data2[] data;
        }

        internal struct Data2
        {
            public decimal amount;
            [JsonProperty("trade-id")] public string tradeId;
            public long ts;
            public string id;
            public decimal price;
            public string direction;
        }
    }

    public struct Trade
    {
        public string TradeId;
        public decimal Price;
        public decimal Quantity;
        public decimal Amount;
        public DateTime Date;
        public TradeTypes TradeTypes;
        public Pair Pair;
    }

    public enum TradeTypes
    {
        Sell,
        Buy
    }
}
