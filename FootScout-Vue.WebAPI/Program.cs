using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.HubManager;
using FootScout_Vue.WebAPI.Models.Constants;
using FootScout_Vue.WebAPI.Repositories.Classes;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using FootScout_Vue.WebAPI.Services.Classes;
using FootScout_Vue.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace FootScout_Vue.WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Connection string z baz¹ danych MS SQL
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MSSQLConnectionString") ??
                    throw new InvalidOperationException("MS SQL connection string is not found!"));
            });

            // Identity u¿ytkownika ze wsparciem dla ról
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Domyœlny schemat uwierzytelniania
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // JWT Bearer Token
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                };
            });

            // Zasady autoryzacji
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminRights", policy =>
                    policy.RequireRole(Role.Admin));
                options.AddPolicy("UserRights", policy =>
                    policy.RequireRole(Role.User));
                options.AddPolicy("AdminOrUserRights", policy =>
                    policy.RequireRole(Role.Admin, Role.User));
            });

            // Serwisy (bez kontaktu z baz¹ danych)
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICookieService, CookieService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IMessageService, MessageService>();

            // Repozytoria (kontakt z baz¹ danych)
            builder.Services.AddScoped<IPlayerPositionRepository, PlayerPositionRepository>();
            builder.Services.AddScoped<IPlayerFootRepository, PlayerFootRepository>();
            builder.Services.AddScoped<IOfferStatusRepository, OfferStatusRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IClubHistoryRepository, ClubHistoryRepository>();
            builder.Services.AddScoped<IAchievementsRepository, AchievementsRepository>();
            builder.Services.AddScoped<ISalaryRangeRepository, SalaryRangeRepository>();
            builder.Services.AddScoped<IPlayerAdvertisementRepository, PlayerAdvertisementRepository>();
            builder.Services.AddScoped<IFavoritePlayerAdvertisementRepository, FavoritePlayerAdvertisementRepository>();
            builder.Services.AddScoped<IClubOfferRepository, ClubOfferRepository>();
            builder.Services.AddScoped<IProblemRepository, ProblemRepository>();

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(Program));

            // Password hasher
            builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

            // Dostêp do w³aœciwoœci HttpContext (pliki cookie)
            builder.Services.AddHttpContextAccessor();

            // Serwis do komunikacji w czasie rzeczywistym (SignalR)
            builder.Services.AddSignalR();

            // Controller handler
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Uwierzytelnianie Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FootScout API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            // Zasady CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVueDevClient",
                    b =>
                    {
                        b.WithOrigins("http://localhost:5173")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(origin => true)
                            .AllowCredentials();
                    });
            });

            var app = builder.Build();

            // Proces ¿¹dañ HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Wykorzystanie zasad CORS
            app.UseCors("AllowVueDevClient");
            app.UseHttpsRedirection();

            // Middleware uwierzytelniania
            app.UseAuthentication();
            app.UseAuthorization();

            // Endpointy
            app.MapControllers();
            app.MapHub<ChatHub>("/chathub");

            // Tworzenie zakresów dla apliakcji
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await AppSeeder.Seed(services); // Seeder do wype³niania tabel bazy danych podczas pierwszego uruchomienia aplikacji
            }

            app.Run();
        }
    }
}