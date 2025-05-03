using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Imaar.Blazor.Helpers
{
    public class AddRequiredHeaderParameterHelper : IOperationFilter
    {

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //if (operation.Parameters == null)
            //    operation.Parameters = new ;

            if (context.DocumentName == "v2")

                //operation.Parameters.Add(new OpenApiParameter
                //{
                //    Name = DubaiNowMobilesConsts.APIsHeaderKey,
                //    In = ParameterLocation.Header,
                //    Required = false,
                //    Schema = new OpenApiSchema
                //    {
                //        Type = "string",

                //    }
                //});

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Required = false,
                Description = "ar for Arabic or en for English",
                Schema = new OpenApiSchema
                {
                    Type = "string",

                }
            });
        }
    }
}
