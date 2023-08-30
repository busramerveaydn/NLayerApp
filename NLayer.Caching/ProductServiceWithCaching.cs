
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;
using NLayer.Service.Exceptions;
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
                //_memoryCache.Set(CacheProductKey, _repository.GetAll().ToList()); // datayı reposittorydn alır
                _memoryCache.Set(CacheProductKey, _repository.GetProductsWithCategory()); 


            }
        }

        public async Task<Product> AddAsync(Product entity)
        {
            //db ye ekleme yapıyoruz.
            await _repository.AddAsync(entity);

            //veri tabanına yansıtıyoruz.
            await _unitOfWork.CommitAsync();

            //cache leme yapıyoruz. her çağırıldığında datayı sıfırdan alıp cache liyor.
            await CacheAllProductsAsync();

            return entity;
        }
        public async Task<IEnumerable<Product>> AddRangeAsync(IEnumerable<Product> entities)
        {
            await _repository.AddRangeAsync(entities);

            await _unitOfWork.CommitAsync();

            await CacheAllProductsAsync();

            return entities;
        }
        public async Task RemoveAsync(Product entity)
        {
             _repository.Remove(entity);

            await  _unitOfWork.CommitAsync();

            await CacheAllProductsAsync();

            
        }
        public async Task RemoveRangeAsync(IEnumerable<Product> entities)
        {
            _repository.RemoveRange(entities);

            await _unitOfWork.CommitAsync();

            await CacheAllProductsAsync();
        }
        public async Task UpdateAsync(Product entity)
        {
            _repository.Update(entity);

            await _unitOfWork.CommitAsync();

            await CacheAllProductsAsync();
        }
        public IQueryable<Product> Where(Expression<Func<Product, bool>> expression)
        {
            //artık cache den dönücez

            return _memoryCache.Get<List<Product>>(CacheProductKey).Where(expression.Compile()).AsQueryable();
        }
        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return Task.FromResult(_memoryCache.Get<IEnumerable<Product>>(CacheProductKey));
        }
        public  Task<Product> GetByIdAsync(int id)
        {
            var product = _memoryCache.Get<List<Product>>(CacheProductKey).FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                throw new NotFoundException($"{typeof(Product).Name}({id}) not found");
            }

            return Task.FromResult(product);
        }
        public async Task<CustomResponseDto<List<ProductWithCategoryDto>>> GetProductsWithCategory()
        {
            var products = _memoryCache.Get<IEnumerable<Product>>(CacheProductKey); //await _repository.GetProductsWithCategory();
            var productsWithCategoryDto = _mapper.Map<List<ProductWithCategoryDto>>(products);
            return  CustomResponseDto<List<ProductWithCategoryDto>>.Success(200,productsWithCategoryDto);
        }
        public Task<bool> AnyAsync(Expression<Func<Product, bool>> expression)
        {
            throw new NotImplementedException();
        }

        //her Seferinde her metot un içerisine cache yapmamak için ayrı bir metot oluşturduk.
        public async Task CacheAllProductsAsync()
        {
            _memoryCache.Set(CacheProductKey, await _repository.GetAll().ToListAsync());
        }
    }
}
