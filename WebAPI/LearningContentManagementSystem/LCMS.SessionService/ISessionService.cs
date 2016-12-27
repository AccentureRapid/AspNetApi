using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.SessionService
{
    public interface ISessionService
    {
        dynamic getSessionsByLocation(string facilityName, string locationName);

        dynamic getSessionsByFacility(string facilityName);
    }
}
