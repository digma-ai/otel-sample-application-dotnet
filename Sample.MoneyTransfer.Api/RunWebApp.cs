using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using OpenTelemetry.Instrumentation.Digma;
using Sample.MoneyTransfer.API.Utils;
using Sample.MoneyTransfer.API.Data;
using Microsoft.EntityFrameworkCore;
using Sample.MoneyTransfer.API.Domain.Services;

namespace Sample.MoneyTransfer.API;

public class RunWebApp
{

		public static void Run(string[] args)
        {
            //Standard MVC boilerplate
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var digmaUrl = builder.Configuration.GetSection("Digma").GetValue<string>("URL");
            Console.WriteLine($"Digma Url: {digmaUrl}");
            var serviceName = typeof(RunWebApp).Assembly.GetName().Name;
            var serviceVersion = typeof(RunWebApp).Assembly.GetName().Version!.ToString();

            //Optional for dev context only
            string commitHash = SCMUtils.GetLocalCommitHash(builder);

            //Configure opentelemetry
            builder.Services.AddOpenTelemetryTracing(builder => builder
                .AddAspNetCoreInstrumentation(options =>{options.RecordException = true;})
                .AddHttpClientInstrumentation()
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddTelemetrySdk()
                        .AddService(serviceName: serviceName, serviceVersion: serviceVersion ?? "0.0.0")
                        .AddDigmaAttributes(configure =>
                        {
                            configure.CommitId = commitHash;
                            configure.SpanMappingPattern = @"(?<ns>[\S\.]+)\/(?<class>\S+)\.(?<method>\S+)";
                            configure.SpanMappingReplacement = @"${ns}.Controllers.${class}.${method}";
                        })
                )
                .AddOtlpExporter(c =>
                {
                    c.Endpoint = new Uri(digmaUrl);
                })
                .AddSource("*")
            );  

            builder.Services
                .AddDbContext<Gringotts >(options =>
                    options.UseInMemoryDatabase(databaseName: "Vault"));

        builder.Services.AddScoped<IMoneyTransferDomainService, MoneyTransferDomainService>();

        builder.Services.AddScoped<ICreditProviderService, CreditProviderService>();


        var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
           // {
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

           //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();


        }

    
}

