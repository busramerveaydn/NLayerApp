
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Caching
{
    public class ProductServiceWithCaching : IProductService
    {
        //Öncelikle tüm prooduct laroı tutabileceğiz bir key oluşturacağız.
        private const string CacheProductKey = "productsCache";

        // aşağıdaki bazı methotlarda mapper kullanacağız
        private readonly IMapper _mapper;

        //in memory cache için IMemoryCache e ihtiyacımız var
        private readonly IMemoryCache _memoryCache;

        private readonly IProductRepository _repository;

        //db ye yansıtmak için
        private readonly IUnitOfWork _unitOfWork;

        public ProductServiceWithCaching(IMapper mapper, IMemoryCache memoryCache, IProductRepository repository, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _memoryCache = memoryCache;
            _repository = repository;
            _unitOfWork = unitOfWork;

            //ilk nesne örneği oluşturulduğu anda cache leme yapmamız gerekiyor.
            // değeri almaya çalış /(geriye bool döner)
            if (!_memoryCache.TryGetValue(CacheProductKey, out _)) // _ yi koymamızdaki sebep biz cache deki datayı almak istemiyoruz var mı yok mu kontrol etmek istiyoruz._ ile memoryde allocate etmesini engelliyoruz.
            {
                // yoksa
                _memoryCache.Set(CacheProductKey, _repository.GetAll().ToList()); // datayı reposittorydn alır


            }
        }

        public Task<Product> AddAsync(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> AddRangeAsync(IEnumerable<Product> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Expression<Func<Product, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CustomResponseDto<List<ProductWithCategoryDto>>> GetProductsWithCategory()
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRangeAsync(IEnumerable<Product> entities)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Product entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Product> Where(Expression<Func<Product, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
