using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Data.Json;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace CoinProfet
{
    public class Coin
    {
        public static Coin[] coins = new Coin[34];
        public enum CoinType
        {
            SNT,ETC,NEO,BCC,ETH,  //5
            ZEC,XMR,BTC,BTG,WAVES, //5
            QTUM,LSK,STRAT,LTC,DASH,//5
            XRP,VTC,XLM,PIVX,ARK,//5
            XEM,STEEM,OMG,REP,MTL,//5
            ADA,GRS,SBD,KMD,STORJ,//5
            MER,POWR,EMC2,TIX//4
        }
        public enum CoinFullName
        {
            스테이터스네트워크토큰,
            이더리움클래식,
            네오,
            비트코인캐시,
            이더리움,
            지캐시,
            모네로,
            비트코인,
            비트코인골드,
            웨이브,
            퀀텀,
            리스크,
            스트라티스,
            라이트코인,
            대쉬,
            리플,
            버트코인,
            스텔라루멘,
            피벡스,
            아크,
            뉴이코노미무브먼트,
            스팀,
            오미세고,
            어거,
            메탈,
            에이다,
            그로스톨코인,
            스팀달러,
            코모도,
            스토리지,
            머큐리,
            파워렛져,
            아인스타이늄,
            블록틱스
        }
        public List<Candle> candles;
        //public double currentPrice;
        public string name { get; set; }
        public string fullName { get; set; }
        public double prevDayTradePrice { get; set; }
        public double tradePrice { get; set; }
        public double deltaValue { get; set; }
        public string imgres { get; set; }

        public static void initCoin()
        {
            for (int i = 0; i < 34; i++) { 
                coins[i] = new Coin((CoinType)i);
                coins[i].imgres = "Assets/coinImgres/" + ((CoinType)i).ToString()+".png";
            }
        }
        public static async void initPrevDayTradePriceAsync()
        {   
            for (int i = 0; i < 34; i++)
            {
                var Link = MainPage.UpBitAPIprevDayGenerator((CoinType)i);
                var client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(new Uri(Link));
                string jsonString = await response.Content.ReadAsStringAsync();
                jsonString = jsonString.Replace(@"\", " ");
                JsonArray root = JsonValue.Parse(jsonString).GetArray();
                coins[i].prevDayTradePrice = root.GetObjectAt(1).GetNamedNumber("tradePrice");
            }
        }
        public Coin()
        {
            candles = new List<Candle>();
        }
        public Coin(CoinType name):this()
        {   
            this.name = name.ToString();
            this.fullName = ((CoinFullName)(int)name).ToString();
        }
        
    }
    
}
