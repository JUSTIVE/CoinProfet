using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinProfet
{
    public class Candle
    {
        public string code { get; set; }
        public DateTime candleDateTime { get; set; }
        public DateTime candleDateTimeKst { get; set; }
        public double OpeningPrice { get; set; }
        public double HighPrice { get; set; }
        public double lowPrice { get; set; }
        public double tradePrice { get; set; }
        public double candleAccTradeVolume { get; set; }
        public double candleAccTradePrice { get; set; }
        public ulong timestamp { get; set; }
        public int unit { get; set; }
    }
}
