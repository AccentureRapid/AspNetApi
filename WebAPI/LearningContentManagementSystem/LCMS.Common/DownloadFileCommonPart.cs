using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
//using MMC.Deliverable.DTO;
namespace LCMS.Common
{
    public class DownloadFileCommonPart : IDownloadFileCommonPart
    {
        public DownloadFileCommonPart()
        {
            
        }

        private void EnsurePathValid(string path)
        {
            if (Directory.Exists(Path.GetDirectoryName(path)) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
        }

        public string GetFileType (string fileName){
            string type = string.Empty;
            string extension = string.Empty;
            extension = Path.GetExtension(fileName);
            switch (extension)
            {
                case ".txt":
                    type = "text/plain";
                    break;
                case ".htm":
                    type = "text/html";
                    break;
                case ".html":
                    type = "text/html";
                    break;
                case ".rtf":
                    type = "text/richtext";
                    break;
                case ".jpg":
                    type = "image/jpeg";
                    break;
                case ".jpeg":
                    type = "image/jpeg";
                    break;
                case ".gif":
                    type = "image/gif";
                    break;
                case ".bmp":
                    type = "image/bmp";
                    break;
                case ".mpg":
                    type = "video/mpeg";
                    break;
                case ".mpeg":
                    type = "video/mpeg";
                    break;
                case ".avi":
                    type = "video/avi";
                    break;
                case ".pdf":
                    type = "application/pdf";
                    break;
                case ".doc":
                    type = "application/msword";
                    break;
                case ".docx":
                    type = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case ".dot":
                    type = "application/msword";
                    break;
                case ".dotx":
                    type = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                    break;
                case ".csv":
                    type = "application/vnd.msexcel";
                    break;
                case ".xls":
                    type = "application/vnd.ms-excel";
                    break;
                case ".xlsx":
                    type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case ".xlsm":
                    type = "application/vnd.ms-excel.sheet.macroEnabled.12";
                    break;
                case ".ppt":
                    type = "application/vnd.ms-powerpoint";
                    break;
                case ".pptx":
                    type = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    break;
                case ".xlt":
                    type = "application/vnd.msexcel";
                    break;
                case ".zip":
                    type = "application/octet-stream";
                    break;

                default:
                    type = "application/octet-stream";
                    break;

            }
            return type;
        }

        //for demo
        public byte[] DownloadFile(string tempFolder, string fileName)
        {
                byte[] result = new byte[0];
                string casestring = string.Empty;
                string type = string.Empty;
                //StreamContent response;
                WebClient client = new WebClient();
                var downloadUri = tempFolder + @"\" + fileName;
                try
                {
                    EnsurePathValid(downloadUri);
                    result = File.ReadAllBytes(downloadUri);
                }
                catch (NullReferenceException ex)
                {
                    result = null;
                }
                //if (File.Exists(downloadUri))
                //{
                //    using (FileStream stream = new FileStream(downloadUri, FileMode.Open))
                //    {
                //        result = new byte[stream.Length];

                //        stream.Read(result, 0, Convert.ToInt32(stream.Length));
                //    }                                     
                    
                //}

                return result;  
              
        }



        //ture part, but can't guarantee it can work
        //public bool DownloadFile(int lcuID, string tempFolder, string fileName, int moduleId)
        //{
        //    bool result = false;
        //    string casestring = string.Empty;
        //    string type = string.Empty;
        //    HttpResponseMessage response = new HttpResponseMessage();

        //    NetworkCredential _credentials;
        //    System.Collections.Specialized.NameValueCollection configSection = default(System.Collections.Specialized.NameValueCollection);
        //    configSection = (System.Collections.Specialized.NameValueCollection)ConfigurationManager.GetSection("SharepointAppSettings");
        //    _credentials = new NetworkCredential(configSection["UserName"], configSection["Password"], configSection["Domain"]);

        //    if (!(lcuID == null) && !(moduleId == null) && !(fileName == null || fileName == string.Empty))
        //    {
        //        try
        //        {
        //            IEnumerable<LCUDetails> details = DeliverableRepository.GetLCUDetails(lcuID);
        //            if (details.Count() > 0)
        //                detailsTable = DeliverableRepository.GetLCUDetails(lcuID);
        //            if (detailsTable.Rows.Count > 0)
        //            {
        //                detailsRow = detailsTable.Rows[0];
        //                foreach (var i in details)
        //                {
        //                    _region = i.GeographicRegionCd; //(string)detailsRow["GeographicRegionCd"];
        //                    _country = i.SharepointCountryCd; //(string)detailsRow["SharepointCountryCd"];
        //                    clientNbr = i.ContentValue; //(string)detailsRow["CustomerNbr"]; // CustomerNbr // ContentValue
        //                }


        //                if (_region == "EL")
        //                {
        //                    region = "EME";
        //                }
        //                else if (_region == "AP")
        //                {
        //                    region = "APA";
        //                }
        //                else
        //                {
        //                    region = "AMR";
        //                }

        //                if (webUrl.Substring(webUrl.Length - 1, 1) != "/")
        //                {
        //                    webUrl = webUrl + "/";
        //                }

        //                strUri.Append(webUrl);
        //                strUri.Append(region);
        //                strUri.Append("/");
        //                strUri.Append(_country);
        //                strUri.Append("/");


        //                strTargetFolder.Append(docLib);
        //                strTargetFolder.Append("/");
        //                strTargetFolder.Append(clientNbr);
        //                strTargetFolder.Append("/");
        //                strTargetFolder.Append(lcuID);
        //                strTargetFolder.Append("/");
        //                strTargetFolder.Append(moduleName.ToString());
        //                strTargetFolder.Append("/");
        //                strTargetFolder.Append(Convert.ToInt32(moduleId).ToString());
        //                strTargetFolder.Append("/");
        //                strTargetFolder.Append(fileName);
        //                targetFolder = strTargetFolder.ToString();
        //                sharepointUrl = strUri.ToString() + targetFolder;
        //                result = DeliverableRepository.Download(sharepointUrl, _credentials);
        //            }
        //            else
        //            {
        //                result = false;
        //                throw new ApplicationException("No Record Found For LCUId " + lcuID);
        //            }
        //        }
        //        finally
        //        {
        //            strUri = null;
        //            strTargetFolder = null;
        //        }
        //    }
        //    else
        //    {
        //        result = false;
        //    }
        //    return result;
        //}

       // public StreamContent response { get; set; }
    }
}
