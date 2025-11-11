//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace OURSTORE.Localization
//{
//    internal class LocalizationResourceManager
//    {
//    }
//}

using System.Globalization;
using System.Resources;
using OURSTORE.Localization;

namespace OURSTORE.Localization
{
    public class LocalizationResourceManager
    {
        private readonly ResourceManager _resourceManager;

        public LocalizationResourceManager(Type resourceType)
        {
            _resourceManager = new ResourceManager(resourceType);
        }

        public void SetCulture(string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        public string GetValue(string key)
        {
            try
            {
                return _resourceManager.GetString(key, CultureInfo.CurrentUICulture);
            }
            catch
            {
                return key;
            }
        }
    }
}
