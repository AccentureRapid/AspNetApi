namespace LCMS.SessionRepository.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("wp_dst")]
    public partial class wp_dst
    {
        [Key]
        public int Id { get; set; }

        public string DST_FK { get; set; }

        public string SessionName { get; set; }

        public string SessionState { get; set; }

        public string SessionCode { get; set; }

        public string FacilityName { get; set; }

        public DateTime SessionStartDT { get; set; }

        public DateTime SessionEndDT { get; set; }

        public string TimeZone { get; set; }

        public string FacultyCount { get; set; }

        public string LocationName { get; set; }

        public DateTime LocationStartDT { get; set; }

        public DateTime LocationEndDT { get; set; }

        public string VenueName { get; set; }

        public bool DoNotDisplay { get; set; }
    }
}
