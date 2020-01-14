using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HuobiPro_Demo
{
    [TestClass]
    public class HuobiProTest
    {
        [TestMethod]
        public async Task TestCurrencies()
        {
            var driver = new HuobiPro();

            var result = await driver.CurrenciesAsync() as List<Currency>;

            Assert.IsNotNull(result);
            Assert.AreEqual(255, result.Count);
        }

        [TestMethod]
        public async Task TestPairSettings()
        {
            var driver = new HuobiPro();

            var result = await driver.PairSettingsAsync() as List<PairSettings>;

            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Count);
        }

        [TestMethod]
        public async Task TestTrades()
        {
            var driver = new HuobiPro();

            var result = await driver.TradesAsync(new Pair
            {
                Currency1 = new Currency {BrifName = "btc"},
                Currency2 = new Currency {BrifName = "usdt"}
            }) as List<Trade>;

            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Count);
        }
    }
}
