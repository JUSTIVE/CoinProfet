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
using Windows.ApplicationModel.Core;
using Windows.UI;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CoinProfet
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
        public MainPage()
        {
            this.InitializeComponent();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
        }

        public static string currentDateFormatter()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
        public static string UpBitAPIgenerator(Coin.CoinType coin,int duration, string currentdate,int amount=1)
        {
            //label.Text = currentDateFormatter();
            return "https://crix-api-endpoint.upbit.com/v1/crix/candles/minutes/" + duration + "?code=CRIX.UPBIT.KRW-" + coin.ToString() + "&count=1";
        }
        public static string UpBitAPIprevDayGenerator(Coin.CoinType coin)
        {
            return "https://crix-api-endpoint.upbit.com/v1/crix/candles/days/?code=CRIX.UPBIT.KRW-" + coin.ToString() + "&count=2";
        }

        async Task loadCoinData(Coin.CoinType coinType,int duration)
        {
            var Link = UpBitAPIgenerator(coinType, duration, currentDateFormatter());
            var client = new HttpClient();
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
                    Coin.coins[(int)coinType].candles.Add(candle);
                }
            }
            catch(Exception ex)
            {
                logger.Text += ex.ToString();
            }
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            { 
                if (coinType == Coin.CoinType.SNT) {
                    coinFullName.Text = ((Coin.CoinFullName)((int)coinType)).ToString();
                    coinName.Text = Coin.coins[(int)coinType].name;
                    tradePrice.Text = Coin.coins[(int)coinType].candles.Last().tradePrice.ToString();
                    double deltaValue = (Math.Round((100 * (Coin.coins[(int)coinType].prevDayTradePrice - Coin.coins[(int)coinType].candles.Last().tradePrice))
                        / Coin.coins[(int)coinType].prevDayTradePrice, 2) * -1.0f);
                    deltaPrevDay.Text = deltaValue.ToString("##.#0")+"%";
                    if (deltaValue < 0)
                        deltaPrevDay.Foreground = new SolidColorBrush(Color.FromArgb(255, 33, 150, 243));
                    else if (deltaValue == 0)
                        deltaPrevDay.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    else
                        deltaPrevDay.Foreground = new SolidColorBrush(Color.FromArgb(255, 244, 67, 54));

                }
            });
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Coin.initPrevDayTradePriceAsync();
            await Task.Run(() => UpdateInfo());
        }
        private async void UpdateInfo()
        {
            while (true)
            {
                try
                {
                    foreach (Coin.CoinType coinType in Enum.GetValues(typeof(Coin.CoinType)))
                    {
                        await loadCoinData(coinType, 10);
                    }
                }

                catch (Exception exception)
                {

                }
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                    //logger.Text += "running\n";
                });
                
                await Task.Delay(300);
            }
        }

    }
}
