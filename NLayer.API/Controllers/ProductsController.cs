using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLayer.API.Filters;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Services;
using System.Reflection.PortableExecutable;

namespace NLayer.API.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    // Açıklama: yukarıdaki kodlar miras alınan base de olduğu için burada kullanılmaya gerek kalmadı
    public class ProductsController : CustomBaseController
    {
        private readonly IMapper _mapper;
        //private readonly IService<Product> _service; aşağıdaki servis olduğu için bu servisi kapadık
        private readonly IProductService _service;

        public ProductsController(IMapper mapper/*, IService<Product> service*/, IProductService productService)
        {
            _mapper = mapper;
            //_service = service;
            _service = productService;
        }

        [HttpGet("GetProductWithCategory")]
        public async Task<IActionResult> GetProductWithCategory()
        {
            return CreateActionResult(await _service.GetProductsWithCategory());
        }



        [HttpGet]
        public async Task<IActionResult> All()
        {
            var products = await _service.GetAllAsync();

            var productsDto = _mapper.Map<List<ProductDto>>(products.ToList());

            return CreateActionResult(CustomResponseDto<List<ProductDto>>.Success(200, productsDto));
        }


        [ServiceFilter(typeof(NotFoundFilter<Product>))]
        // GET/api/products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            var productDto = _mapper.Map<ProductDto>(product);
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(200, productDto));
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProductDto productDto)
        {
            var product = await _service.AddAsync(_mapper.Map<Product>(productDto));
            var productsDto = _mapper.Map<ProductDto>(product);
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(201, productsDto));
        }

        [HttpPut]
        public async Task<IActionResult> Update(ProductUpdateDto productUpdateDto)
        {
            await _service.UpdateAsync(_mapper.Map<Product>(productUpdateDto));

            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var product = await _service.GetByIdAsync(id);
            await _service.RemoveAsync(product);
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));

        }
    }
}
