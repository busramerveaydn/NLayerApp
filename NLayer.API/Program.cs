using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLayer.API.Filters;
using NLayer.API.Middlewares;
using NLayer.Core.DTOs;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;
using NLayer.Repository;
using NLayer.Repository.Repositories;
using NLayer.Repository.UnitOfWorks;
using NLayer.Service.Mapping;
using NLayer.Service.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options => options.Filters.Add(new ValidateFilterAttribute())).AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductDto>()); // yazmýþ olduðumuz filtreyi eklemiþ olduk.

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true; // otomatik oluþturulan modeli kapadýk.
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));

builder.Services.AddScoped(typeof(IProductRepository), typeof(ProductRepository));
builder.Services.AddScoped(typeof(IProductService), typeof(ProductService));


builder.Services.AddScoped(typeof(ICatcegoryRepository), typeof(CategoryRepository));
builder.Services.AddScoped(typeof(ICategoryService), typeof(CategoryService));

builder.Services.AddAutoMapper(typeof(MapProfile));


builder.Services.AddDbContext<AppDbContext>(x =>
    {
        // Migration dosyalarý Repository katmanýnda oluþacak, AppDbContext de Repository katmanýnda, bu durumda AppDbContext in bulunmuþ olduüu assemblysini API tarafýnda uygulamaya haber vermek gerekiyor. option ile birlikte içersine girdik ve 
        x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
        {
            // MigrationsAssembly içerisinde direk isimde yazabilirdik fakat "NLayer.Repository", sonrasýnda isim deðiþikliði olamsý durumunda migration adýnda bozukluk olmasýn diye dinamik olarak çaðýrdýk.
            // Assembly diye bir sýnýfýmýz var, bunun içerisinden bana bir assemby al diyoruz, oda bize bir tip ver diyor. Burada da AppDbContext in assembly sini al ve bunun adýný getir diyoruz.

            option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);
        });
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UserCustomException(); // hata olduðundan aþaðýdaki middleware lerden yukarýda olmasý önemli

app.UseAuthorization();

app.MapControllers();

app.Run();
