using CardStorageService.Config;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using ServiceUtils;
using System.Text;

namespace CardStorageService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            byte[] key = { 1, 6, 9, 2, 3, 67, 92 };

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

            CacheProvider cacheProvider = new CacheProvider(key, null);

            try
            {
                // Cache db connection file generated with GenerateDatabaseConnectionString
                byte[] connectionCache = File.ReadAllBytes(builder.Configuration["Settings:DatabaseOptions:ConnectionString"]);
                DatabaseConnectionInfo databaseConnectionInfo = cacheProvider.GetDatabaseConnectionFromCahce(connectionCache);
                builder.Services.AddDbContext<CardStorageServiceDbContext>(options =>
                {
                    options.UseSqlServer(databaseConnectionInfo.ToString());
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error get Db connection string. Error message: {ex.Message}");
            }

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

            #region Configure settings

            builder.Host.ConfigureAppConfiguration((hostingContext, options) => 
            {
                if (hostingContext.HostingEnvironment.IsDevelopment())
                {
                    options.AddJsonFile("controllersSetting.Development.json", optional: false, reloadOnChange: true);
                }
                else
                {
                    options.AddJsonFile("controllersSetting.json", optional: false, reloadOnChange: true);

                }
                
            });
            builder.Services.Configure<AccountControllerConfig>(builder.Configuration.GetSection("AccountController"));
            builder.Services.Configure<ClientControllerConfig>(builder.Configuration.GetSection("ClientController"));
            builder.Services.Configure<CardControllerConfig>(builder.Configuration.GetSection("CardController"));

            #endregion

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