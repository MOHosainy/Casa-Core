//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace OURSTORE.Converters
//{
//    class IntToBoolConverter
//    {
//    }
//}



using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiStoreApp.Converters
{
    
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
                return count > 0; // يظهر إذا السلة فيها عناصر
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
