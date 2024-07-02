using MangaAPI.Entities;
using MangaAPI.Models.Pagination;

namespace MangaAPI.Models.View
{
    public class MangaViewModel
    {
        public Guid Id { get; set; }
        public string Cover { get; set; }
        public string Banner { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public DateTime Release { get; set; }
        public string Status { get; set; }
        public List<string> Authors { get; set; }
        public List<string> Artists { get; set; }
        public PageList<ChapterListViewModel> Chapters { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
