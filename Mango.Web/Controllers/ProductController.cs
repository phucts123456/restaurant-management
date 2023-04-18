using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductServices _productServices;

        public ProductController(IProductServices productServices)
        {
            _productServices = productServices;
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto> list = new();
            var accesstoken = await HttpContext.GetTokenAsync("access_token");

            var response = await _productServices.GetAllProductsAsync<ResponseDto>(accesstoken);
            if(response != null && response.IsSuccess) 
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(list);
        }

        public  IActionResult ProductCreate()
        {      
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if(ModelState.IsValid)
            {
                var accesstoken = await HttpContext.GetTokenAsync("access_token");
                var response = await _productServices.CreateProductAsync<ResponseDto>(model,accesstoken);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ProductEdit(int productId)
        {
            if(ModelState.IsValid)
            {
                var accesstoken = await HttpContext.GetTokenAsync("access_token");
                var response = await _productServices.GetAllProductByIdAsync<ResponseDto>(productId,accesstoken);
                if (response != null && response.IsSuccess)
                {
                    ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                    return View(model);
                }
            }
            return NotFound();
        }  
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(ProductDto model)
        {
            if(ModelState.IsValid)
            {
                var accesstoken = await HttpContext.GetTokenAsync("access_token");
                var response = await _productServices.UpdateProductAsync<ResponseDto>(model,accesstoken);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProductDelete(int productId)
        {
            if(ModelState.IsValid)
            {
                var accesstoken = await HttpContext.GetTokenAsync("access_token");
                var response = await _productServices.GetAllProductByIdAsync<ResponseDto>(productId,accesstoken);
                if (response != null && response.IsSuccess)
                {
                    ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                    return View(model);
                }
            }
            return NotFound();
        }      

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {
            if(ModelState.IsValid)
            {
                var accesstoken = await HttpContext.GetTokenAsync("access_token");
                var response = await _productServices.DeleteProductAsync<ResponseDto>(model.ProductId,accesstoken);
                if (response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);
        }

    }
}
