using LCMS.AnnouncementRepository;
using LCMS.Common;
using LCMS.Common.Datetime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.AnnouncementService.Impl
{
    public class AnnouncementService : IAnnouncementService
    {
        public IAnnouncementRepository AnnouncementRepository { get; private set; }

        public AnnouncementService(IAnnouncementRepository announcementRepository)
        {
            AnnouncementRepository = announcementRepository;
        }

        public dynamic GetAnnouncementsByFacility(string facilityName)
        {
            string timeZoneId = DatetimeHelper.GetTimeZoneIDByFacility(facilityName);
            return AnnouncementRepository.GetAnnouncementsByFacility(facilityName, timeZoneId);
        }
    }
}
