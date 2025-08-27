using APIWeaver;
using DotNetEnv;
using FactOfHuman.Data;
using FactOfHuman.Mapper;
using FactOfHuman.Repository.IService;
using FactOfHuman.Repository.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

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
        scheme.In = ParameterLocation.Header;
        scheme.Type = SecuritySchemeType.Http;
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
        options.RequireHttpsMetadata = false;
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
    .AddGoogle("Google",options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.SignInScheme = "Cookies";
    });
// Register AuthService for dependency injection
builder.Services.AddScoped<IAuthService, AuthService>();
//Auto Mapper Configurations
builder.Services.AddAutoMapper(cfg => {
    cfg.LicenseKey = Environment.GetEnvironmentVariable("AUTO_MAP_KEY") ?? string.Empty;
}, typeof(MappingProfile));
var app = builder.Build();
app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "FactOfHuman API";
        options.Theme = ScalarTheme.BluePlanet;
        options.DefaultHttpClient = new(ScalarTarget.CSharp,ScalarClient.HttpClient);
        options.CustomCss = "";
        options.ShowSidebar = true;
    });
}

app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();
