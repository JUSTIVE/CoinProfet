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
            HttpResponseMessage response = await client.GetAsync(new Uri(Link));
            
            var jsonString = await response.Content.ReadAsStringAsync();
            JsonObject root = JsonObject.Parse(jsonString).GetObject();
            Coin c = new Coin();
            //for(uint i = 0;i<root.Count;i++)
            {
                c.candles.Last().code = root.GetNamedString("code");
                c.candles.Last().candleDateTime = DateTime.Parse(root.GetNamedString("candleDateTime"));
                c.candles.Last().tradePrice = double.Parse(root.GetNamedString("tradePrice"));
            }
            Coin.coins.Add(c);
            tradePrice.Text = BTC.candles.Last().tradePrice.ToString();
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
                tradePrice.Text = exception.ToString();
            }
        }

    }
}
