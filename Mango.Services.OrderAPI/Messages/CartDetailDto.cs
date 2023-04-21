using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.OrderAPI.Messages
{
    public class CartDetailDto
    {
        public int CartDetailId { get; set; }
        public int CartHeaderId { get; set; }
        public int ProductId { get; set; }
        public virtual ProductDto Product { get; set; }
        public int Count { get; set; }
    }
}
