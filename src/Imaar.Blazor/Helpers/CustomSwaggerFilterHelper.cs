using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Imaar.Blazor.Helpers
{
    public class CustomSwaggerFilterHelper : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (swaggerDoc.Info.Title.Equals("Imaar API For Mobile"))
            {
                var nonMobileRoutes = swaggerDoc.Paths
                    .Where(x => !x.Key.ToLower().Contains("api/mobile"))
                    .ToList();
                nonMobileRoutes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
            }
            if (swaggerDoc.Info.Title.Equals("EtihadRails API"))
            {
                var nonMobileRoutes = swaggerDoc.Paths
                    .Where(x => x.Key.ToLower().Contains("api/app"))
                    .ToList();
                nonMobileRoutes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
            }

        }
    }
}
