using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.AnnouncementRepository
{
    public interface IAnnouncementRepository
    {
        dynamic GetAnnouncementsByFacility(string facilityName, string timeZoneId);
    }
}
