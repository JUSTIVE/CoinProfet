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
        void LoadWebAsync()
        {
            string Link = UpBitAPIgenerator(Coin.CoinType.BTC, "10", currentDateFormatter());
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(Link);
            string labelText = "null";
            try { 
                HtmlNodeCollection nodecollection = doc.DocumentNode.SelectNodes("/html/body/pre");
            }
            catch (NullReferenceException) { 
            }
            label.Text = labelText;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWebAsync();
        }
    }
}
