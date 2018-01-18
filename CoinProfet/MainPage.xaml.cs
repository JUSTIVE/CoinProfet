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
using System.Collections.ObjectModel;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CoinProfet
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public CoinViewModel coinViewModel { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            Coin.initCoin();
            this.coinViewModel = new CoinViewModel(Coin.coins);
            
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
                Coin.coins[(int)coinType].deltaValue = (Math.Round((100 * (Coin.coins[(int)coinType].prevDayTradePrice - Coin.coins[(int)coinType].candles.Last().tradePrice))
                        / Coin.coins[(int)coinType].prevDayTradePrice, 2) * -1.0f);
                Coin.coins[(int)coinType].tradePrice = Coin.coins[(int)coinType].candles.Last().tradePrice;
            }
            catch(Exception ex)
            {
                logger.Text += ex.ToString();
            }
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (coinType == Coin.CoinType.SNT)
                {
                    coinFullName.Text = ((Coin.CoinFullName)((int)coinType)).ToString();
                    coinName.Text = Coin.coins[(int)coinType].name;
                    tradePrice.Text = Coin.coins[(int)coinType].candles.Last().tradePrice.ToString();
                    double deltaValue = (Math.Round((100 * (Coin.coins[(int)coinType].prevDayTradePrice - Coin.coins[(int)coinType].candles.Last().tradePrice))
                        / Coin.coins[(int)coinType].prevDayTradePrice, 2) * -1.0f);
                    deltaPrevDay.Text = deltaValue.ToString("#0.#0") + "%";
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
            this.coinViewModel = new CoinViewModel(Coin.coins);
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
                    //coinViewModel.coins.Clear();
                    CoinListView.ItemsSource = coinViewModel.coins;
                    for (int i = 0; i < Coin.coins.Length; i++)
                        coinViewModel.coins[i]=(Coin.coins[i]);
                });
                
                await Task.Delay(150);
                
            }
            
        }

        private void TextBlock_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            logger.Text += ((string)args.NewValue) + "\n";

            if (double.Parse((string)args.NewValue) < 0)
                (sender as TextBlock).Foreground = new SolidColorBrush(Color.FromArgb(255, 33, 150, 243));
            else if (double.Parse((string)args.NewValue) == 0)
                (sender as TextBlock).Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            else
                (sender as TextBlock).Foreground = new SolidColorBrush(Color.FromArgb(255, 244, 67, 54));

        }
    }
}
