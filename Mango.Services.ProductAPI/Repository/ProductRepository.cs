using AutoMapper;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public ProductRepository(IMapper mapper, ApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {
            Product product = _mapper.Map<ProductDto, Product>(productDto);
            if(product.ProductId > 0)
            {
                _context.Products.Update(product);
            }
            else
            {
                _context.Products.Add(product);
            }
            await _context.SaveChangesAsync(); ;
            return _mapper.Map<Product, ProductDto>(product);
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            try
            {
                Product product = await _context.Products.FirstOrDefaultAsync(u => u.ProductId == productId);
                if (product == null)
                {
                    return false;
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<ProductDto> GetProductById(int productId)
        {

            Product product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            List<Product> productList = await _context.Products.ToListAsync();
            return _mapper.Map<List<ProductDto>>(productList);
        }
    }
}
