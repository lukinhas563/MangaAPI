namespace MangaAPI.Models.View
{
    public class ChapterListViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public DateTime Release { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
