using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.Common
{
    public static class Constants
    {
        // Status
        public const string STATUS_OK = "OK";
        public const string STATUS_NG = "NG";
        public const string STATUS_SUCCESS = "Success";

        // Sharefolder
        public const string FOLDER_TEMP_FILES_TO_DOWNLOAD = "TempFilesToDownloadFolder";
        public const string DOWNLOAD_FILES_TO_SIMULATION_SHAREPOIN = "SharepointTempFolder";
        public static readonly DateTime DEFAULT_DATE_IN_DATABASE = new DateTime(1900, 1, 1);

        // Announcement
        public const string ANNOUNCEMENT_POST_TYPE = "announcement";
        public const string ANNOUNCEMENT_META_KEY_FACILITY_NAME = "facility_name";
        //public const string ANNOUNCEMENT_META_KEY_FACILITY_CODE = "facility_code";
        public const string ANNOUNCEMENT_META_KEY_START_DATE = "start_date_time";
        public const string ANNOUNCEMENT_META_KEY_END_DATE = "end_date_time";
        public const string ANNOUNCEMENT_META_KEY_ANNOUNCEMENT_DESCRIPTION = "announcement_description";
        public const string ANNOUNCEMENT_META_KEY_PRIORITY = "priority";
        public const string ANNOUNCEMENT_META_KEY_ISACTIVE = "isactive";
        public const string ANNOUNCEMENT_META_KEY_TRASH = "trash";
        public const string ANNOUNCEMENT_META_KEY_ISACTIVE_FALSE = "0";

        // Time Zone
        public const string TIME_ZONE_MAP_XML_PATH = "FacilityTimeZoneMapXmlPath";
        public const string TIME_ZONE_MAP_NODE = "FacilityTimeZoneMap";
        public const string TIME_ZONE_MAP_FACILITY = "Facility";
        public const string TIME_ZONE_MAP_TIME_ZONE = "TimeZone";

        // Time Format
        public const string TIME_FORMAT_DATE_TIME_12H = "MM/dd/yyyy hh:mm:ss tt";
        public const string TIME_FORMAT_DATE = "MM/dd/yyyy";
        public const string TIME_FORMAT_ZERO_DATETIME = "0000-00-00 00:00:00";
    }
}
