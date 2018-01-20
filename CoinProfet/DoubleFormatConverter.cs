using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CoinProfet
{
    public class DoubleFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object param, string lang)
        {
            var ret = String.Format(CultureInfo.InvariantCulture, "{0:0,0}", value); ;
            if (ret == "00") ret = "0";
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object param, string lang)
        {
            return Double.Parse(value.ToString().Replace(",", ""));
        }
    }
}
