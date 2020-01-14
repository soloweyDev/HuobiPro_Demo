using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HuobiPro_Demo
{
    public class HuobiPro
    {
        private static readonly HttpClient Client = new HttpClient();

        /// <summary>
        /// Адрес API биржи
        /// </summary>
        public string UrlApi { get; } = "https://api.huobi.pro";

        private const string PairSplitter = "";

        public HuobiPro() { }

        /// <summary>
        /// Cписок валют биржи
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Currency>> CurrenciesAsync()
        {
            var currencies = new List<Currency>();

            HttpResponseMessage response = await Client.GetAsync($"{UrlApi}/v1/common/currencys");

            if (!CheckResponse(response, "HuobiPro")) return currencies;

            var jsonCurrency = await response.Content.ReadAsStringAsync();

            var symbols = JsonConvert.DeserializeObject<CurrencyJson>(jsonCurrency);

            if (symbols.data == null)
            {
                Console.WriteLine("Нет ни одной валюты биржи");
            }
            else
            {
                //Перебираем распарсеный лист валют, получая обеъкты "Валюты"
                foreach (string currency in symbols.data)
                {
                    currencies.Add(new Currency
                    {
                        BrifName = currency,
                        FullName = currency.ToUpper(),
                        Id = Guid.NewGuid()
                    });
                }
            }

            return currencies;
        }

        /// <summary>
        /// Получение настроек валютных пар
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PairSettings>> PairSettingsAsync()
        {
            var pairsSettings = new List<PairSettings>();

            HttpResponseMessage response = await Client.GetAsync($"{UrlApi}/v1/common/symbols");

            if (!CheckResponse(response, "HuobiPro")) return pairsSettings;

            var jsonPairSettings = await response.Content.ReadAsStringAsync();

            var parsedPairSettings = JsonConvert.DeserializeObject<PairSetting>(jsonPairSettings);

            if (parsedPairSettings.data == null)
            {
                Console.WriteLine("Нет настроек валютных пар");
            }
            else
            {
                foreach (var parsedPairSetting in parsedPairSettings.data)
                {
                    var pair = new Pair
                    {
                        Name = parsedPairSetting.symbol,
                        Currency1 = new Currency { BrifName = parsedPairSetting.baseCurrency },
                        Currency2 = new Currency { BrifName = parsedPairSetting.quoteCurrency },
                        Id = Guid.NewGuid()
                    };

                    pairsSettings.Add(new PairSettings
                    {
                        MaxAmount = parsedPairSetting.maxOrderAmt,
                        //MaxPrice = ,
                        //MaxQuantity = ,
                        MinAmount = parsedPairSetting.minOrderAmt,
                        MinPrice = parsedPairSetting.minOrderValue,
                        //MinQuantity = ,
                        Pair = pair
                    });
                }
            }

            return pairsSettings;
        }

        /// <summary>
        /// Запрос на список сделок по валютной паре
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Trade>> TradesAsync(Pair pair)
        {
            var trades = new List<Trade>();
            //TODO  Нужно этот хардкод заменить
            const int limit = 100;

            var stringPair = pair.GetSystemName(PairSplitter);

            HttpResponseMessage response = await Client.GetAsync($"{UrlApi}/market/history/trade?symbol={stringPair}&size={limit}");

            if (!CheckResponse(response, "HuobiPro")) return trades;

            var jsonTrades = await response.Content.ReadAsStringAsync();

            //Десерелизация сделок из json в .net структуру
            //https://www.newtonsoft.com/json/help/html/SerializingCollections.htm
            var tradesOnPair = JsonConvert.DeserializeObject<TradeJson>(jsonTrades);

            if (tradesOnPair.data == null)
            {
                Console.WriteLine($"Нет ни одной сделки по валютной паре {stringPair}");
            }
            else
            {
                //Перебрать все сделки по валютной паре
                foreach (var element in tradesOnPair.data)
                {
                    foreach (var oData in element.data)
                    {
                        trades.Add(new Trade
                        {
                            TradeId = oData.tradeId,
                            Price = oData.price,
                            Pair = pair,
                            Amount = oData.amount,
                            Quantity = 0,
                            Date = ConvertDateTime(oData.ts),
                            TradeTypes = oData.direction == "sell" ? TradeTypes.Sell : TradeTypes.Buy
                        });
                    }
                }
            }

            return trades;
        }

        /// <summary>
        /// Проверяет успешность http запроса
        /// </summary>
        /// <param name="response"></param>
        /// <param name="exchangeName"></param>
        protected bool CheckResponse(HttpResponseMessage response, string exchangeName)
        {
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Сервер биржи {exchangeName} возвращает ошибку: {response?.StatusCode ?? HttpStatusCode.BadRequest}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// С бирж приходят сведения о датах в различных исчислениях и их нужно приводить
        /// к DateTime. На разных биржах, это могут быть разные приведения.
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        protected virtual DateTime ConvertDateTime(long datetime)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(datetime);
        }
    }
}
