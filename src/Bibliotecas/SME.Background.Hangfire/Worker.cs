﻿using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.Background.Core.Interfaces;
using SME.Background.Hangfire.Logging;
using System;
using System.IO;

namespace SME.Background.Hangfire
{
    public class Worker : IWorker
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;
        private readonly IServiceCollection serviceCollection;
        private BackgroundJobServer hangFireServer;
        private IWebHost host;

        public Worker(IConfiguration configuration, IServiceCollection serviceCollection, string connectionString)
        {
            this.configuration = configuration;
            this.serviceCollection = serviceCollection;
            this.connectionString = connectionString;
        }

        public void Dispose()
        {
            host?.Dispose();
        }

        public void Registrar()
        {
            //RegistrarHangfireServer();
            //RegistrarDashboard();
        }

        private void RegistrarDashboard()
        {
            host = new WebHostBuilder()
                           .UseKestrel()
                           .UseContentRoot(Directory.GetCurrentDirectory())
                           .ConfigureAppConfiguration((hostContext, config) =>
                           {
                               config.SetBasePath(Directory.GetCurrentDirectory());
                               config.AddJsonFile("appsettings.json", optional: false);
                               config.AddEnvironmentVariables();
                           })
                           .UseStartup<Startup>()
                           .UseUrls(new[] { "http://*:5000" })
                           .Build();

            host.RunAsync();
        }

        private void RegistrarHangfireServer()
        {
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseLogProvider<SentryLogProvider>(new SentryLogProvider())
                .UseRecommendedSerializerSettings()
                .UseActivator<HangfireActivator>(new HangfireActivator(serviceCollection.BuildServiceProvider()))
                .UseFilter<AutomaticRetryAttribute>(new AutomaticRetryAttribute() { Attempts = 0 })
                .UsePostgreSqlStorage(configuration.GetConnectionString(connectionString), new PostgreSqlStorageOptions()
                {
                    QueuePollInterval = TimeSpan.FromSeconds(1),
                    SchemaName = "hangfire"
                });

            GlobalJobFilters.Filters.Add(new SGP.Hangfire.ContextFilterAttribute());

            hangFireServer = new BackgroundJobServer();
        }
    }
}

//