﻿using Dapper;
using HealthChecks.UI.Client;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Prometheus;
using SME.Background.Core;
using SME.Background.Hangfire;
using SME.SGP.Api.Filtros;
using SME.SGP.Api.HealthCheck;
using SME.SGP.Background;
using SME.SGP.Dados;
using SME.SGP.Dados.Contexto;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using SME.SGP.IoC;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace SME.SGP.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;

        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment _env;

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseRequestLocalization();

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SGP Api");
            });

            //TODO: Ajustar para as os origins que irão consumir
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseMetricServer();

            app.UseHttpMetrics();

            app.UseAuthentication();

            app.UseMvc();

            app.UseStaticFiles();

            Console.WriteLine("CURRENT------",Directory.GetCurrentDirectory());
            Console.WriteLine("COMBINE------", Path.Combine(Directory.GetCurrentDirectory(), @"Imagens"));
            

            //TODO: <Configuração para upload com Jodit, se necessário pode ser removido após aprovação da história de demonstração>
            if (_env.EnvironmentName != "teste-integrado")
                app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                         Path.Combine(Directory.GetCurrentDirectory(), @"Imagens")),
                RequestPath = new PathString("/imagens"),
                ServeUnknownFileTypes = true
            });
            //TODO: </Configuração para upload com Jodit, se necessário pode ser removido após aprovação da história de demonstração>

            app.UseHealthChecks("/healthz", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });  
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.AddSingleton(Configuration);
            services.AddHttpContextAccessor();

            RegistraDependencias.Registrar(services);
            RegistraClientesHttp.Registrar(services, Configuration);
            RegistraAutenticacao.Registrar(services, Configuration);
            RegistrarMvc.Registrar(services, Configuration);
            RegistraDocumentacaoSwagger.Registrar(services);

            DefaultTypeMap.MatchNamesWithUnderscores = true;

            services.AddApplicationInsightsTelemetry(Configuration);

            var serviceProvider = services.BuildServiceProvider();

            Orquestrador.Inicializar(serviceProvider);

            

            //services.AdicionarRedis(Configuration, serviceProvider.GetService<IServicoLog>());

            if (Configuration.GetValue<bool>("FF_BackgroundEnabled", false))
            {
                Orquestrador.Registrar(new Processor(Configuration, "SGP_Postgres"));
                RegistraServicosRecorrentes.Registrar();
            }
            else
                Orquestrador.Desativar();

            services.AddHealthChecks()
                   //.AddRedis(
                   //     Configuration.GetConnectionString("SGP_Redis"),
                   //     "Redis Cache",
                   //     null,
                   //     tags: new string[] { "db", "redis" })
                    .AddNpgSql(
                        Configuration.GetConnectionString("SGP_Postgres"),
                        name: "Postgres")
                    .AddCheck<ApiJuremaCheck>("API Jurema")
                    .AddCheck<ApiEolCheck>("API EOL");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-BR");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("pt-BR"), new CultureInfo("pt-BR") };
            });

            if (_env.EnvironmentName != "teste-integrado")
            {
                services.AddRabbit();
            }

            // Teste para injeção do client de telemetria em classe estática 


            var clientTelemetry = serviceProvider.GetService<TelemetryClient>();
            DapperExtensionMethods.Init(clientTelemetry);

            //

            services.AddMemoryCache();            
        }
    }
}