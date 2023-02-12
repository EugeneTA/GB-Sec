namespace BooksCatalogService.Models.Settings
{
    public class BooksCatalogDatabaseSettings : IBooksCatalogDatabaseSettings
    {
        public string? BooksCollectionName { get; set; }
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
    }
}
