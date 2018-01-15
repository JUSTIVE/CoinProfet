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
        string UpBitAPIgenerator(Coin.CoinType coin,string duration, string currentdate)
        {
            //label.Text = currentDateFormatter();
            return "https://crix-api-endpoint.upbit.com/v1/crix/candles/minutes/" + duration + "?code=CRIX.UPBIT.KRW-" + coin.ToString() + "&count=1";
        }

        async Task LoadWebAsync()
        {
            var Link = @UpBitAPIgenerator(Coin.CoinType.BTC, "10", currentDateFormatter());
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(new Uri(Link));
            
            var jsonString = await response.Content.ReadAsStringAsync();
            JsonArray root = JsonValue.Parse(jsonString).GetArray();
            for(uint i = 0;i<root.Count;i++)
            {
                BTC.candles.Last().code = root.GetObjectAt(i).GetNamedString("code");
                BTC.candles.Last().candleDateTime = DateTime.Parse(root.GetObjectAt(i).GetNamedString("candleDateTime"));
                BTC.candles.Last().tradePrice = double.Parse(root.GetObjectAt(i).GetNamedString("tradePrice"));
            }
            
            label.Text = BTC.candles.Last().tradePrice.ToString();
        }

        private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
        {

            await LoadWebAsync();
            
        }
        
    }
}
