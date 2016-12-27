using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.Common
{
    public interface IConfigGateway
    {
        T GetAppSetting<T>(string item);
        T GetAppSetting<T>(string item, T defaultValue);
        ConnectionStringSettings GetConnectionString(string item);
        T GetSection<T>(string sectionName);
    }
}
