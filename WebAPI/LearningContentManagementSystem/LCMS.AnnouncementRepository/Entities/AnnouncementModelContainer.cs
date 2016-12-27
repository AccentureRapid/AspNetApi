namespace LCMS.AnnouncementRepository.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AnnouncementModelContainer : DbContext
    {
        public AnnouncementModelContainer()
            : base("name=LCMSDB")
        {
        }
    
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    throw new UnintentionalCodeFirstException();
        //}
    
        public virtual DbSet<wp_posts> wp_posts { get; set; }

        public virtual DbSet<wp_postmeta> wp_postmeta { get; set; }

        public virtual DbSet<wp_users> wp_users { get; set; }
    }
}
