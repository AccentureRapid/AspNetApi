using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.Common.Datetime
{
    public static class DatetimeHelper
    {
        public static string GetTimeZoneIDByFacility(string facilityName)
        {
            string facilityTimeZoneMapXmlPath = System.Configuration.ConfigurationManager.AppSettings[Constants.TIME_ZONE_MAP_XML_PATH];
            string facilityTimeZoneMapXmlFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, facilityTimeZoneMapXmlPath);
            var document = System.Xml.Linq.XDocument.Load(facilityTimeZoneMapXmlFullPath);
            string timeZoneID = string.Empty;

            if (document != null)
            {
                IEnumerable<FacilityTimeZoneDTO> list = document.Descendants(Constants.TIME_ZONE_MAP_NODE).Select(node => new FacilityTimeZoneDTO()
                {
                    Facility = node.Element(Constants.TIME_ZONE_MAP_FACILITY).Value,
                    TimeZone = node.Element(Constants.TIME_ZONE_MAP_TIME_ZONE).Value,
                });

                if (list != null && list.Count() > 0)
                {
                    foreach (var map in list)
                    {
                        if (map.Facility.Equals(facilityName, StringComparison.OrdinalIgnoreCase))
                        {
                            timeZoneID = map.TimeZone;
                            break;
                        }
                    }
                }
            }

            return timeZoneID;
        }

        public static DateTime ConvertUTCtoLocal(DateTime utc, string timeZoneId)
        {
            // TODO: CUICK REPLACE START
            utc = SetUtcTime(utc);
            return System.TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, timeZoneId);
            // TODO: CUICK REPLACE END
        }

        // TODO: CUICK ADD START
        public static DateTime SetUtcTime(DateTime dt)
        {
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        }
        // TODO: CUICK ADD END
    }
}
