using Newtonsoft.Json;
using NiceHashMiner.Configs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;



namespace NiceHashMiner.Stats
{
    internal static class ExchangeRateApi
    {
        //private static string ApiUrl = Links.ApiUrl;

        private static readonly ConcurrentDictionary<string, double> ExchangesFiat = new ConcurrentDictionary<string, double>();
        private static double _usdBtcRate = -1;
        //public static double BTCcost = 1;
        //public static double BTCcost { get; set; }

        public static double UsdBtcRate
        {
            // Access in thread-safe way
            private get => Interlocked.Exchange(ref _usdBtcRate, _usdBtcRate);
            set
            {
                try
                {
                    if (value > 0)
                    {
                        Interlocked.Exchange(ref _usdBtcRate, value);
                        Helpers.ConsolePrint("NICEHASH", $"BTC rate updated: {value} USD");
                    }
                    if (value > 0 && value < 100)
                    {
                        Helpers.ConsolePrint("NICEHASH", "BTC rate error: " + value.ToString());
                        GetNewBTCRate();
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("UsdBtcRate-error", ex.Message);
                    return;
                }
            }
        }
        public static string ActiveDisplayCurrency = "USD";

        //   private static async void GetNewBTCRate()
        public static async void GetNewBTCRate()
        {
            string ResponseFromAPI;
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create(Links.exchangeRateList);
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 30 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 5 * 1000;
                StreamReader Reader = new StreamReader(SS);
                ResponseFromAPI = await Reader.ReadToEndAsync();
                Reader.Close();
                Response.Close();
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("API-error", ex.Message);
                return;
            }


            try
            {
                dynamic resp = JsonConvert.DeserializeObject(ResponseFromAPI);
                if (resp != null)
                {
                    var er = resp.list;

                    foreach (var pair in er)
                    {
                        if (pair.fromCurrency == "BTC" && pair.toCurrency == "USD")
                        {
                            //Helpers.ConsolePrint("API:", pair.exchangeRate.ToString());
                            var sBTCcost = pair.exchangeRate.ToString();


                            double.TryParse(sBTCcost, NumberStyles.Number, CultureInfo.InvariantCulture, out double BTCcost);
                            Interlocked.Exchange(ref _usdBtcRate, BTCcost);
                            Helpers.ConsolePrint("NICEHASH", $"USD rate updated: {sBTCcost} ");
                            //BTCcost = pair.exchangeRate;

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("API-error", ex.Message);
            }
            return;
        }
        private static bool ConverterActive => ConfigManager.GeneralConfig.DisplayCurrency != "USD";

        public static void UpdateExchangesFiat(Dictionary<string, double> newExchanges)
        {
            try
            {
                if (newExchanges == null) return;
                foreach (var key in newExchanges.Keys)
                {
                    ExchangesFiat.AddOrUpdate(key, newExchanges[key], (k, v) => newExchanges[k]);
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("API-error", ex.Message);
            }
        }

        public static double ConvertToActiveCurrency(double amount)
        {
            if (!ConverterActive)
            {
                return amount;
            }

            // if we are still null after an update something went wrong. just use USD hopefully itll update next tick
            if (ExchangesFiat.Count == 0 || ActiveDisplayCurrency == "USD")
            {
                return amount;
            }

            //Helpers.ConsolePrint("CurrencyConverter", "Current Currency: " + ConfigManager.Instance.GeneralConfig.DisplayCurrency);
            if (ExchangesFiat.TryGetValue(ActiveDisplayCurrency, out var usdExchangeRate))
                return amount * usdExchangeRate;

            Helpers.ConsolePrint("CurrencyConverter", "Unknown Currency Tag: " + ActiveDisplayCurrency + " falling back to USD rates");
            ActiveDisplayCurrency = "USD";
            return amount;
        }

        public static double GetUsdExchangeRate()
        {
            return UsdBtcRate > 0 ? UsdBtcRate : 0.0;
        }

        /// <summary>
        /// Get price of kW-h in BTC if it is set and exchanges are working
        /// Otherwise, returns 0
        /// </summary>
        public static double GetKwhPriceInBtc()
        {
            var price = Form_Main.GetKwhPrice();
            if (price <= 0) return 0;
            // Converting with 1/price will give us 1/usdPrice
            var invertedUsdRate = ConvertToActiveCurrency(1 / price);
            if (invertedUsdRate <= 0)
            {
                // Should never happen, indicates error in ExchangesFiat
                // Fall back with 0
                Helpers.ConsolePrint("EXCHANGE", "Exchange for currency is 0, power switching disabled.");
                return 0;
            }
            // Make price in USD
            price = 1 / invertedUsdRate;
            // Race condition not a problem since UsdBtcRate will never update to 0
            if (UsdBtcRate <= 0)
            {
                //                Helpers.ConsolePrint("EXCHANGE", "Bitcoin price is unknown, power switching disabled");
                return 0;
            }
            return price / UsdBtcRate;
        }


    }
}
