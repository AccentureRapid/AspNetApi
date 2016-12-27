using LCMS.Common.Datetime;
using LCMS.SessionRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.SessionService.Impl
{
    public class SessionService : ISessionService
    {
        public ISessionRepository SessionRepository { get; private set; }

        public SessionService (ISessionRepository sessionRepository)
        {
            this.SessionRepository = sessionRepository;
        }

        public dynamic getSessionsByLocation(string facilityName, string locationName)
        {
            string timeZoneId = DatetimeHelper.GetTimeZoneIDByFacility(facilityName);
            return SessionRepository.getSessionsByLocation(facilityName, locationName, timeZoneId);
        }

        public dynamic getSessionsByFacility(string facilityName)
        {
            string timeZoneId = DatetimeHelper.GetTimeZoneIDByFacility(facilityName);
            return SessionRepository.getSessionsByFacility(facilityName, timeZoneId);
        }
    }
}
