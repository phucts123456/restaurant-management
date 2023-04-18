using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
	public class AzureServiceBusMessageBus : IMessageBus
	{
		private readonly IConfiguration Configuration;
		public AzureServiceBusMessageBus(IConfiguration configuration)
		{
			Configuration = configuration;
		}
		public async Task PublishMessage(BaseMessage message, string topicName)
		{
			try
			{
				await using var client = new ServiceBusClient(Configuration.GetConnectionString("azureServiceBus"));
				ServiceBusSender sender = client.CreateSender(topicName);
				var JsonMessage = JsonConvert.SerializeObject(message);
				ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonMessage))
				{
					CorrelationId = Guid.NewGuid().ToString(),
				};
				await sender.SendMessageAsync(finalMessage);
				await client.DisposeAsync();
			}
			catch (Exception ex)
			{

				Console.WriteLine(ex.ToString());
			}
			
		}
	}
}
