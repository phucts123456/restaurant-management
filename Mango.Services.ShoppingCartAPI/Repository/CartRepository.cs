using AutoMapper;
using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public CartRepository(IMapper mapper, ApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> ApplyCode(string userId, string couponCode)
        {
            var cartFromDb = await _context.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
            cartFromDb.CouponCode = couponCode;
            _context.CartHeaders.Update(cartFromDb);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderFromDb = await _context.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
            if (cartHeaderFromDb != null) 
            {
                _context.CartDetails.
                    RemoveRange(_context.CartDetails.Where(u => u.CartDetailId == cartHeaderFromDb.CartHeaderId));
                _context.CartHeaders.Remove(cartHeaderFromDb);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {
            Cart cart = _mapper.Map<Cart>(cartDto); // mapp cartDto to Cart
            //check if product exists in database, if not create it!
            var prodInDb = await _context.Products
                .FirstOrDefaultAsync(u => u.ProductId == cart.CartDetails.FirstOrDefault()
                .ProductId);
            if (prodInDb == null)
            {
                _context.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _context.SaveChangesAsync();
            }
            //check if header is null
            var cartHeaderFromDb = await _context.CartHeaders.AsNoTracking() //no updating cartHeader from db
                .FirstOrDefaultAsync(u => u.UserId == cart.CartHeader.UserId);
            if(cartHeaderFromDb == null)
            {  
                //create header and details
                _context.CartHeaders.Add(cart.CartHeader); // add cart header to context
                await _context.SaveChangesAsync();
                Console.WriteLine(cart.CartDetails.FirstOrDefault().Product.ToString()); 
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null; // product has been added above -> no need to add it again
                _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _context.SaveChangesAsync();
            }
            else
            {
                //if header is not null 
                //check if details has same product
                var CartDetailsFromDb = await _context.CartDetails.AsNoTracking() //no updating cartDetail from db
                    .FirstOrDefaultAsync(u => u.ProductId == cart.CartDetails.FirstOrDefault().ProductId
                    && u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                if(CartDetailsFromDb == null)
                {
                    //create details
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null; // product has been added above -> no need to add it again
                    _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //update the count / cart detail
                    cart.CartDetails.FirstOrDefault().Product = null; // product has been added above -> no need to add it again
                    cart.CartDetails.FirstOrDefault().Count += CartDetailsFromDb.Count;
                    cart.CartDetails.FirstOrDefault().CartDetailId = CartDetailsFromDb.CartDetailId;
                    cart.CartDetails.FirstOrDefault().CartHeaderId = CartDetailsFromDb.CartHeaderId;
                    _context.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _context.SaveChangesAsync();
                }
            }
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _context.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId),
            };
            cart.CartDetails = _context.CartDetails.Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId)
                .Include(u => u.Product);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveCode(string userId)
        {
            var cartFromDb = await _context.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
            cartFromDb.CouponCode = "";
            _context.CartHeaders.Update(cartFromDb);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCart(int cartDetailsId)
        {
            try
            {
                CartDetail cartDetails = await _context.CartDetails
               .FirstOrDefaultAsync(u => u.CartDetailId == cartDetailsId);
                int totalCountOfCartItems = _context.CartDetails
                    .Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                //remove cartDetail
                _context.CartDetails.Remove(cartDetails);
                //remove cartHeader if there is no cartDetail left
                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders
                        .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    _context.CartHeaders.Remove(cartHeaderToRemove);
                } 
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
           
        }
    }
}
