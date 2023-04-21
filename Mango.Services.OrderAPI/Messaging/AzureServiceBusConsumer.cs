using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mango.Services.OrderAPI.Messaging
{
	public class AzureServiceBusConsumer : IAzureServiceBusConsumer
	{
		private readonly OrderRepository _orderRepository;
		private readonly string subcriptionCheckout;
		private readonly string checkoutMessageTopic;
		private readonly string serviceBusConnectionString;
		private readonly string processPaymentMessageTopic;
		private readonly IConfiguration _configuration;
		private ServiceBusProcessor _checkoutProcessor;
		private IMessageBus _messageBus;
		public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration, IMessageBus messageBus)
		{
			_orderRepository = orderRepository;
			serviceBusConnectionString = configuration.GetConnectionString("AzureServiceBus");
			subcriptionCheckout = configuration.GetConnectionString("SubCriptionName");
			checkoutMessageTopic = configuration.GetConnectionString("CheckoutMessageTopic");
			processPaymentMessageTopic = configuration.GetConnectionString("OrderPaymentSubcriptionName");
			var client = new ServiceBusClient(serviceBusConnectionString);
			_checkoutProcessor = client.CreateProcessor(checkoutMessageTopic, subcriptionCheckout);
			_messageBus = messageBus;
		}

		public async Task Start()
		{
			_checkoutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
			_checkoutProcessor.ProcessErrorAsync += ErrorHandler;
			await _checkoutProcessor.StartProcessingAsync();
		}
		public async Task Stop()
		{
			await _checkoutProcessor.StopProcessingAsync();
			await _checkoutProcessor.DisposeAsync();
		}
		private Task ErrorHandler(ProcessErrorEventArgs arg)
		{
			Console.WriteLine(arg.Exception.ToString());
			return Task.CompletedTask;
		}

		private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
		{
			var message = args.Message;
			var body = Encoding.UTF8.GetString(message.Body);

			CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

			OrderHeader orderHeader = new()
			{
				UserId = checkoutHeaderDto.UserId,
				FirstName = checkoutHeaderDto.FirstName,
				CardNumber = checkoutHeaderDto.CardNumber,
				CouponCode = checkoutHeaderDto.CouponCode,
				OrderDetail = new List<OrderDetail>(),
				CVV = checkoutHeaderDto.CVV,
				Email = checkoutHeaderDto.Email,
				DiscountTotal = checkoutHeaderDto.DiscountTotal,
				ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
				LastName = checkoutHeaderDto.LastName,
				Phone = checkoutHeaderDto.Phone,
				PaymentStatus = false,
				OrderTime = DateTime.Now,
				PickupDateTime = checkoutHeaderDto.PickupDateTime,
				OrderTotal = checkoutHeaderDto.OrderTotal,
				
			};
			foreach (var detail in checkoutHeaderDto.cartDetails)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = detail.ProductId,
					Count = detail.Count,
					ProductName = detail.Product.Name,
					Price = detail.Product.Price,
				};

				orderHeader.CartTotalItems += detail.Count;
				orderHeader.OrderDetail.Add(orderDetail);

			}
			await _orderRepository.AddOrder(orderHeader);
			PaymentRequestMessage paymentRequestMessage = new()
			{
				Name = orderHeader.FirstName + " " + orderHeader.LastName,
				CartNumber = orderHeader.CardNumber,
				CVV = orderHeader.CVV,
				ExpiryMonthYear = orderHeader.ExpiryMonthYear,
				OrderId = orderHeader.OrderHeaderId,
				OrderTotal = orderHeader.OrderTotal
			};
			try
			{
				Console.WriteLine("processPaymentMessageTopic: " + processPaymentMessageTopic);
				await _messageBus.PublishMessage(paymentRequestMessage, processPaymentMessageTopic);
				await args.CompleteMessageAsync(args.Message);
			}
			catch (Exception e)
			{
				throw;
			}
		}
	}
}
