using Accenture.Security.Eso.Web;
using LCMS.AnnouncementService;
using LCMS.Common;
using LCMS.Common.Logging;
using LCMS.Common.WebApi;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace LCMS.ServiceController.Announcement
{
    [ESOAuthorizeFilter]
    public class AnnouncementServiceController : ApiController
    {
        public IAnnouncementService AnnouncementService { get; private set; }

        public AnnouncementServiceController(IAnnouncementService announcementService)
        {
            this.AnnouncementService = announcementService;
        }

        [HttpGet]
        public HttpResponseMessage GetAnnouncementsByFacility(string facilityName)
        {
            HttpResponseMessage message = new HttpResponseMessage();

            var result = AnnouncementService.GetAnnouncementsByFacility(facilityName);
            message.Content = Serialize(result);
            message.StatusCode = HttpStatusCode.OK;

            return message;
        }

        private StringContent Serialize(dynamic source)
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new NullToEmptyStringResolver(),
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var stringcontent = JsonConvert.SerializeObject(source, Newtonsoft.Json.Formatting.Indented, settings);
            return new StringContent(stringcontent, Encoding.GetEncoding("UTF-8"), "application/json");
        }
    }
}
