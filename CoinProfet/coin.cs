using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinProfet
{
    class Coin
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
        public List<Candle> candles;
        //public double currentPrice;
        public string name { get; set; }

        public Coin()
        {
            candles = new List<Candle>();
        }
        public Coin(string name):this()
        {   
            this.name = name;
        }
    }
}
