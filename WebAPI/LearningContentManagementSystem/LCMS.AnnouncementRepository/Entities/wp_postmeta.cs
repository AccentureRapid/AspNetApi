namespace LCMS.AnnouncementRepository.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("wp_postmeta")]
    public partial class wp_postmeta
    {
        [Key]
        public int meta_id { get; set; }

        public int post_id { get; set; }

        public string meta_key { get; set; }

        public string meta_value { get; set; }

        [ForeignKey("post_id")]
        public virtual wp_posts wp_posts { get; set; }
    }
}
