using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Services;

namespace NLayer.API.Filters
{
    public class NotFoundFilter<T> : IAsyncActionFilter where T : BaseEntity
    {
        //burada bir service çağırdığımız için bu filter ı program.cs de yazmamız gerek. 
        private readonly IService<T> _service;

        public NotFoundFilter(IService<T> service)
        {
            _service = service;
        }

        // buradaki next eğer bir hata olmazsa devam etmesi için gelmiş bir parametre
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // ProductsController da örneğin bizim GetByIdProduct fonksiyonu bir filtre koymuştuk validation kontrolü.
            // burada daha o fonksiyona gitmeden o id ye ait değer var mı yok mu ona bakmalıyız.

            var idValue = context.ActionArguments.Values.FirstOrDefault();

            if (idValue == null)
            {
                // eğer bana id null geliyorsa yoluna devam etsin bir şeyi karşılaştırmama gerek yok 
                await next.Invoke(); //sen yoluna devam et dedik.
                return;
            }

            var id = (int)idValue;

            // daha sonrasında bir entity var mı bunu kontrol edeceğiz.
            var anyEntity = await _service.AnyAsync(x => x.Id == id);

            if (anyEntity)
            {
                // eğer true gelirse yoluna devam et.
                await next.Invoke();
                return;
            }

            //bu id ye sahip data yok demektir.

            context.Result =  new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(404, $"{typeof(T).Name}({id}) not found."));

        }
    }
}
