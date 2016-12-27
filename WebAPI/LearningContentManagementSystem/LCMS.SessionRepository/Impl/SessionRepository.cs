using LCMS.Common;
using LCMS.Common.Datetime;
using LCMS.SessionRepository.DTO;
using LCMS.SessionRepository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.SessionRepository.Impl
{
    public class SessionRepository : ISessionRepository
    {
        public SessionModelContainer Context { get; private set; }

        public SessionRepository()
        {
            this.Context = new SessionModelContainer();
        }

        public SessionRepository(SessionModelContainer context)
        {
            this.Context = context;
        }

        public dynamic getSessionsByLocation(string facilityName, string locationName, string timeZoneId)
        {
            ReturnResult result = new ReturnResult();

            try
            {
                //var localDateTimeNow = DatetimeHelper.ConvertUTCtoLocal(DateTime.Parse("2016/12/02 11:30:00"), timeZoneId);
                var localDateTimeNow = DatetimeHelper.ConvertUTCtoLocal(DateTime.UtcNow, timeZoneId);
                var localDateNow = localDateTimeNow.Date;
                var localDateTommorow = localDateTimeNow.Date.AddDays(1);

                // TODO: CUICK ADD START
                DateTime currentLocalTime = DatetimeHelper.ConvertUTCtoLocal(DateTime.UtcNow, timeZoneId);
                // TODO: CUICK ADD END

                var qSessions = from dst in Context.wp_dst
                                where dst.FacilityName == facilityName
                                && dst.LocationName == locationName
                                && dst.DoNotDisplay == false
                                select dst;

                var sessionList = qSessions.Distinct().OrderBy(r => r.LocationStartDT).ToList();
                foreach (var session in sessionList)
                {
                    session.LocationStartDT = DatetimeHelper.ConvertUTCtoLocal(session.LocationStartDT, timeZoneId);
                    session.LocationEndDT = DatetimeHelper.ConvertUTCtoLocal(session.LocationEndDT, timeZoneId);
                }

                var sessionAllList = sessionList.Where(r => r.SessionStartDT.Date <= currentLocalTime.Date && r.SessionEndDT.Date >= currentLocalTime.Date).Distinct().OrderBy(r => r.LocationStartDT);
                //var sessionFromNowList = sessionAllList.Where((r => r.LocationStartDT <= localDateTimeNow && r.LocationEndDT >= localDateTimeNow)).Union(sessionAllList.Where(r => r.LocationStartDT > localDateTimeNow)).Distinct().OrderBy(r => r.LocationStartDT);
                var sessionContentList = new List<SessionByLocationDTO>();
                var idSum = 0;

                foreach (var session in sessionAllList)
                {
                    idSum++;
                    sessionContentList.Add(new SessionByLocationDTO
                    {
                        OrderID = idSum.ToString(),
                        SessionName = session.SessionName,
                        VenueName = session.VenueName,
                        StartDT = session.LocationStartDT.ToString(Constants.TIME_FORMAT_DATE_TIME_12H),
                        EndDate = session.LocationEndDT.ToString(Constants.TIME_FORMAT_DATE_TIME_12H)
                    });
                }

                var sessionResultList = new List<dynamic>();
                sessionResultList.Add(new
                {
                    FacilityName = facilityName,
                    Date = localDateTimeNow.ToString(Constants.TIME_FORMAT_DATE),
                    LocationName = locationName,
                    Transition = 10.ToString(),
                    DisplayValues = sessionContentList
                });

                result.ReturnCode = ReturnStatusCode.Succeed;
                result.ReturnMessage = Constants.STATUS_SUCCESS;
                result.Content = sessionResultList;
            }
            catch (Exception ex)
            {
                result.ReturnCode = ReturnStatusCode.Failed;
                result.ReturnMessage = ex.Message;
                result.Content = null;
            }

            return result;
        }

        public dynamic getSessionsByFacility(string facilityName, string timeZoneId)
        {
            ReturnResult result = new ReturnResult();

            try
            {
                //var localDateTimeNow = DatetimeHelper.ConvertUTCtoLocal(DateTime.Parse("2016/12/02 11:15:00"), timeZoneId);
                var localDateTimeNow = DatetimeHelper.ConvertUTCtoLocal(DateTime.UtcNow, timeZoneId);
                var localDateNow = localDateTimeNow.Date;
                var localDateTommorow = localDateTimeNow.Date.AddDays(1);

                // TODO: CUICK ADD START
                DateTime currentLocalTime = DatetimeHelper.ConvertUTCtoLocal(DateTime.UtcNow, timeZoneId);
                // TODO: CUICK ADD END

                var qSessions = from dst in Context.wp_dst
                                where dst.FacilityName == facilityName
                                && dst.DoNotDisplay == false
                                select dst;

                var sessionList = qSessions.Distinct().ToList();
                foreach (var session in sessionList)
                {
                    session.SessionStartDT = DatetimeHelper.ConvertUTCtoLocal(session.SessionStartDT, timeZoneId);
                    session.SessionEndDT = DatetimeHelper.ConvertUTCtoLocal(session.SessionEndDT, timeZoneId);
                }

                // TODO: CUICK REPLACE START
                //var sessionAllList = sessionList.Where(r => r.SessionStartDT >= localDateNow && r.SessionEndDT <= localDateTommorow).OrderBy(r => r.SessionStartDT).ToList();
                var sessionAllList = sessionList.Where(r => r.SessionEndDT >= currentLocalTime && (r.SessionStartDT.Date <= currentLocalTime.Date && r.SessionEndDT.Date >= currentLocalTime.Date)).OrderBy(r => r.SessionStartDT).ToList();
                // TODO: CUICK REPLACE END


                var sessionNames = sessionAllList.GroupBy(r => r.SessionName).Distinct().ToList();
                var sessionContentList = new List<dynamic>();
                
                foreach (var name in sessionNames)
                {
                    var sessionByName = sessionAllList.Where(r => r.SessionName == name.Key).OrderBy(r => r.SessionStartDT).ToList();
                    var sessionDetailList = new List<SessionByFacilityDTO>();
                    var sum = 0;

                    if (sessionByName.Any())
                    {
                        foreach (var session in sessionByName)
                        {
                            sum++;
                            sessionDetailList.Add(new SessionByFacilityDTO
                            {
                                OrderID = sum.ToString(),
                                VenueName = session.VenueName,
                                StartDT = session.SessionStartDT.ToString(Constants.TIME_FORMAT_DATE_TIME_12H),
                                EndDate = session.SessionEndDT.ToString(Constants.TIME_FORMAT_DATE_TIME_12H),
                                LocationName = session.LocationName
                            });
                        }

                        sessionContentList.Add(new
                        {
                            SessionName = name.Key,
                            SessionDetails = sessionDetailList
                        });
                    }
                }

                var sessionResultList = new List<dynamic>();
                sessionResultList.Add(new
                {
                    FacilityName = facilityName,
                    // TODO: CUICK REPLACE START
                    //Date = localDateTimeNow.ToString(Constants.TIME_FORMAT_DATE),
                    Date = currentLocalTime.ToString(Constants.TIME_FORMAT_DATE),
                    // TODO: CUICK REPLACE END
                    DisplayValues = sessionContentList
                });

                result.ReturnCode = ReturnStatusCode.Succeed;
                result.ReturnMessage = Constants.STATUS_SUCCESS;
                result.Content = sessionResultList;
            }
            catch (Exception ex)
            {
                result.ReturnCode = ReturnStatusCode.Failed;
                result.ReturnMessage = ex.Message;
                result.Content = null;
            }

            return result;
        }
    }
}
