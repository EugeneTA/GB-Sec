using CardStorageService.Data;
using CardStorageService.Models.Requests.Account;
using CardStorageService.Models.Requests.Authentication;
using CardStorageService.Models.Validators;
using CardStorageService.Services;
using CardStorageService.Services.Impl;
using EmployeeService.Models.Validators;
using EmployeeService.Services.Repositories.Impl;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using System.Text;

namespace CardStorageService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Logger

            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestQuery;
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
                logging.RequestHeaders.Add("Authorization");
                logging.RequestHeaders.Add("X-Real-IP");
                logging.RequestHeaders.Add("X-Forwarded-For");
            });

            // NLog integration
            builder.Host.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();

            }).UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = true });

            #endregion

            #region Configure EF DBContext(CardStorageService.Data)

            builder.Services.AddDbContext<CardStorageServiceDbContext>(options => 
            {
                options.UseSqlServer(builder.Configuration["Settings:DatabaseOptions:ConnectionString"]);
            });

            #endregion

            #region Configure Authenticate 

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new
                TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthenticateService.SecretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            #endregion

            #region Configure FluentValidator

            // ѕроверка поступающих данных в запросе
            builder.Services.AddScoped<IValidator<AuthenticationRequest>, AuthenticationRequestValidator>();

            builder.Services.AddScoped<IValidator<UpdateAccountRequest>, UpdateAccountRequestValidator>();
            builder.Services.AddScoped<IValidator<string>, EmailValidator>();
            builder.Services.AddScoped<IValidator<string>, PasswordValidator>();
            #endregion

            #region Confugure Repositories/Services

            builder.Services.AddScoped<IClientRepositoryService, ClientRepository>();
            builder.Services.AddScoped<ICardRepositoryService, CardRepository>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddSingleton<IAuthenticateService, AuthenticateService>();

            #endregion


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Card Storage Service", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Enable HTTP requests Log
            app.UseHttpLogging();

            app.MapControllers();

            app.Run();
        }
    }
}