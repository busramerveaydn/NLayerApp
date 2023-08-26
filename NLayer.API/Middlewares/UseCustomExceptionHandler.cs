using Microsoft.AspNetCore.Diagnostics;
using NLayer.Core.DTOs;
using NLayer.Service.Exceptions;
using System.Text.Json;

namespace NLayer.API.Middlewares
{
    //bir extention method yazabilmek içinmethot mutlaka statik olmak zorunda
    public static class UseCustomExceptionHandler
    {
        // Program.cs de "var app = builder.Build();" de ki app,  WebApplication classından gelir ve bu class IApplicationBuilder ı miras alır bu durumda aşağıda oluşturduğumuz metotta parametre olarak IApplicationBuilder bu türü alırsak bunu tüm miras almış classlarda exceptionumuzu kullanabiliriz.
        public static void UserCustomException(this IApplicationBuilder app)
        {

            app.UseExceptionHandler(config =>
            {
                // Run: sonlandırıcı bir middleware dır.
                config.Run(async context =>
                {
                    context.Response.ContentType = "application/json";

                    //hatayı alacağız.
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

                    var statusCode = exceptionFeature.Error switch
                    {
                        ClientSideException => 400, // clienttan gelirse 400
                        NotFondException => 404,
                        _ => 500 // default olarak _ tanımladık başka bir hata ise 500 döndürsün
                    };
                    context.Response.StatusCode = statusCode;

                    var response = CustomResponseDto<NoContentDto>.Fail(statusCode, exceptionFeature.Error.Message); // geriye dönecek br response oluşturduk fakat bu bir tip
                    //bizim bunu geriye dönmemiz için json a Serializer etmemiz gerekiyor.


                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));

                });
            });
        }
    }
}