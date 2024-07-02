namespace MangaAPI.Entities
{
    public class Chapter
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Number {  get; set; }
        public DateTime Release {  get; set; }
        public bool IsDeleted { get; set; }
        public List<PageManga> Pages { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid MangaId { get; set; }


        public Chapter(string title, int number, DateTime release, Guid mangaId)
        {
            Id = Guid.NewGuid();
            Title = title;
            Number = number;
            Release = release;
            MangaId = mangaId;

            Pages = new List<PageManga>();
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string title, int number, DateTime release)
        {
            Title = title;
            Number = number;
            Release = release;

            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}