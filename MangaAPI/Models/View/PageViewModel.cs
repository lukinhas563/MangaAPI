namespace MangaAPI.Models.View
{
    public class PageViewModel
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
