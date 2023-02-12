using MongoDB.Driver;
using BooksCatalogService.Models.BooksApi.Models;
using BooksCatalogService.Models.Settings;
using Microsoft.Extensions.Options;

namespace BooksCatalogService.Services.Impl
{
    public class BookService: IBookService
    {
        private readonly IMongoCollection<Book> _books;
        private readonly IOptions<BooksCatalogDatabaseSettings> _settings;

        public BookService(IOptions<BooksCatalogDatabaseSettings> settings)
        {
            _settings = settings;

            var client = new MongoClient(_settings.Value.ConnectionString);
            var database = client.GetDatabase(_settings.Value.DatabaseName);

            _books = database.GetCollection<Book>(_settings.Value.BooksCollectionName);
        }

        public List<Book> Get() =>
            _books.Find(book => true).ToList();

        public Book Get(string id) =>
            _books.Find<Book>(book => book.Id == id).FirstOrDefault();

        public Book Create(Book book)
        {
            _books.InsertOne(book);
            return book;
        }

        public void Update(string id, Book bookIn) =>
            _books.ReplaceOne(book => book.Id == id, bookIn);

        public void Remove(Book bookIn) =>
            _books.DeleteOne(book => book.Id == bookIn.Id);

        public void Remove(string id) =>
            _books.DeleteOne(book => book.Id == id);
    }
}
