﻿using DeliveryService.Application.Core;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using DeliveryService.Data.SQL.Repositories;
using Microsoft.EntityFrameworkCore;
using DeliveryService.Data.SQL.Context;
using DeliveryService.Application.Domain.Interfaces;

namespace DeliveryService.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            AddApplicationServices(services);

            services.AddMvc();

			//services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info
            //    {
            //        Version = "v1",
            //        Title = "API de entrga de melhores rotas",
            //        Description = "API para gerenciamento e manutencao de rotas do sistema",
            //        TermsOfService = "None",
            //        Contact = new Contact() { Name = "Giancarlos A. Macedo", Email = "gianaugusto@gmail.com", Url = "www.gianaugusto.com.br" }
            //    });
            //});

			services.AddScoped<DbContext, ServiceContext>();

            var connString = Configuration.GetConnectionString("DefaultConnection");
			services.AddDbContext<ServiceContext>(o => o.UseSqlServer(connString));
			services.AddScoped<IServiceRepository, ServiceRepository>();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        
		
			//app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
			//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "XPTO : Delivery Services - V1");
            //});


			using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
				var context = serviceScope.ServiceProvider.GetRequiredService<ServiceContext>();

                if (context.Database.EnsureCreated())
                {
					var seed = new DeliveryService.Data.SQL.Tools.Seed(context);

                    // preenche os dados iniciais
					seed.SeedServiceRoutes();
                    
                }
            }
		}

        private static void AddApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IServiceRepository, ServiceRepository>();
            AddMediatr(services);
        }

        private static void AddMediatr(IServiceCollection services)
        {
            const string applicationAssemblyName = "DeliveryService.Application";
            var assembly = AppDomain.CurrentDomain.Load(applicationAssemblyName);

            AssemblyScanner
                .FindValidatorsInAssembly(assembly)
                .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FailFastRequestBehavior<,>));

            services.AddMediatR();
        }
    }
}