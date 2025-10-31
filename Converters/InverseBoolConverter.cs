//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace OURSTORE.Converters
//{
//    internal class InverseBoolConverter
//    {
//    }
//}


using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiStoreApp.Converters
{
    /// <summary>
    /// Converter لعكس القيم المنطقية (true ↔ false)
    /// </summary>
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }
    }
}


