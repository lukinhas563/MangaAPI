using MangaAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaAPI.Persistence
{
    public class MangasContext : DbContext
    {
        public DbSet<Mangas> Mangas { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<PageManga> Pages { get; set; }
        public DbSet<User> Users { get; set; }

        public MangasContext(DbContextOptions<MangasContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Mangas>(entity =>
            {
                entity.HasKey(manga => manga.Id);

                entity.Property(manga => manga.Cover).IsRequired();
                entity.Property(manga => manga.Banner).IsRequired();
                entity.Property(manga => manga.Title).HasMaxLength(200).IsRequired();
                entity.Property(manga => manga.Description).HasMaxLength(200).IsRequired();
                entity.Property(manga => manga.Tags).IsRequired();
                entity.Property(manga => manga.Release).IsRequired();
                entity.Property(manga => manga.Status).IsRequired();
                entity.Property(manga => manga.Authors).IsRequired();
                entity.Property(manga => manga.Artists).IsRequired();

                entity.HasMany(manga => manga.Chapters)
                      .WithOne()
                      .HasForeignKey(chapter => chapter.MangaId);
            });

            modelBuilder.Entity<Chapter>(chapter =>
            {
                chapter.HasKey(chapter => chapter.Id);

                chapter.HasIndex(chapter => new { chapter.MangaId, chapter.Number }).IsUnique();

                chapter.Property(chapter => chapter.Title).HasMaxLength(200).IsRequired();
                chapter.Property(chapter => chapter.Number).IsRequired();
                chapter.Property(chapter => chapter.Release).IsRequired();

                chapter.HasMany(chapter => chapter.Pages).WithOne().HasForeignKey(page => page.ChapterId);
            });

            modelBuilder.Entity<PageManga>(pageEntity =>
            {
                pageEntity.HasKey(page => page.Id);

                pageEntity.HasIndex(page => new { page.ChapterId, page.Order }).IsUnique();

                pageEntity.Property(page => page.Order).IsRequired();
                pageEntity.Property(page => page.Url).IsRequired();
            });

            modelBuilder.Entity<User>(userEntity =>
            {
                userEntity.HasKey(user => user.Id);

                userEntity.HasIndex(user => user.Email).IsUnique();
                userEntity.HasIndex(user => user.Username).IsUnique();

                userEntity.Property(user => user.Name).HasMaxLength(200).IsRequired();
                userEntity.Property(user => user.LastName).HasMaxLength(200).IsRequired();
                userEntity.Property(user => user.Username).HasMaxLength(200).IsRequired();
                userEntity.Property(user => user.Email).HasMaxLength(250).IsRequired();
                userEntity.Property(user => user.Password).IsRequired();
                userEntity.Property(user => user.UserType).HasColumnType("text").IsRequired();
            });
        }
    }
}
