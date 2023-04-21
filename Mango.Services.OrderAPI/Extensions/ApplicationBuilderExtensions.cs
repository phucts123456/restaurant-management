using Mango.Services.OrderAPI.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mango.Services.OrderAPI.Extensions
{
	public static class ApplicationBuilderExtensions
	{ 
		public static IAzureServiceBusConsumer serviceBusConsumer { get; set; }
		public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
		{
			serviceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
			var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();
			hostApplicationLife.ApplicationStarted.Register(OnStart);
			hostApplicationLife.ApplicationStopped.Register(OnStop);
			return app;
		}

		private static void OnStop()
		{
			 serviceBusConsumer.Stop();
		}

		private static  void OnStart()
		{
			 serviceBusConsumer.Start();
		}
	}
}
