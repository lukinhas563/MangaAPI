namespace MangaAPI.Models.Input
{
    public class TesteInputModel
    {
        public IFormFile Cover { get; set; }
        public IFormFile Banner { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public DateTime Release { get; set; }
        public string Status { get; set; }
        public List<string> Authors { get; set; }
        public List<string> Artists { get; set; }
    }
}
