using LCMS.Common;
using LCMS.Common.WebApi;
using LCMS.SessionService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace LCMS.ServiceController.Session
{
    [ESOAuthorizeFilter]
    public class SessionServiceController : ApiController
    {
        public ISessionService SessionService { get; private set; }

        public SessionServiceController (ISessionService sessionService)
        {
            this.SessionService = sessionService;
        }

        [HttpGet]
        public HttpResponseMessage getSessionsByLocation(string facilityName, string locationName)
        {
            HttpResponseMessage message = new HttpResponseMessage();

            var result = SessionService.getSessionsByLocation(facilityName, locationName);
            message.Content = Serialize(result);
            message.StatusCode = HttpStatusCode.OK;

            return message;
        }

        [HttpGet]
        public HttpResponseMessage getSessionsByFacility(string facilityName)
        {
            HttpResponseMessage message = new HttpResponseMessage();

            var result = SessionService.getSessionsByFacility(facilityName);
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
