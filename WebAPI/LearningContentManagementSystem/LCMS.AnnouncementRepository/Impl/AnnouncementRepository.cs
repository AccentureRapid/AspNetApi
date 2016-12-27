using LCMS.AnnouncementRepository.DTO;
using LCMS.AnnouncementRepository.Entities;
using LCMS.Common;
using LCMS.Common.Datetime;
using LCMS.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.AnnouncementRepository.Impl
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        public AnnouncementModelContainer Context { get; private set; }
        
        public AnnouncementRepository(AnnouncementModelContainer context)
        {
            Context = context;
        }

        public AnnouncementRepository()
        {
            Context = new AnnouncementModelContainer();
        }

        public dynamic GetAnnouncementsByFacility(string facilityName, string timeZoneId)
        {
            SimpleLogger logger = new SimpleLogger(AppDomain.CurrentDomain.BaseDirectory + string.Format(@"/log/log_{0}.txt", DateTime.Now.ToString("yyyy_MM_dd")));

            ReturnResult result = new ReturnResult();

            try
            {
                var localDateTimeNow = DatetimeHelper.ConvertUTCtoLocal(DateTime.UtcNow, timeZoneId);

                var qAnnPosts = from post in Context.wp_posts
                                join meta in Context.wp_postmeta on post.id equals meta.post_id
                                where post.post_type == Constants.ANNOUNCEMENT_POST_TYPE
                                && post.post_status != Constants.ANNOUNCEMENT_META_KEY_TRASH
                                select post;

                // MySQL do not support MARS(Multiple Active Result Set) so call toList() first
                var annIDs = qAnnPosts.Select(r => r.id).Distinct().ToList();

                //RapidLogger.Trace("announcements",string.Format("all announcement ids: {0}", string.Join(",", annIDs)));
                //Context.Database.Log = s => RapidLogger.Trace("EF log :", s);

                StringBuilder sb = new StringBuilder();
                sb.Append(" SELECT meta_id, post_id, meta_key, meta_value FROM wp_postmeta WHERE post_id in ( ");
                sb.Append(string.Join(",", annIDs));
                sb.Append(" )");

                //RapidLogger.Trace("EF log :", sb.ToString());

                var annMetas = Context.Database.SqlQuery<wp_postmeta_entity>(sb.ToString()).ToList();

                IList<AnnouncementDTO> annList = new List<AnnouncementDTO>();
                foreach (var id in annIDs)
                {
                    var annMeta = annMetas.Where(p => p.post_id == id).ToList();
                    var facilityId = Convert.ToInt32(annMeta.FirstOrDefault(p => p.meta_key == Constants.ANNOUNCEMENT_META_KEY_FACILITY_NAME).meta_value);
                    var facilityMetas = Context.wp_postmeta.Where(r => r.post_id == facilityId).ToList();

                    annList.Add(new AnnouncementDTO()
                    {
                        FacilityName = facilityMetas.FirstOrDefault(p => p.meta_key == Constants.ANNOUNCEMENT_META_KEY_FACILITY_NAME).meta_value,
                        StartDate = DatetimeHelper.ConvertUTCtoLocal(Convert.ToDateTime(annMeta.FirstOrDefault(p => p.meta_key == Constants.ANNOUNCEMENT_META_KEY_START_DATE).meta_value), timeZoneId),
                        EndDate = annMeta.Select(p => p.meta_key).Contains(Constants.ANNOUNCEMENT_META_KEY_END_DATE) ? annMeta.FirstOrDefault(p => p.meta_key == Constants.ANNOUNCEMENT_META_KEY_END_DATE).meta_value.Equals(Constants.TIME_FORMAT_ZERO_DATETIME) ? DateTime.MaxValue :
                            DatetimeHelper.ConvertUTCtoLocal(Convert.ToDateTime(annMeta.FirstOrDefault(p => p.meta_key == Constants.ANNOUNCEMENT_META_KEY_END_DATE).meta_value), timeZoneId) : DateTime.MaxValue,
                        AnnouncementDesc = annMeta.Select(p => p.meta_key).Contains(Constants.ANNOUNCEMENT_META_KEY_ANNOUNCEMENT_DESCRIPTION) ?
                            annMeta.FirstOrDefault(p => p.meta_key == Constants.ANNOUNCEMENT_META_KEY_ANNOUNCEMENT_DESCRIPTION).meta_value : string.Empty,
                        Priority = annMeta.Select(p => p.meta_key).Contains(Constants.ANNOUNCEMENT_META_KEY_PRIORITY) ? annMeta.FirstOrDefault(p => p.meta_key == Constants.ANNOUNCEMENT_META_KEY_PRIORITY).meta_value : string.Empty,
                        IsActive = annMeta.Select(p => p.meta_key).Contains(Constants.ANNOUNCEMENT_META_KEY_ISACTIVE) ? annMeta.FirstOrDefault(p => p.meta_key == Constants.ANNOUNCEMENT_META_KEY_ISACTIVE).meta_value.Equals(Constants.ANNOUNCEMENT_META_KEY_ISACTIVE_FALSE) ? false : true : false
                    });
                }

                var qAnnContent = annList.Where(r => r.FacilityName == facilityName && r.IsActive == true);
                //var annContent = qAnnContent.Where(r => r.StartDate <= localDateTimeNow && r.EndDate >= localDateTimeNow).ToList();
                var annContent = qAnnContent.Where(r => r.StartDate.Date <= localDateTimeNow.Date 
                                                            && (r.EndDate==null || 
                                                            (r.EndDate != null && r.EndDate.Date >= localDateTimeNow.Date))).ToList();

                var annContentList = new List<dynamic>();
                var annResultList = new List<dynamic>();
                foreach (var ann in annContent)
                {
                    annContentList.Add(new
                    {
                        AnnouncementDesc = ann.AnnouncementDesc,
                        Priority = ann.Priority,
                        EffectiveStartDT = ann.StartDate.ToString(Constants.TIME_FORMAT_DATE_TIME_12H),
                        EffectiveEndDate = ann.EndDate.ToString(Constants.TIME_FORMAT_DATE_TIME_12H)
                    });
                }

                annResultList.Add(new
                {
                    FacilityName = facilityName,
                    Date = localDateTimeNow.ToString(Constants.TIME_FORMAT_DATE),
                    Announcements = annContentList
                });

                result.ReturnCode = ReturnStatusCode.Succeed;
                result.ReturnMessage = Constants.STATUS_SUCCESS;
                result.Content = annResultList;
            }
            catch (Exception ex)
            {
                result.ReturnCode = ReturnStatusCode.Failed;
                result.ReturnMessage = ex.StackTrace + " and " + ex.InnerException.StackTrace;
                result.Content = null;
            }

            return result;
        }
    }
}
