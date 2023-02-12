namespace BooksCatalogService.Models.Request
{
    public class CreateBookRequest
    {
        public string BookName { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public string Author { get; set; }
    }
}
