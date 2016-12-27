using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS.AnnouncementRepository.DTO
{
    public class AnnouncementDTO
    {
        public string FacilityName { get; set; }

        //public string FacilityCode { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Priority { get; set; }

        public bool IsActive { get; set; }

        public string AnnouncementDesc { get; set; }
    }

    public class wp_postmeta_entity
    {
        public int meta_id { get; set; }

        public int post_id { get; set; }

        public string meta_key { get; set; }

        public string meta_value { get; set; }

    }
}
