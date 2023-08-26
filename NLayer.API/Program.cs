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

builder.Services.AddControllers(options => options.Filters.Add(new ValidateFilterAttribute())).AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductDto>()); // yazm�� oldu�umuz filtreyi eklemi� olduk.

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true; // otomatik olu�turulan modeli kapad�k.
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
        // Migration dosyalar� Repository katman�nda olu�acak, AppDbContext de Repository katman�nda, bu durumda AppDbContext in bulunmu� oldu�u assemblysini API taraf�nda uygulamaya haber vermek gerekiyor. option ile birlikte i�ersine girdik ve 
        x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
        {
            // MigrationsAssembly i�erisinde direk isimde yazabilirdik fakat "NLayer.Repository", sonras�nda isim de�i�ikli�i olams� durumunda migration ad�nda bozukluk olmas�n diye dinamik olarak �a��rd�k.
            // Assembly diye bir s�n�f�m�z var, bunun i�erisinden bana bir assemby al diyoruz, oda bize bir tip ver diyor. Burada da AppDbContext in assembly sini al ve bunun ad�n� getir diyoruz.

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

app.UserCustomException(); // hata oldu�undan a�a��daki middleware lerden yukar�da olmas� �nemli

app.UseAuthorization();

app.MapControllers();

app.Run();
