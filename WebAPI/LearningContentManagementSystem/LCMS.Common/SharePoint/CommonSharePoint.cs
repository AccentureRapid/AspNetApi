using System;
using System.Net;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.SharePoint.Client;
using System.Windows;
using System.IO;

namespace LCMS.Common
{
    public partial class AddAcceptanceFormEntities
    {
        public string LcuID { get; set; }
        public int ModuleID { get; set; }
        public string UserID { get; set; }
    }

    public partial class HttpMessage
    {
        public StreamContent streams { get; set; }
        public string contentType { get; set; }
        public string fileName { get; set; }
        //public string contentDisposition { get; set; }
    }
    public partial class SharePointMetaData
    {
        public string operatingGrpCode { get; set; }
        public string operatingGrp { get; set; }
        public string operatingUnitCode { get; set; }
        public string operatingUnit { get; set; }
        public string clientServiceGrpCode { get; set; }
        public string clientServiceGrp { get; set; }
        public string clientName { get; set; }
        public string clientNumber { get; set; }
        public string contractNumber { get; set; }
        public string strLCUId { get; set; }
        public string moduleName { get; set; }
        public string moduleId { get; set; }
        public string userId { get; set; }
        public string fileTypeCode { get; set; }
        public string fileType { get; set; }
        public string documentTypeCode { get; set; }
        public string documentType { get; set; }
        public string documentName { get; set; }
    }
    public enum ModuleName
    {
        Document,
        ContractChange,
        Deliverables,
        Communications,
        IssuesAndRisks
    }
    public enum LevelOfDelete
    {
        File,
        ModuleFolder,
        ContractFolder,
        ClientFolder
    }
    public class CommonSharePoint
    {
        public static string webUrl = ConfigurationManager.AppSettings["SharepointUrl"];

        //Settings1.Default.SharepointUrl;
        public static string docLib = ConfigurationManager.AppSettings["DocumentLibrary"];
        public static string userName = ConfigurationManager.AppSettings["UserName"];
        public static string passWord = ConfigurationManager.AppSettings["Password"];
        public static string domain = ConfigurationManager.AppSettings["Domain"];
        //private static NetworkCredential _credentials = new NetworkCredential(userName, passWord, domain);

        //private static ClientContext init(string contextUrl)
        //{
        //    var clientContext = new ClientContext(contextUrl);
        //    Web web = clientContext.Web;
        //    clientContext.Credentials = new NetworkCredential(userName, passWord, domain);
        //    clientContext.Load(web);
        //    clientContext.ExecuteQuery();
        //    return clientContext;
        //}

        public static bool UploadDocument(byte[] bytes, string fileName, string GeographicRegionCd, string SharepointCountryCd, string CustomerNbr, string lcuId, int moduleId, SharePointMetaData spModel, ModuleName moduleName)
        {
            bool result = false;
            string region = string.Empty;
            string country = string.Empty;

            if (GeographicRegionCd == "EL")
            {
                region = "EME";
            }
            if (GeographicRegionCd == "AP")
            {
                region = "APA";
            }
            if (GeographicRegionCd != "EL" && GeographicRegionCd != "AP")
            {
                region = "AMR";
            }

            if (webUrl.Substring(webUrl.Length - 1, 1) != "/")
            {
                webUrl = webUrl + "/";
            }
            //Stream stream = new MemoryStream(bytes);

            using (var clientContext = new ClientContext(webUrl + region + @"/" + SharepointCountryCd))
            {
                string contextUrl = webUrl + region + @"/" + SharepointCountryCd;
                Web web = clientContext.Web;
                //clientContext.Credentials = new NetworkCredential(userName, passWord, domain);
                CredentialCache cc = new CredentialCache();
                cc.Add(new Uri(webUrl),
                "NTLM", new NetworkCredential(userName, passWord, domain));
                clientContext.Credentials = cc;
                clientContext.AuthenticationMode = ClientAuthenticationMode.Default;
                clientContext.Load(web);
                clientContext.ExecuteQuery();
                string destinationUrl = EnsureParentFolder(lcuId, moduleId.ToString(), moduleName, GeographicRegionCd, SharepointCountryCd, CustomerNbr, clientContext);
                var newFile = web.GetFolderByServerRelativeUrl(destinationUrl);
                clientContext.Load(newFile);
                clientContext.ExecuteQuery();

                //Upload File
                using (var stream = new MemoryStream(bytes))
                {
                    clientContext.RequestTimeout = 360000;
                    string destinationPath = destinationUrl + fileName;
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, destinationPath, stream, true);

                    //Set Metadata

                    Microsoft.SharePoint.Client.File metaData = web.GetFileByServerRelativeUrl(destinationPath);
                    //clientContext.Load(metaData.ListItemAllFields);
                    //clientContext.ExecuteQuery();
                    var setMetadata = metaData.ListItemAllFields;
                    setMetadata["OperatingGroupCd"] = spModel.operatingGrpCode;
                    setMetadata["OperatingGroupNm"] = spModel.operatingGrp;
                    setMetadata["OperatingUnitCd"] = spModel.operatingUnitCode;
                    setMetadata["OperatingUnitNm"] = spModel.operatingUnit;
                    setMetadata["ClientServiceGroupCd"] = spModel.clientServiceGrpCode;
                    setMetadata["ClientServiceGroupDesc"] = spModel.clientServiceGrp;
                    setMetadata["CustomerNm"] = spModel.clientName;
                    setMetadata["CustomerNbr"] = spModel.clientNumber;
                    setMetadata["ContractNbr"] = spModel.contractNumber;
                    setMetadata["LCUId"] = spModel.strLCUId;

                    if ((spModel.moduleName == "IssuesAndRisks"))
                    {
                        setMetadata["ModuleNm"] = "Issues and Risks";
                    }
                    else
                    {
                        setMetadata["ModuleNm"] = spModel.moduleName;
                    }

                    setMetadata["ModuleId"] = spModel.moduleId;
                    setMetadata["UserId"] = spModel.userId;
                    setMetadata["FileTypeCd"] = spModel.fileTypeCode;
                    setMetadata["FileTypeNm"] = spModel.fileType;
                    setMetadata["DocumentTypeCd"] = spModel.documentTypeCode;
                    setMetadata["DocumentTypeNm"] = spModel.documentType;
                    setMetadata["DocumentNm"] = spModel.documentName;
                    setMetadata.Update();
                    clientContext.Load(setMetadata);
                    clientContext.ExecuteQuery();
                    result = true;
                }
            }
            return result;
        }

        private static bool SetMeta(SharePointMetaData spModel, string contextUrl, string destinationPath)
        {
            var clientContext = new ClientContext(contextUrl);
            Web web = clientContext.Web;
            //clientContext.Credentials = new NetworkCredential(userName, passWord, domain);
            CredentialCache cc = new CredentialCache();
            cc.Add(new Uri(webUrl),
            "NTLM", new NetworkCredential(userName, passWord, domain));
            clientContext.Credentials = cc;
            clientContext.AuthenticationMode = ClientAuthenticationMode.Default;
            clientContext.Load(web);
            clientContext.ExecuteQuery();


            Microsoft.SharePoint.Client.File metaData = web.GetFileByServerRelativeUrl(destinationPath);
            //clientContext.Load(metaData.ListItemAllFields);
            //clientContext.ExecuteQuery();
            var setMetadata = metaData.ListItemAllFields;
            setMetadata["OperatingGroupCd"] = spModel.operatingGrpCode;
            setMetadata["OperatingGroupNm"] = spModel.operatingGrp;
            setMetadata["OperatingUnitCd"] = spModel.operatingUnitCode;
            setMetadata["OperatingUnitNm"] = spModel.operatingUnit;
            setMetadata["ClientServiceGroupCd"] = spModel.clientServiceGrpCode;
            setMetadata["ClientServiceGroupDesc"] = spModel.clientServiceGrp;
            setMetadata["CustomerNm"] = spModel.clientName;
            setMetadata["CustomerNbr"] = spModel.clientNumber;
            setMetadata["ContractNbr"] = spModel.contractNumber;
            setMetadata["LCUId"] = spModel.strLCUId;

            if ((spModel.moduleName == "IssuesAndRisks"))
            {
                setMetadata["ModuleNm"] = "Issues and Risks";
            }
            else
            {
                setMetadata["ModuleNm"] = spModel.moduleName;
            }

            setMetadata["ModuleId"] = spModel.moduleId;
            setMetadata["UserId"] = spModel.userId;
            setMetadata["FileTypeCd"] = spModel.fileTypeCode;
            setMetadata["FileTypeNm"] = spModel.fileType;
            setMetadata["DocumentTypeCd"] = spModel.documentTypeCode;
            setMetadata["DocumentTypeNm"] = spModel.documentType;
            setMetadata["DocumentNm"] = spModel.documentName;
            setMetadata.Update();
            //clientContext.Load(setMetadata);
            clientContext.ExecuteQuery();
            return true;
        }
        public static string EnsureParentFolder(string strLCUId, string moduleId, ModuleName moduleName, string GeographicRegionCd, string SharepointCountryCd, string CustomerNbr, ClientContext context)
        {
            string clientNbr = null;
            string destinationUrl = string.Empty;
            StringBuilder strUri = new StringBuilder("");
            StringBuilder strClientFolder = new StringBuilder("");
            StringBuilder strContractFolder = new StringBuilder("");
            StringBuilder strModuleFolder = new StringBuilder("");
            StringBuilder strModuleId = new StringBuilder("");
            string region = null;
            string _region = string.Empty;
            string _country = string.Empty;


            if (!(strLCUId == null || strLCUId == string.Empty) && !(moduleId == string.Empty || moduleId == null))
            {
                if (GeographicRegionCd != null && SharepointCountryCd != null && CustomerNbr != null)
                {

                    _region = GeographicRegionCd;
                    _country = SharepointCountryCd;
                    clientNbr = CustomerNbr;

                    if (_region == "EL")
                    {
                        region = "EME";
                    }
                    if (_region == "AP")
                    {
                        region = "APA";
                    }
                    if (_region != "EL" && _region != "AP")
                    {
                        region = "AMR";
                    }

                    if (webUrl.Substring(webUrl.Length - 1, 1) != "/")
                    {
                        webUrl = webUrl + "/";
                    }

                    strUri.Append("/sites/");
                    strUri.Append(region);
                    strUri.Append("/");
                    strUri.Append(_country);
                    strUri.Append("/");

                    //create folder for Client Number
                    strClientFolder.Append(docLib);
                    strClientFolder.Append("/");
                    strClientFolder.Append(clientNbr);

                    //create folder for Contract Number
                    strContractFolder.Append(strClientFolder.ToString());
                    strContractFolder.Append("/");
                    strContractFolder.Append(strLCUId);

                    //create folder for module name
                    strModuleFolder.Append(strContractFolder.ToString());
                    strModuleFolder.Append("/");
                    strModuleFolder.Append(moduleName.ToString());

                    //create folder for module Id
                    strModuleId.Append(strModuleFolder.ToString());
                    strModuleId.Append("/");
                    strModuleId.Append(moduleId);

                    string baseUrl = webUrl + region + @"/" + _country;
                    if (CheckIfFolderExist(strUri.ToString() + strModuleId.ToString(), baseUrl, context) == false)
                    {
                        Web web = context.Web;
                        if (CheckIfFolderExist(strUri.ToString() + strModuleFolder.ToString(), baseUrl, context) == false)// 
                        {
                            //CreateFolders strClientFolder/strContractFolder/strModuleFolder
                            string relativeUrl = strUri.ToString() + docLib + @"/";
                            var relativeClient = web.GetFolderByServerRelativeUrl(relativeUrl);
                            relativeClient.Folders.Add(clientNbr);

                            relativeUrl = relativeUrl + clientNbr + @"/";
                            var relativeContract = web.GetFolderByServerRelativeUrl(relativeUrl);
                            relativeContract.Folders.Add(strLCUId);

                            relativeUrl = relativeUrl + strLCUId + @"/";
                            var relativeModule = web.GetFolderByServerRelativeUrl(relativeUrl);
                            relativeModule.Folders.Add(moduleName.ToString());
                        }
                        //CreateFolder  strModuleId
                        var relativeUrlDirect = strUri.ToString() + strModuleFolder.ToString();
                        var relativeModuleId = web.GetFolderByServerRelativeUrl(relativeUrlDirect);
                        relativeModuleId.Folders.Add(strModuleId.ToString());
                        context.Load(relativeModuleId);
                        context.ExecuteQuery();
                    }
                    destinationUrl = strUri.ToString() + strModuleId.ToString() + "/";

                }
            }
            return destinationUrl;
        }

        public static Boolean DeleteUploadedFile(string lcuID, ModuleName moduleName, int moduleId, string fileName, LevelOfDelete level, string GeographicRegionCd, string SharepointCountryCd, string CustomerNbr)
        {
            string clientNbr = string.Empty;
            bool result = false;
            string targetFolder = string.Empty;
            string region = null;
            StringBuilder strUri = new StringBuilder("");
            StringBuilder strTargetFolder = new StringBuilder("");
            string _region;
            string _country;
            //DeliverableRepository.DeleteFromDB(moduleId);

            if (!(lcuID == null || lcuID == string.Empty) && !(moduleId == null) && !(fileName == null || fileName == string.Empty))
            {

                if (GeographicRegionCd != null && SharepointCountryCd != null && CustomerNbr != null)
                {

                    _region = GeographicRegionCd;
                    _country = SharepointCountryCd;
                    clientNbr = CustomerNbr;

                    if (_region == "EL")
                    {
                        region = "EME";
                    }
                    if (_region == "AP")
                    {
                        region = "APA";
                    }
                    if (_region != "EL" && _region != "AP")
                    {
                        region = "AMR";
                    }

                    if (webUrl.Substring(webUrl.Length - 1, 1) != "/")
                    {
                        webUrl = webUrl + "/";
                    }
                    strUri.Append("sites");
                    strUri.Append("/");
                    strUri.Append(region);  // Need to add into new URL 
                    strUri.Append("/");
                    strUri.Append(_country);
                    strUri.Append("/");

                    strTargetFolder.Append(docLib);
                    strTargetFolder.Append("/");
                    strTargetFolder.Append(clientNbr);

                    //Checks the exact location to be deleted
                    if (level == LevelOfDelete.File)
                    {
                        strTargetFolder.Append("/");
                        strTargetFolder.Append(lcuID);
                        strTargetFolder.Append("/");
                        strTargetFolder.Append(moduleName.ToString());
                        strTargetFolder.Append("/");
                        strTargetFolder.Append(moduleId);
                        strTargetFolder.Append("/");
                        strTargetFolder.Append(fileName);
                        targetFolder = strTargetFolder.ToString();
                    }
                    if (level == LevelOfDelete.ModuleFolder)
                    {
                        strTargetFolder.Append("/");
                        strTargetFolder.Append(lcuID);
                        strTargetFolder.Append("/");
                        strTargetFolder.Append(moduleName.ToString());
                        strTargetFolder.Append("/");
                        strTargetFolder.Append(moduleId);
                        targetFolder = strTargetFolder.ToString();
                    }
                    if (level == LevelOfDelete.ContractFolder)
                    {
                        strTargetFolder.Append("/");
                        strTargetFolder.Append(lcuID);
                        targetFolder = strTargetFolder.ToString();
                    }
                    else
                    {
                        targetFolder = strTargetFolder.ToString();
                    }

                    result = DeleteDocument(strUri.ToString(), targetFolder, fileName, SharepointCountryCd, region);
                }
                else
                {
                    result = false;
                    throw new ApplicationException("No Record Found For LCUId " + lcuID);
                }
            }
            else
            {
                result = false;
            }
            return result;
        }


        public static HttpMessage DownloadFile(int lcuID, ModuleName moduleName, int moduleId, string fileName, string GeographicRegionCd, string SharepointCountryCd, string ContentValue)
        {
            HttpMessage message = new HttpMessage();
            string strContentType = string.Empty;
            string casestring = string.Empty;
            string clientNbr = string.Empty;
            string _region = string.Empty;
            string _country = string.Empty;
            HttpResponseMessage result = new HttpResponseMessage();
            StringBuilder strUri = new StringBuilder("");
            StringBuilder strTargetFolder = new StringBuilder("");
            string sharepointUrl = string.Empty;
            string targetFolder = string.Empty;
            string region = string.Empty;

            if (!(lcuID == null) && !(moduleId == null) && !(fileName == null || fileName == string.Empty))
            {

                if (GeographicRegionCd != null && SharepointCountryCd != null && ContentValue != null)
                {
                    _region = GeographicRegionCd; //(string)detailsRow["GeographicRegionCd"];
                    _country = SharepointCountryCd; //(string)detailsRow["SharepointCountryCd"];
                    clientNbr = ContentValue; //(string)detailsRow["CustomerNbr"]; s // ContentValue
                }


                if (_region == "EL")
                {
                    region = "EME";
                }
                if (_region == "AP")
                {
                    region = "APA";
                }
                if (_region != "EL" && _region != "AP")
                {
                    region = "AMR";
                }

                if (webUrl.Substring(webUrl.Length - 1, 1) != "/")
                {
                    webUrl = webUrl + "/";
                }

                strUri.Append(webUrl);
                strUri.Append(region); // Set back when using new URL
                strUri.Append("/");
                strUri.Append(_country);
                strUri.Append("/");

                strTargetFolder.Append(docLib);
                strTargetFolder.Append("/");
                strTargetFolder.Append(clientNbr);
                strTargetFolder.Append("/");
                strTargetFolder.Append(lcuID);
                strTargetFolder.Append("/");
                strTargetFolder.Append(moduleName.ToString());
                strTargetFolder.Append("/");
                strTargetFolder.Append(Convert.ToInt32(moduleId).ToString());
                strTargetFolder.Append("/");
                strTargetFolder.Append(fileName);
                targetFolder = strTargetFolder.ToString();
                //sharepointUrl = strUri.ToString() + targetFolder;
                sharepointUrl = @"/sites/" + region + @"/" + _country + @"/" + targetFolder;
                message = Download(sharepointUrl, fileName, _country, region);

            }
            return message;
        }


        private static HttpMessage Download(string url, string fileName, string country, string region)
        {
            HttpMessage message = new HttpMessage();
            string casestring = string.Empty;
            string strContentType = string.Empty;
            //string filename = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            //var finalUrl = @"/sites"+url;
            try
            {
                ClientContext context = new ClientContext(webUrl + region + @"/" + country);
                //context.Credentials = new NetworkCredential(userName, passWord, domain);
                CredentialCache cc = new CredentialCache();
                cc.Add(new Uri(webUrl),
                "NTLM", new NetworkCredential(userName, passWord, domain));
                context.Credentials = cc;
                context.AuthenticationMode = ClientAuthenticationMode.Default;
                Web web = context.Web;
                context.Load(web);
                context.ExecuteQuery();

                WebClient client = new WebClient();

                var stream = Microsoft.SharePoint.Client.File.OpenBinaryDirect(context, url);
                //var stream = new FileStream(downloadUri, FileMode.Open);
                //using (var fileStream = new FileStream(fullPath, FileMode.Create)) // Download to Local
                //    stream.Stream.CopyTo(fileStream);

                message.streams = new StreamContent(stream.Stream);
                // downloadUri.Substring(downloadUri.LastIndexOf("/") + 1);
                var fext = fileName.Split('.');
                if (fext.Length > 1)
                {
                    casestring = fext[fext.Length - 1];
                }

                //set the content type of file according to extension 
                switch (casestring)
                {
                    case "txt":
                        strContentType = "text/plain";
                        break;
                    case "htm":
                        strContentType = "text/html";
                        break;
                    case "html":
                        strContentType = "text/html";
                        break;
                    case "rtf":
                        strContentType = "text/richtext";
                        break;
                    case "jpg":
                        strContentType = "image/jpeg";
                        break;
                    case "jpeg":
                        strContentType = "image/jpeg";
                        break;
                    case "gif":
                        strContentType = "image/gif";
                        break;
                    case "bmp":
                        strContentType = "image/bmp";
                        break;
                    case "mpg":
                        strContentType = "video/mpeg";
                        break;
                    case "mpeg":
                        strContentType = "video/mpeg";
                        break;
                    case "avi":
                        strContentType = "video/avi";
                        break;
                    case "pdf":
                        strContentType = "application/pdf";
                        break;
                    case "doc":
                        strContentType = "application/msword";
                        break;
                    case "docx":
                        strContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                    case "dot":
                        strContentType = "application/msword";
                        break;
                    case "dotx":
                        strContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                        break;
                    case "csv":
                        strContentType = "application/vnd.msexcel";
                        break;
                    case "xls":
                        strContentType = "application/vnd.ms-excel";
                        break;
                    case "xlsx":
                        strContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                    case "xlsm":
                        strContentType = "application/vnd.ms-excel.sheet.macroEnabled.12";
                        break;
                    case "ppt":
                        strContentType = "application/vnd.ms-powerpoint";
                        break;
                    case "pptx":
                        strContentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                        break;
                    case "xlt":
                        strContentType = "application/vnd.msexcel";
                        break;
                    case "zip":
                        strContentType = "application/octet-stream";
                        break;

                    default:
                        strContentType = "application/octet-stream";
                        break;

                }

                //response.Content.Headers.ContentType = new MediaTypeHeaderValue(strContentType);
                //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                //response.Content.Headers.ContentDisposition.FileName = fileName;
                message.contentType = strContentType;
                message.fileName = fileName;

            }
            catch (Exception ex)
            { }
            return message;
        }

        private static bool DeleteDocument(string uri, string target, string fileName, string SharepointCountryCd, string GeographicRegionCd)
        {
            bool result = false;
            try
            {
                ClientContext context = new ClientContext(webUrl + GeographicRegionCd + @"/" + SharepointCountryCd);
                Web web = context.Web;
                //context.Credentials = new NetworkCredential(userName, passWord, domain);
                CredentialCache cc = new CredentialCache();
                cc.Add(new Uri(webUrl),
                "NTLM", new NetworkCredential(userName, passWord, domain));
                context.Credentials = cc;
                context.AuthenticationMode = ClientAuthenticationMode.Default;
                context.Load(web);
                context.ExecuteQuery();

                string finalUrl = @"/" + uri + target + @"/" + fileName;
                Microsoft.SharePoint.Client.File loadUrl = web.GetFileByServerRelativeUrl(finalUrl);
                loadUrl.DeleteObject();
                context.ExecuteQuery();
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private static Boolean CheckIfFolderExist(string fianlUrl, string baseUrl, ClientContext context)
        {
            try
            {
                Web web = context.Web;
                var relativeurl = context.Web.GetFolderByServerRelativeUrl(fianlUrl);
                context.Load(relativeurl);
                context.ExecuteQuery();
                return true;
            }
            catch (WebException we)
            {
                return false;
            }
            catch (ServerException se)
            {
                return false;
            }
        }

        public static bool Move(string currentLCU,
            string newLCU,
            ModuleName moduleName,
            int moduleId,
            string geographicRegionCd,
            string sharepointCountryCd,
            string customerNbr,
            string fileName,
            string newGeographicRegionCd, string newSharepointCountryCd, string newCustomerNbr)
        {
            bool result = false;
            var _region = geographicRegionCd;
            var _country = sharepointCountryCd;
            var clientNbr = customerNbr;
            string region = string.Empty;
            string newRegion = string.Empty;
            StringBuilder finalUri = new StringBuilder("");
            if (_region == "EL")
            {
                region = "EME";
            }
            if (_region == "AP")
            {
                region = "APA";
            }
            if (_region != "EL" && _region != "AP")
            {
                region = "AMR";
            }

            _region = newGeographicRegionCd;
            if (_region == "EL")
            {
                newRegion = "EME";
            }
            if (_region == "AP")
            {
                newRegion = "APA";
            }
            if (_region != "EL" && _region != "AP")
            {
                newRegion = "AMR";
            }

            if (webUrl.Substring(webUrl.Length - 1, 1) != "/")
            {
                webUrl = webUrl + "/";
            }

            try
            {
                string uri = webUrl + region + @"/" + sharepointCountryCd;
                string newUri = webUrl + newRegion + @"/" + newSharepointCountryCd;
                var newClientContext = new ClientContext(newUri);
                Web newWeb = newClientContext.Web;
                //newClientContext.Credentials = new NetworkCredential(userName, passWord, domain);
                CredentialCache cc = new CredentialCache();
                cc.Add(new Uri(webUrl),
                "NTLM", new NetworkCredential(userName, passWord, domain));
                newClientContext.Credentials = cc;
                newClientContext.AuthenticationMode = ClientAuthenticationMode.Default;
                newClientContext.Load(newWeb);
                newClientContext.ExecuteQuery();
                using (ClientContext context = new ClientContext(uri))
                {
                    Web web = context.Web;
                    //context.Credentials = new NetworkCredential(userName, passWord, domain);
                    CredentialCache ccs = new CredentialCache();
                    cc.Add(new Uri(webUrl),
                    "NTLM", new NetworkCredential(userName, passWord, domain));
                    context.Credentials = ccs;
                    context.AuthenticationMode = ClientAuthenticationMode.Default;
                    context.Load(web);
                    context.ExecuteQuery();

                    string destinationUrl = EnsureParentFolder(newLCU, moduleId.ToString(), moduleName,newGeographicRegionCd, newSharepointCountryCd, newCustomerNbr, newClientContext);

                    finalUri.Append("/sites");
                    finalUri.Append("/");
                    finalUri.Append(region);
                    finalUri.Append("/");
                    finalUri.Append(sharepointCountryCd);
                    finalUri.Append("/");
                    finalUri.Append(docLib);
                    finalUri.Append("/");
                    finalUri.Append(clientNbr);
                    finalUri.Append("/");
                    finalUri.Append(currentLCU);
                    finalUri.Append("/");
                    finalUri.Append(moduleName.ToString());
                    finalUri.Append("/");
                    finalUri.Append(moduleId);
                    finalUri.Append("/");

                    string destUrl = finalUri.ToString() + fileName;
                    Microsoft.SharePoint.Client.File loadUrl = web.GetFileByServerRelativeUrl(destUrl);
                    if (region == newRegion)
                    {
                        loadUrl.MoveTo(destinationUrl + fileName, MoveOperations.Overwrite);
                        context.ExecuteQuery();
                        result = true;
                    }
                    if (region != newRegion)
                    {
                        var setMetadata = loadUrl.ListItemAllFields;
                        context.Load(setMetadata);
                        context.ExecuteQuery();
                        SharePointMetaData spModel = new SharePointMetaData();

                        spModel.operatingGrpCode = (string)setMetadata["OperatingGroupCd"];
                        spModel.operatingGrp = (string)setMetadata["OperatingGroupNm"];
                        spModel.operatingUnitCode = (string)setMetadata["OperatingUnitCd"];
                        spModel.operatingUnit = (string)setMetadata["OperatingUnitNm"];
                        spModel.clientServiceGrpCode = (string)setMetadata["OperatingUnitNm"];
                        spModel.clientServiceGrp = (string)setMetadata["ClientServiceGroupDesc"];
                        spModel.clientName = (string)setMetadata["CustomerNm"];
                        spModel.clientNumber = (string)setMetadata["CustomerNbr"];
                        spModel.contractNumber = (string)setMetadata["ContractNbr"];
                        spModel.strLCUId = (string)setMetadata["LCUId"];
                        spModel.moduleName = (string)setMetadata["ModuleNm"];
                        spModel.moduleId = (string)setMetadata["ModuleId"];
                        spModel.userId = (string)setMetadata["UserId"];
                        spModel.fileTypeCode = (string)setMetadata["FileTypeCd"];
                        spModel.fileType = (string)setMetadata["FileTypeNm"];
                        spModel.documentTypeCode = (string)setMetadata["DocumentTypeCd"];
                        spModel.documentType = (string)setMetadata["DocumentTypeNm"];
                        spModel.documentName = (string)setMetadata["DocumentNm"];

                        string newDestUrl = destinationUrl + fileName;
                        FileInformation loadStream = Microsoft.SharePoint.Client.File.OpenBinaryDirect(context, destUrl);

                        Microsoft.SharePoint.Client.File.SaveBinaryDirect(newClientContext, destinationUrl + fileName,loadStream.Stream, true);

                        SetMeta(spModel, newUri, newDestUrl);

                        loadUrl.DeleteObject();
                        context.ExecuteQuery();

                        result = true;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static string GetFileUrl(int lcuId, ModuleName moduleName, string moduleId, string geographicRegionCd, string sharepointCountryCd, string customerNbr)
        {
            var _region = geographicRegionCd;
            var _country = sharepointCountryCd;
            var clientNbr = customerNbr;
            string region = string.Empty;
            StringBuilder finalUri = new StringBuilder("");
            if (_region == "EL")
            {
                region = "EME";
            }
            if (_region == "AP")
            {
                region = "APA";
            }
            if (_region != "EL" && _region != "AP")
            {
                region = "AMR";
            }
            if (webUrl.Substring(webUrl.Length - 1, 1) != "/")
            {
                webUrl = webUrl + "/";
            }

            finalUri.Append(webUrl);

            finalUri.Append(region);
            finalUri.Append("/");
            finalUri.Append(_country);
            finalUri.Append("/");
            finalUri.Append(docLib);
            finalUri.Append("/");
            finalUri.Append(clientNbr);
            finalUri.Append("/");
            finalUri.Append(lcuId);
            finalUri.Append("/");
            finalUri.Append(moduleName.ToString());
            finalUri.Append("/");
            finalUri.Append(moduleId);
            finalUri.Append("/");
            return finalUri.ToString();
        }
    }
}
