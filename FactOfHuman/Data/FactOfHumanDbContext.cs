using Microsoft.EntityFrameworkCore;
using FactOfHuman.Models;
namespace FactOfHuman.Data
{
    public class FactOfHumanDbContext : DbContext
    {
        public FactOfHumanDbContext(DbContextOptions<FactOfHumanDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<Fact> Facts { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Bookmark> BookMarks { get; set; } = null!;
        public DbSet<Reaction> Reactions { get; set; } = null!;
        public DbSet<PostBlock> PostBlocks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Name)
                .IsUnique(); 
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>(); // Enum to string
            modelBuilder.Entity<User>()
                .Property(u => u.AuthProvider)
                .HasConversion<string>(); // Enum to string
            // Category
            modelBuilder.Entity<Category>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            // Tag
            modelBuilder.Entity<Tag>()
                .HasKey(t => t.Id);
            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Slug)
                .IsUnique();

            // Post
            modelBuilder.Entity<Post>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany() // 1 User -> nhiều Post
                .HasForeignKey(p => p.AuthorId);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany() // 1 Category -> nhiều Post
                .HasForeignKey(p => p.CategoryId);
            modelBuilder.Entity<Post>()
                .HasIndex(p => p.Slug)
                .IsUnique();
            //Block Post
            modelBuilder.Entity<PostBlock>()
                .HasKey(pb => pb.Id);
            modelBuilder.Entity<PostBlock>()
                .HasIndex(pb => pb.Id)
                .IsUnique();
            modelBuilder.Entity<PostBlock>()
                .HasOne(pb => pb.Post)
                .WithMany(p=>p.Block) // 1 Post có nhiều Post block
                .HasForeignKey(pb => pb.PostId);
            // Many-to-Many Post <-> Tag
            modelBuilder.Entity<Post>()
                .HasMany(p => p.Tags)
                .WithMany();

            // Fact
            modelBuilder.Entity<Fact>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Fact>()
                .HasOne(f => f.Author)
                .WithMany()
                .HasForeignKey(f => f.AuthorId);

            modelBuilder.Entity<Fact>()
                .HasOne(f => f.Category)
                .WithMany()
                .HasForeignKey(f => f.CategoryId);

            modelBuilder.Entity<Fact>()
                .HasMany(f => f.Tags)
                .WithMany();

            // Comment
            modelBuilder.Entity<Comment>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany()
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Fact)
                .WithMany()
                .HasForeignKey(c => c.FactId)
                .OnDelete(DeleteBehavior.Cascade);

            // Reaction
            modelBuilder.Entity<Reaction>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            // Bookmark
            modelBuilder.Entity<Bookmark>()
                .HasKey(b => new { b.UserId, b.TargetId, b.TargetType }); // composite key
            modelBuilder.Entity<Bookmark>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId);
            modelBuilder.Entity<Bookmark>()
                .Property(b => b.TargetType)
                .HasConversion<string>(); // Enum to string
        }
    }
}
