using APIWeaver;
using DotNetEnv;
using FactOfHuman.Data;
using FactOfHuman.Mapper;
using FactOfHuman.Repository.IService;
using FactOfHuman.Repository.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

namespace FactOfHuman.Extensions
{
    public static class DependenciesConfig
    {
        public static void AddDependencies(this WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddControllers();
            // Load .env file
            Env.Load();
            builder.Configuration.AddEnvironmentVariables();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            var connectionString = builder.Configuration.GetConnectionString("Conn");
            builder.Services.AddOpenApi(options =>
            {
                options.AddSecurityScheme(JwtBearerDefaults.AuthenticationScheme, scheme =>
                {
                    scheme.In = Microsoft.OpenApi.Models.ParameterLocation.Header;
                    scheme.Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http;
                    scheme.Scheme = JwtBearerDefaults.AuthenticationScheme;
                    scheme.BearerFormat = "JWT";
                });
                options.AddAuthResponse();
            });
            // more specific configuration for JSON serialization
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.WriteIndented = true;
                });
            // Add DbContext with SQL Server provider
            builder.Services.AddDbContext<FactOfHumanDbContext>(options =>
                options.UseSqlServer(connectionString)
            );
            //Adding CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            // Add Jwt Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false; ;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Token"]!))
                    };
                })
                .AddCookie("Cookies")
                .AddGoogle("Google", options =>
                {
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
                    options.SignInScheme = "Cookies";
                })
                .AddFacebook(facebookoptions => 
                { 
                    facebookoptions.AppId = builder.Configuration["Authentication:Facebook:AppId"]!;
                    facebookoptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]!;
                }
                );
            // Register AuthService for dependency injection
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<IPostBlockService, PostBlockService>();
            builder.Services.AddScoped<ICommentService, CommentService>();

            //Auto Mapper Configurations
            builder.Services.AddAutoMapper(cfg => {
                cfg.LicenseKey = Environment.GetEnvironmentVariable("AUTO_MAP_KEY") ?? string.Empty;
            }, typeof(MappingProfile));
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApiServices();
        }
    }
}
