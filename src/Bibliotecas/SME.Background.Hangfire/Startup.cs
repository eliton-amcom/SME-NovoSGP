﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SME.Background.Hangfire
{
    public class Startup
    {
        private IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.UseHangfireDashboard("/worker", new DashboardOptions()
            //{
            //    IsReadOnlyFunc = (DashboardContext context) => !env.IsDevelopment(),
            //    Authorization = new[] { new DashboardAuthorizationFilter() }
            //});
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddHangfire(configuration => configuration
            //    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //    .UseSimpleAssemblyNameTypeSerializer()
            //    .UseRecommendedSerializerSettings()
            //    .UseFilter<AutomaticRetryAttribute>(new AutomaticRetryAttribute() { Attempts = 0 })
            //    .UsePostgreSqlStorage(this.configuration.GetConnectionString("SGP-Postgres"), new PostgreSqlStorageOptions()
            //    {
            //        QueuePollInterval = TimeSpan.FromSeconds(10),
            //        SchemaName = "hangfire"
            //    }));
        }
    }
}