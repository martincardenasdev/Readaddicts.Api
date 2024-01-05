using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : IdentityDbContext<User>(options)
    {
        private readonly IConfiguration _config = configuration;
        // Add-Migration init -OutputDir Data/Migrations
        public new DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UsersGroups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Tier> Tiers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User and tier
            builder.Entity<User>()
                .HasOne(u => u.Tier)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TierId);

            // User and sent messages
            builder.Entity<User>()
                .HasMany(u => u.MessagesSent)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            // User and received messages
            builder.Entity<User>()
                .HasMany(u => u.MessagesReceived)
                .WithOne(m => m.Receiver)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);

            // User and comments
            builder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);

            // User and groups
            builder.Entity<User>()
                .HasMany(u => u.Groups)
                .WithMany(g => g.Users)
                .UsingEntity<UserGroup>(x =>
                {
                    x.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
                    x.HasOne<Group>().WithMany().HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.NoAction);
                });

            // User creator and group
            builder.Entity<Group>()
                .HasOne(g => g.Creator)
                .WithMany(u => u.GroupsCreated)
                .HasForeignKey(g => g.CreatorId);

            // User and post
            builder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.Creator)
                .HasForeignKey(p => p.UserId);

            // User and image
            builder.Entity<User>()
                .HasMany(u => u.UploadedImages)
                .WithOne(i => i.Creator)
                .HasForeignKey(i => i.UserId);

            // Post and comment
            builder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            // Post and group
            builder.Entity<Post>()
                .HasOne(p => p.Group)
                .WithMany(g => g.Posts)
                .HasForeignKey(p => p.GroupId);

            // Post and image
            builder.Entity<Post>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Post)
                .HasForeignKey(i => i.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            // Comment and parent comment
            builder.Entity<Comment>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId);

            // GUID
            builder.Entity<User>().Property(u => u.Id).HasDefaultValueSql("NEWID()");
            builder.Entity<Post>().Property(p => p.Id).HasDefaultValueSql("NEWID()");
            builder.Entity<Comment>().Property(c => c.Id).HasDefaultValueSql("NEWID()");
            builder.Entity<Image>().Property(i => i.Id).HasDefaultValueSql("NEWID()");
            builder.Entity<Group>().Property(g => g.Id).HasDefaultValueSql("NEWID()");
            builder.Entity<UserGroup>().Property(ug => ug.Id).HasDefaultValueSql("NEWID()");
            builder.Entity<Message>().Property(m => m.Id).HasDefaultValueSql("NEWID()");
            builder.Entity<Tier>().Property(t => t.Id).HasDefaultValueSql("NEWID()");
        }
    }
}
