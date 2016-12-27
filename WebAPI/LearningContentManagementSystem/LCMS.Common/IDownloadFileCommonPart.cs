using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Configuration;

namespace LCMS.Common
{
    public interface IDownloadFileCommonPart
    {
        byte[] DownloadFile(string tempFolder, string fileName);
        string GetFileType(string fileName);
    }
}
