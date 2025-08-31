using FactOfHuman.Extensions;
var builder = WebApplication.CreateBuilder(args);
builder.AddDependencies();
var app = builder.Build();
app.UseOpenApi();
    