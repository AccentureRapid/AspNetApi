using System;
using System.Configuration;
using System.IO;

namespace LCMS.Common
{
    public class SimpleLogger
    {
        private string logFile;
        private StreamWriter writer;
        private FileStream fileStream = null;



        public SimpleLogger(string fileName)
        {
            logFile = fileName;
            CreateDirectory(logFile);
            var enabled = ConfigurationManager.AppSettings["log-enabled"];
            if (enabled == "0")
            {
                this.LogEnabled = false;
            }
            else
            {
                this.LogEnabled = true;
            }
        }

        public bool LogEnabled { get; set; }
        public void Log(string info)
        {

            try
            {
                if (LogEnabled)
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(logFile);
                    if (!fileInfo.Exists)
                    {
                        fileStream = fileInfo.Create();
                        writer = new StreamWriter(fileStream);
                    }
                    else
                    {
                        fileStream = fileInfo.Open(FileMode.Append, FileAccess.Write);
                        writer = new StreamWriter(fileStream);
                    }
                    writer.WriteLine(DateTime.Now + ": " + info); 
                }

            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }

        public void CreateDirectory(string infoPath)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(infoPath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }
    }
}
