using System.Collections.Generic;

namespace MangaAPI.Entities
{
    public class Mangas
    {
        public Guid Id { get; set; }
        public string Cover { get; set; }
        public string Banner { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public DateTime Release {  get; set; }
        public string Status { get; set; }
        public List<string> Authors {  get; set; }
        public List<string> Artists {  get; set; }
        public List<Chapter> Chapters { get; set; }
        public int Views {  get; set; }
        public int Likes {  get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt {  get; set; }

        public Mangas(string cover, string banner, string title, string description, List<string> tags, DateTime release, string status, List<string> authors, List<string> artists)
        {
            Id = Guid.NewGuid();
            Cover = cover;
            Banner = banner;
            Title = title;
            Description = description;
            Tags = tags;
            Release = release;
            Status = status;
            Authors = authors;
            Artists = artists;

            Chapters = new List<Chapter>();
            Views = 0;
            Likes = 0;
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string cover, string banner, string title, string description, List<string> tags, DateTime release, string status, List<string> authors, List<string> artists)
        {
            Cover = cover;
            Banner = banner;
            Title = title;
            Description = description;
            Tags = tags;
            Release = release;
            Status = status;
            Authors = authors;
            Artists = artists;

            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Liked()
        {
            Likes++;
        }

        public void Viewed()
        {
            Views++;
        }

    }
}
