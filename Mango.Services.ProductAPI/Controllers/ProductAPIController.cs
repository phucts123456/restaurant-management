using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        protected ResponseDto _response;
        private readonly IProductRepository _repository;

        public ProductAPIController(IProductRepository repository)
        {
            this._response = new ResponseDto();
            _repository = repository;
        }
        [HttpGet]
        public async Task<ResponseDto> Get()
        {
            try
            {
               IEnumerable<ProductDto> products = await _repository.GetProducts();
                _response.Result= products;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        //[Route("{id}")]
		[HttpGet("{id}")]
        public async Task<object> GetById(int id)
        {
            try
            {
                ProductDto product = await _repository.GetProductById(id);
                _response.Result = product;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
		[Authorize]
		[HttpPost]
        public async Task<object> Post([FromBody] ProductDto product)
        {
            try
            {
                ProductDto model = await _repository.CreateUpdateProduct(product);
                _response.Result = model;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
		[Authorize]
		[HttpPut]
        public async Task<object> Put([FromBody] ProductDto product)
        {
            try
            {
                ProductDto model = await _repository.CreateUpdateProduct(product);
                _response.Result = model;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
		[Authorize(Roles = SD.Admin)]
		[HttpDelete("{id}")]
        public async Task<object> Delete(int id)
        {
            try
            {
                bool idSuccess = await _repository.DeleteProduct(id);
                _response.Result = idSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}
