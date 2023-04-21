using Mango.Services.OrderAPI.DbContexts;
using Mango.Services.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Mango.Services.OrderAPI.Repository
{
	public class OrderRepository : IOrderRepository
	{
		private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
		public OrderRepository(DbContextOptions<ApplicationDbContext> dbContextOptions)
		{
			_dbContextOptions = dbContextOptions;
		}

		public async Task<bool> AddOrder(OrderHeader orderHeader)
		{
			await using var _dbContext = new ApplicationDbContext(_dbContextOptions);
			_dbContext.OrderHeaders.Add(orderHeader);
			var result = await _dbContext.SaveChangesAsync();
			System.Console.WriteLine(result);
			return true;
		}

		public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
		{
			await using var _dbContext = new ApplicationDbContext(_dbContextOptions);
			var orderHeaderFromDb = await _dbContext.OrderHeaders.FirstOrDefaultAsync(u => u.OrderHeaderId == orderHeaderId);
			if (orderHeaderFromDb != null)
			{
				orderHeaderFromDb.PaymentStatus = paid;
				await _dbContext.SaveChangesAsync();
			}
		}
	}
}
