using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.SessionRepository
{
    public interface ISessionRepository
    {
        dynamic getSessionsByLocation(string facilityName, string locationName, string timeZoneId);

        dynamic getSessionsByFacility(string facilityName, string timeZoneId);
    }
}
