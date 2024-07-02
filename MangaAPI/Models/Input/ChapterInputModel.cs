using MangaAPI.Entities;

namespace MangaAPI.Models.Input
{
    public class ChapterInputModel
    {
        public string Title { get; set; }
        public int Number { get; set; }
        public DateTime Release { get; set; }

    }
}
