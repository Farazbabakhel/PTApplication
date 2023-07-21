
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PTApplication.Models.ORM;

namespace PTApplication.Models.DBContext
{
    public class PTAppDBContext : DbContext
    {
        public PTAppDBContext(DbContextOptions<PTAppDBContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<BioQuestion> BioQuestions { get; set; }

        public DbSet<BioOptions> BioOptions { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<UserTag> UserTags { get; set; }
        public DbSet<UserBioAnswer> UserBioAnswers { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<PTSetting> PTSettings { get; set; }
        public DbSet<Recharge> Recharge { get; set; }
        public DbSet<Balance> Balance { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Conversion> Conversions { get; set; }
        public DbSet<RequestReply> RequestReply { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {optionsBuilder.UseSqlServer(@"Server=.\SQLExpress;Database=SchoolDB;Trusted_Connection=True;");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                //entity.SetTableName("tbl_" + entity.ClrType.Name.ToLower());
                entity.SetTableName("tbl_" + entity.ClrType.Name.ToLower());
            }




            //modelBuilder.Entity<User>(entity =>
            //{
            //    entity.Property(e => e.firstName).HasColumnName("FirstName")
            //        .HasMaxLength(250)
            //        .IsUnicode(false);

            //    entity.Property(e => e.fullName).HasColumnName("FullName")
            //        .HasMaxLength(250)
            //        .IsUnicode(false);

            //    entity.Property(e => e.username).HasColumnName("Username")
            //        .HasMaxLength(250)
            //        .IsUnicode(false);

            //    entity.Property(e => e.email).HasColumnName("Email")
            //        .HasMaxLength(250)
            //        .IsUnicode(false);

            //    entity.Property(e => e.cell).HasColumnName("cell")
            //        .HasMaxLength(250)
            //        .IsUnicode(false);

            //    entity.Property(e => e.creationDate).HasColumnName("CreationDate")
            //        .HasMaxLength(250)
            //        .IsUnicode(false);

            //    entity.Property(e => e.createdBy).HasColumnName("createdBy")
            //        .HasMaxLength(250)
            //        .IsUnicode(false);

                //entity.Property(e => e.firstName).HasColumnName("FirstName")
                //    .HasMaxLength(250)
                //    .IsUnicode(false);

                //entity.Property(e => e.firstName).HasColumnName("FirstName")
                //    .HasMaxLength(250)
                //    .IsUnicode(false);

                //entity.Property(e => e.firstName).HasColumnName("FirstName")
                //    .HasMaxLength(250)
                //    .IsUnicode(false);

                //entity.HasOne(d => d.Teacher)
                //    .WithMany(p => p.Course)
                //    .HasForeignKey(d => d.TeacherId)
                //    .OnDelete(DeleteBehavior.Cascade)
                //    .HasConstraintName("FK_Course_Teacher");
            //});

            //modelBuilder.Entity<VirtualRoute>(entity => entity.Property(e => e.OrganizationID).HasColumnName("RouteID"));
            //modelBuilder.Entity<VirtualRouteStation>(entity => entity.Property(e => e.OrganizationID).HasColumnName("RouteID")); 


            //modelBuilder.Entity<User>().HasNoKey(m => m.AssignRoles);
            //modelBuilder.Entity<User>().HasKey(m => m.Guid);
            //entity.GetTableName()
            //entity.ClrType.Name.

            // modelBuilder.Entity<User>().HasNoKey();
            //modelBuilder.Entity<User>(e => e.Property(o => o.UserID).HasColumnType("int"));

            //modelBuilder.Entity<User>(entity =>
            //{
            //    entity.Property(e => e.UserID).HasColumnName("UserID");
            //    entity.Property(e => e.OrganizationID).HasColumnName("OrganizationID");//.HasMaxLength(50).IsUnicode(false);
            //    entity.Property(e => e.DepartmentID).HasColumnName("DepartmentID");
            //    //entity.HasOne(d => d.Standard).WithMany(p => p.Student)HasForeignKey(d => d.StandardId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Student_Standard");
            //});


            //    modelBuilder
            //.Entity<User>(
            //    eb =>
            //    {
            //        eb.HasNoKey();
            //       // eb.ToView("View_BlogPostCounts");
            //    });



        }
    }
}
