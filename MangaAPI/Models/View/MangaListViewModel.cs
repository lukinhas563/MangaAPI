using MangaAPI.Entities;

namespace MangaAPI.Models.View
{
    public class MangaListViewModel
    {
        public Guid Id { get; set; }
        public string Cover { get; set; }
        public string Banner { get; set; }
        public string Title { get; set; }
        public List<string> Tags { get; set; }
        public DateTime Release { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
