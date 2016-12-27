using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.Common
{
    public class ReturnResult
    {
        public ReturnStatusCode ReturnCode { get; set; }

        public string ReturnMessage { get; set; }

        public dynamic Content { get; set; }
    }

    public enum ReturnStatusCode
    {
        Failed = -1,
        Succeed
    }
}
