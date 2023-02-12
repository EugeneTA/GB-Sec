namespace BooksCatalogService.Models.Settings
{
    public interface IBooksCatalogDatabaseSettings
    {
        string BooksCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
