using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HuobiPro_Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var driver = new HuobiPro();

            var currencies = await driver.CurrenciesAsync() as List<Currency>;
            if (currencies != null)
                foreach (Currency currency in currencies)
                {
                    Console.WriteLine($"Currency - {currency.BrifName}\t{currency.FullName}\t{currency.Id}");
                }

            var pairSettings = await driver.PairSettingsAsync() as List<PairSettings>;
            if (pairSettings != null)
                foreach (PairSettings pairSetting in pairSettings)
                {
                    Console.WriteLine($"PairSettings - {pairSetting.Pair.Name}\t{pairSetting.MinAmount}\t{pairSetting.MaxAmount}");
                }

            var pair = new Pair
            {
                Currency1 = new Currency {BrifName = "btc"},
                Currency2 = new Currency {BrifName = "usdt"}
            };
            var trades = await driver.TradesAsync(pair) as List<Trade>;
            if (trades != null)
                foreach (Trade trade in trades)
                {
                    Console.WriteLine($"Trade - {trade.Pair.Name}\t{trade.TradeTypes}\t{trade.Price}");
                }
        }
    }
}
