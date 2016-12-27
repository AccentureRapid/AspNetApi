using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace LCMS.Common
{
    public static class HttpRequestService
    {
        public static string HttpPost(string Url, string postDataStr, string contenttype, Dictionary<string, string> headers, out HttpStatusCode respcode)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = contenttype; //"application/json"; //application/x-www-form-urlencoded
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            //request.CookieContainer = cookie;
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, new UTF8Encoding(false));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            respcode = response.StatusCode;
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        //windows integrated authentication 
        public static string HttpWIAPost(string Url, string postDataStr, string contenttype, string username, string password, string domain, out HttpStatusCode respcode)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = contenttype; //"application/json"; //application/x-www-form-urlencoded
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Credentials = new NetworkCredential(username, password, domain);


            //request.CookieContainer = cookie;
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, new UTF8Encoding(false));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            respcode = response.StatusCode;
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public static string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public static string HttpWIAGet(string Url, string contenttype, string username, string password, string domain, out HttpStatusCode respcode)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = contenttype; //"application/json"; //application/x-www-form-urlencoded
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Credentials = new NetworkCredential(username, password, domain);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            respcode = response.StatusCode;

            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public static string HttpDelete(string Url, string postDataStr, string contenttype, string username, string password, string domain, out HttpStatusCode respcode)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "DELETE";
            request.Credentials = new NetworkCredential(username, password, domain);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            respcode = response.StatusCode;
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

    }
}