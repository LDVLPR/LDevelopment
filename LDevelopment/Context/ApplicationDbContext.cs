using System.Data.Entity;
using LDevelopment.Models;
using Microsoft.AspNet.Identity.EntityFramework;


namespace LDevelopment.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", false)
        {

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<IdentityUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().HasKey(r => r.Id).ToTable("Roles");
            modelBuilder.Entity<IdentityUserLogin>().HasKey(l => l.UserId).ToTable("UserLogin");
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId }).ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");

            modelBuilder.Entity<Post>().HasMany(x => x.PostTags).WithMany(x => x.TaggedPosts).Map(x => x.ToTable("PostTags"));
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}