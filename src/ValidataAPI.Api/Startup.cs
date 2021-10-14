using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using ValidataAPI.Api.Middleware;
using ValidataAPI.Utils.Common;
using ValidataAPI.Utils.Services;
using ValidataAPI.Utils.Repositories;

namespace ValidataAPI.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var healthChecks = services.AddHealthChecks();
            ConfigureSqlServer(services, healthChecks);
            ConfigureLogging(services);
            services.AddHttpContextAccessor();
            services.AddTransient<IHttpContextService, HttpContextService>();
            services.AddTransient<ITraceIdResolverService, TraceIdResolverService>();
            services.AddTransient<IJsonSerializerService, JsonSerializerService>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ValidataAPI.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<HeaderToContextMiddleware>();
            app.UseMiddleware<ContextToLoggerMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ValidataAPI.Api v1"));
            }

            app.UseRouting();
            app.UseHealthChecks("/health-check", new HealthCheckOptions
            {
                ResponseWriter = async (c, r) =>
                {
                    var result = JsonConvert.SerializeObject(new
                    {
                        status = r.Status.ToString(),
                        components = r.Entries.Select(e => new {key = e.Key, value = e.Value.Status.ToString()})
                    });
                    c.Response.StatusCode = r.Status == HealthStatus.Healthy
                        ? (int) HttpStatusCode.OK
                        : (int) HttpStatusCode.InternalServerError;
                    c.Response.ContentType = Constants.ContentTypeApplicationJson;
                    await c.Response.WriteAsync(result);
                }
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        
        private void ConfigureSqlServer(IServiceCollection services, IHealthChecksBuilder healthChecks)
        {
            if (string.IsNullOrEmpty(Configuration.GetConnectionString(Constants.DatabaseConnectionStringKey))) return;
            services.AddSingleton<IDatabaseConnectionFactory>(e => new SqlConnectionFactory(Configuration));
            healthChecks.AddSqlServer(
                Configuration.GetConnectionString(Constants.DatabaseConnectionStringKey),
                "SELECT 1;",
                "ValidataAPI.API",
                HealthStatus.Degraded,
                timeout: TimeSpan.FromSeconds(30),
                tags: new[] {"db", "sql", "sqlServer",});
        }
        
        private void ConfigureLogging(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog(Configuration);
            });
        }
    }
}
