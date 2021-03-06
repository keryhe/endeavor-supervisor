﻿using Endeavor.Supervisor.Messaging;
using Endeavor.Supervisor.Persistence;
using Endeavor.Supervisor.Polling;
using Keryhe.Messaging.RabbitMQ.Extensions;
using Keryhe.Persistence.SqlServer.Extensions;
using Keryhe.Polling;
using Keryhe.Polling.Delay;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Endeavor.Supervisor.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    services.AddTransient<IDelay, FibonacciDelay>();
                    services.Configure<FibonacciOptions>(hostContext.Configuration.GetSection("FibonacciOptions"));

                    services.AddSqlServerProvider(hostContext.Configuration.GetSection("SqlServerProvider"));
                    services.AddRabbitMQPublisher<TaskToBeWorked>(hostContext.Configuration.GetSection("RabbitMQPublisher"));

                    services.AddTransient<IDal, SupervisorDal>();
                    services.AddTransient<IPoller<TaskToBeWorked>, ReadyTaskPoller>();

                    services.AddSingleton<IHostedService, Supervisor>();

                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync();
        }
    }
}
