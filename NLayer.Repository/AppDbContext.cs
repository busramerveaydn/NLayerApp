using Microsoft.EntityFrameworkCore;
using NLayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Repository
{
    public class AppDbContext : DbContext
    {
        //options, veri tabanı yolunu start up dosyasından vermek için kullanıldı
        protected AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //HER BİR ENTITY E KARŞILIK BİR DB SET OLUŞTURULDU
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductFeature> ProductFeatures { get; set; }

        //MODEL OLUŞURKEN ÇALIŞACAK OLAN METOT
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //ÖRNEK OLARAK GÖSTERİLDİ BEST PRACTIES AÇISINDAN BURADA YAZILMAMALI
            modelBuilder.Entity<ProductFeature>().HasData(
                new ProductFeature
                {
                    Id = 1,
                    Color = "Kırmızı",
                    Height = 100,
                    Width = 200,
                    ProductId = 1
                },
                new ProductFeature
                {
                    Id = 2,
                    Color = "Mavi",
                    Height = 100,
                    Width = 300,
                    ProductId = 2
                }
            );
            base.OnModelCreating(modelBuilder);
        }

    }
}
