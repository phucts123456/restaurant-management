﻿using Mango.Services.ShoppingCartAPI.Messages;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        protected ResponseDto _response;

        public CartAPIController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
            this._response = new ResponseDto();
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(userId);
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {

                _response.IsSuccess =false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost("AddCart")]
        public async Task<object> AddCart(CartDto cartDto)
        {
            try
            {
                CartDto cartDtoResult = await _cartRepository.CreateUpdateCart(cartDto);
                _response.Result = cartDtoResult;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart(CartDto cartDto)
        {
            try
            {
                CartDto cartDtoResult = await _cartRepository.CreateUpdateCart(cartDto);
                _response.Result = cartDtoResult;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        } 
        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart([FromBody]int cartId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveFromCart(cartId);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        } 
        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckoutHeaderDto checkoutHeader)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(checkoutHeader.UserId);
                if(cartDto == null)
                {
                    return BadRequest();
                }
				checkoutHeader.cartDetails = cartDto.CartDetails;
                //logic to add message to process order.
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody]CartDto cartDto)
        {
            try
            {
                bool isSuccess = await _cartRepository.ApplyCode(cartDto.CartHeader.UserId, 
                    cartDto.CartHeader.CouponCode);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody]string userId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveCode(userId);
                _response.Result = isSuccess;
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
