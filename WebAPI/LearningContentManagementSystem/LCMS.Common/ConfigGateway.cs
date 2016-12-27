using LCMS.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.Common
{
    public class ConfigGateway : IConfigGateway
    {
        public T GetAppSetting<T>(string item)
        {
            return Utilities.ConvertTo<T>(ConfigurationManager.AppSettings[item]);
        }
        public T GetAppSetting<T>(string item, T defaultValue)
        {
            return Utilities.ConvertTo<T>(ConfigurationManager.AppSettings[item], defaultValue);
        }
        public ConnectionStringSettings GetConnectionString(string item)
        {
            return ConfigurationManager.ConnectionStrings[item];
        }
        public T GetSection<T>(string sectionName)
        {
            return (T)((object)ConfigurationManager.GetSection(sectionName));
        }
    }
}
