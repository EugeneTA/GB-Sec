using BooksCatalogService.Models.BooksApi.Models;

namespace BooksCatalogService.Services
{
    public interface IBookService
    {
        public List<Book> Get();
        public Book Get(string id);
        public Book Create(Book book);
        public void Update(string id, Book bookIn);
        public void Remove(Book bookIn);
        public void Remove(string id);
    }
}
