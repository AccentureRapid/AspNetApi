using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.AnnouncementService
{
    public interface IAnnouncementService
    {
        dynamic GetAnnouncementsByFacility(string facilityName);
    }
}
