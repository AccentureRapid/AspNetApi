using Accenture.Security.Eso.Web;
using LCMS.Common.Cache;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace LCMS.Common.WebApi
{
    public class ESOAuthorizeFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
            var request = actionContext.Request;
            //Get ESO info and store it in http context
            HttpResponseMessage response = new HttpResponseMessage();

            // TODO: CUICK DELETE START
            //var version =  ConfigurationManager.AppSettings["eso:oauth-version"];
            //if (!string.IsNullOrEmpty(version) && version == "oauth1")
            //{
            //    Oauth1Authorize(request, response);
            //}
            //else
            //{
            // TODO: CUICK DELETE START
                Oauth2Authorize(request, response); 
            //}

            if (!response.IsSuccessStatusCode)
            {
                actionContext.Response = response;
            }
        }

        private void Oauth1Authorize(HttpRequestMessage request, HttpResponseMessage response)
        {
            if (request.Headers.Authorization == null || string.IsNullOrEmpty(request.Headers.Authorization.Parameter))
            {
                response.StatusCode = HttpStatusCode.Forbidden;
            }
            else
            {
                // https://federation-sts.accenture.com/services/jwt/issue/adfs Production
                // https://federation-sts-stage.accenture.com/services/jwt/issue/adfs Staging
                string token = request.Headers.Authorization.Parameter;

                try
                {
                    var result = SmartCache<string>.Get(token, (key) => GetESOResponse(key));
                    response.StatusCode = string.IsNullOrEmpty(result) ? HttpStatusCode.Forbidden : HttpStatusCode.OK;
                }
                catch (Exception)
                {
                    response.StatusCode = HttpStatusCode.Forbidden;
                }
            }
        }

        private void Oauth2Authorize(HttpRequestMessage request, HttpResponseMessage response)
        {
            if (request.Headers.Authorization == null || string.IsNullOrEmpty(request.Headers.Authorization.Parameter))
            {
                response.StatusCode = HttpStatusCode.Forbidden;
            }
            else
            {
                string allowedScopes = ConfigurationManager.AppSettings["eso:scope"];
                string[] allowScopesCollection;
                if (!string.IsNullOrEmpty(allowedScopes))
                {
                    allowScopesCollection = allowedScopes.Split(' ');
                }
                
                string token = request.Headers.Authorization.Parameter;

                try
                {
                    var result = SmartCache<string>.Get(token, (key) => ParseToJWT(key));
                    //scopes compare and is time expires
                    var jobject = (JObject)JsonConvert.DeserializeObject(result);
                    var scopeProperty = (JProperty)jobject.Children().FirstOrDefault(p =>((JProperty)p).Name.Equals("scope"));
                    var scope = scopeProperty.Value;

                    var expProperty = (JProperty)jobject.Children().FirstOrDefault(p => ((JProperty)p).Name.Equals("exp"));

                    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); 
                    var exp = startTime.AddSeconds(double.Parse(expProperty.Value.ToString()));
                    var now = DateTime.Now;
                    
                    //expire check and resource scope check
                    //if (!allowScopesCollection.Any(p => p.Trim().Equals(scope.ToString().Trim())) || exp < now)
                    //{
                    //    response.StatusCode = HttpStatusCode.Forbidden;
                    //}
                    if (exp < now)
                    {
                        response.StatusCode = HttpStatusCode.Forbidden;
                    }
                    //client id check
                    var allowedClientId = ConfigurationManager.AppSettings["eso:client-id"];
                    if (!string.IsNullOrEmpty(allowedClientId))
                    {
                        var clientIdProperty = (JProperty)jobject.Children().FirstOrDefault(p => ((JProperty)p).Name.Equals("client_id"));
                        var clientId = clientIdProperty.Value.ToString();
                        if (!allowedClientId.Trim().Equals(clientId.Trim()))
                        {
                            response.StatusCode = HttpStatusCode.Forbidden;
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.StatusCode = HttpStatusCode.Forbidden;
                }
            }
        }
        private string GetESOResponse(string jwt)
        {
            string endPoint = ConfigurationManager.AppSettings["eso:end-point"];
            string scope = ConfigurationManager.AppSettings["eso:scope"];
            // Create a request using a URL that can receive a post. 
            WebRequest esoRequest = WebRequest.Create(endPoint);
            // Set the Method property of the request to POST.
            esoRequest.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = string.Format("grant_type={0}&assertion={1}&scope={2}", "urn:ietf:params:oauth:grant-type:jwt-bearer", jwt, scope);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            esoRequest.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            esoRequest.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = esoRequest.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();

            // Get the response.
            WebResponse esoResponse = esoRequest.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)esoResponse).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = esoResponse.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            JObject jsob = (JObject)JsonConvert.DeserializeObject(responseFromServer);
            var accessToken = jsob["access_token"].ToString();

            reader.Close();
            dataStream.Close();
            esoResponse.Close();

            return accessToken;
        }

        private string ParseToJWT(string token)
        {
            string[] tokens = token.Split('.');

            if (tokens != null && tokens.Length == 3)
            {
                string claims = tokens[1];
                int mod4 = claims.Length % 4;

                if (mod4 > 0)
                {
                    claims += new string('=', 4 - mod4);
                }

                return Encoding.ASCII.GetString(Convert.FromBase64String(claims));

            }

            return string.Empty;
        }
    }
}
