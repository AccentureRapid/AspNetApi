namespace LCMS.SessionRepository.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class SessionModelContainer : DbContext
    {
        public SessionModelContainer()
            : base("name=LCMSDB")
        {
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    throw new UnintentionalCodeFirstException();
        //}

        public virtual DbSet<wp_dst> wp_dst { get; set; }
    }
}
