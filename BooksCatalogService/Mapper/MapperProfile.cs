using AutoMapper;
using BooksCatalogService.Models.BooksApi.Models;
using BooksCatalogService.Models.Request;

namespace BooksCatalogService.Mapper
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateBookRequest, Book>();
        }

    }
}
