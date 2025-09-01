using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;

namespace FactOfHuman.Extensions
{
    public static class OpenApiConfig
    {
        public static void AddOpenApiServices(this IServiceCollection service)
        {
            service.AddOpenApi();
        }
        public static void UseOpenApi(this WebApplication app) {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.Title = "FactOfHuman API";
                    options.Theme = ScalarTheme.BluePlanet;
                    options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
                    options.HideClientButton = true;
                    options.Layout = ScalarLayout.Modern;
                });
            }
            app.UseCors("AllowAll");
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
