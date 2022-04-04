using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using OpenTelemetry.Instrumentation.Digma;
using Sample.MoneyTransfer.API.Utils;
using Sample.MoneyTransfer.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Sample.MoneyTransfer.API;

public class RunWebApp
{

		public static void Run(string[] args)
        {
            //Standard MVC boilerplate
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var digma_url = builder.Configuration.GetSection("Digma").GetValue<string>("URL");

            var serviceName = typeof(RunWebApp).Assembly.GetName().Name;
            var serviceVersion = typeof(RunWebApp).Assembly.GetName().Version!.ToString();

            //Optional for dev context only
            string commitHash = SCMUtils.GetLocalCommitHash(builder);

            //Configure opentelemetry
            builder.Services.AddOpenTelemetryTracing((builder) => builder
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;

                })
                .AddEntityFrameworkCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: serviceName, serviceVersion: serviceVersion ?? "0.0.0")
                    .AddDigmaAttributes(configure => { configure.CommitId = commitHash; }))
                .AddOtlpExporter(c =>
                {
                    c.Endpoint = new Uri(digma_url);
                })
                .AddSource(serviceName)
            );  

            builder.Services
                .AddDbContext<MoneyKeepingContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "Vault"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();


        }

    
}

