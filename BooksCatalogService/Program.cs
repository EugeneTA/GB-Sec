using AutoMapper;
using BooksCatalogService.Mapper;
using BooksCatalogService.Models.Settings;
using BooksCatalogService.Services;
using BooksCatalogService.Services.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BooksCatalogService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Configure settings

            builder.Services.Configure<BooksCatalogDatabaseSettings>(opt =>
            {
                builder.Configuration.GetSection("Settings:DatabaseOptions").Bind(opt);
            });

            #endregion

            #region Configure services

            builder.Services.AddSingleton<IBookService, BookService>();

            #endregion

            #region Configure Automapper

            // Create mapper configuration from our class MapperProfile
            var mapperConfiguration = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile()));

            // Create mapper from our configuration
            var mapper = mapperConfiguration.CreateMapper();

            // Register our mapper with the app as singletone 
            builder.Services.AddSingleton(mapper);

            #endregion

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}