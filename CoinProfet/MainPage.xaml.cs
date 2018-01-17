using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.Data.Json;
using System.Text.RegularExpressions;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CoinProfet
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Coin BTC;
        public MainPage()
        {
            this.InitializeComponent();  
        }

        string currentDateFormatter()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
        string UpBitAPIgenerator(Coin.CoinType coin,int duration, string currentdate,int amount=1)
        {
            //label.Text = currentDateFormatter();
            return "https://crix-api-endpoint.upbit.com/v1/crix/candles/minutes/" + duration + "?code=CRIX.UPBIT.KRW-" + coin.ToString() + "&count=1";
        }

        async Task loadCoinData(Coin.CoinType coinType,int duration)
        {
            var Link = UpBitAPIgenerator(coinType, duration, currentDateFormatter());
            var client = new HttpClient();
            var c = new Coin();
            HttpResponseMessage response = await client.GetAsync(new Uri(Link));
            
            string jsonString = await response.Content.ReadAsStringAsync();
            jsonString = jsonString.Replace(@"\", " ");
            try { 
                JsonArray root = JsonValue.Parse(jsonString).GetArray();
                for (uint i = 0; i < root.Count; i++)
                {
                    Candle candle = new Candle();
                    candle.code = root.GetObjectAt(i).GetNamedString("code");
                    candle.candleDateTime = DateTime.Parse(root.GetObjectAt(i).GetNamedString("candleDateTime"));
                    candle.tradePrice = root.GetObjectAt(i).GetNamedNumber("tradePrice");
                    c.candles.Add(candle);
                }
                Coin.coins.Add(c);
            }
            catch(Exception ex)
            {
                logger.Text += ex.ToString();
            }
            if(coinType== Coin.CoinType.BTC)
                tradePrice.Text = Coin.coins[(int)Coin.CoinType.BTC].candles.Last().tradePrice.ToString();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try {
                foreach(Coin.CoinType coinType in Enum.GetValues(typeof(Coin.CoinType))) { 
                    await loadCoinData(coinType, 10);
                }
            }
            catch (Exception exception)
            {
                //logger.Text = exception.ToString();
            }
        }

    }
}
