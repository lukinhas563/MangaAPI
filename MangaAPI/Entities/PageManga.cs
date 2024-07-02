namespace MangaAPI.Entities
{
    public class PageManga
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public string Url { get; set; }
        public Guid ChapterId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public PageManga(int order, string url, Guid chapterId)
        {
            Id = new Guid();
            Order = order;
            Url = url;
            ChapterId = chapterId;

            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(int order, string url)
        {
            Order = order;
            Url = url;

            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}