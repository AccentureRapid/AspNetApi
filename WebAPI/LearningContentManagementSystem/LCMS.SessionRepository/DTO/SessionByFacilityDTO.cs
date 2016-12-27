using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.SessionRepository.DTO
{
    public class SessionByFacilityDTO
    {
        public string OrderID { get; set; }

        public string VenueName { get; set; }

        public string StartDT { get; set; }

        public string EndDate { get; set; }

        public string LocationName { get; set; }
    }
}
