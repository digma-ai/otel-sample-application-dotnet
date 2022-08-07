using Digma.MassTransit.Integration;
using MassTransit;
using MassTransit.Definition;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Instrumentation.Digma;
using Sample.MoneyTransfer.API.Utils;
using Sample.MoneyTransfer.API.Data;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.Digma.Diagnostic;
using Sample.MoneyTransfer.API.Consumer;
using Sample.MoneyTransfer.API.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

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
            builder.Services.AddTransient<TransferFundsEventConsumer>();
            
            var digmaUrl = builder.Configuration.GetSection("Digma").GetValue<string>("URL");
            Console.WriteLine($"Digma Url: {digmaUrl}");
            var serviceName = typeof(RunWebApp).Assembly.GetName().Name;
            var serviceVersion = typeof(RunWebApp).Assembly.GetName().Version!.ToString();

            Console.WriteLine($"DEPLOYMENT_COMMIT_ID={Environment.GetEnvironmentVariable("DEPLOYMENT_COMMIT_ID")}");
            Console.WriteLine($"DEPLOYMENT_ENVIORNMENT={Environment.GetEnvironmentVariable("DEPLOYMENT_ENV")}");

            var rabbitSection = builder.Configuration.GetSection("RabbitMq");
            if (rabbitSection.Exists())
            {
                builder.Services.AddMassTransitHostedService();
                builder.Services.AddTransient<IMessagePublisher, MessagePublisher>();
                builder.Services.AddMassTransit(o =>
                {
                    o.SetKebabCaseEndpointNameFormatter();
                    o.AddConsumer<TransferFundsEventConsumer>();
                    
                    o.UsingRabbitMq((context, configurator) =>
                    {  
                        var configuration = context.GetService<IConfiguration>();
                        var host = configuration.GetValue<string>("RabbitMq:Host");
                        var userName = configuration.GetValue<string>("RabbitMq:Username");
                        var password = configuration.GetValue<string>("RabbitMq:Password");
                        configurator.Host(host, c =>
                        {
                            c.Username(userName);
                            c.Password(password);
                        });
                        
                        configurator.ReceiveEndpoint(KebabCaseEndpointNameFormatter.Instance.Consumer<TransferFundsEventConsumer>(), c => {
                             c.ConfigureConsumer<TransferFundsEventConsumer>(context);
                        });
                        configurator.ConfigureEndpoints(context);
                    });
                });
            }
            else
            {
                builder.Services.AddTransient<IMessagePublisher, DoNothingMessagePublisher>();

            }
           
            
            
            //Optional for dev context only
            string ? commitHash = SCMUtils.GetLocalCommitHash(builder);

            Console.WriteLine($"GetLocalCommitHash: {commitHash}");
            builder.Services.UseDigmaHttpDiagnosticObserver();
            builder.Services.UseDigmaMassTransitConsumeObserver(o =>
            {
                o.Observe<TransferFundsEventConsumer>();
            });

            //Configure opentelemetry
            builder.Services.AddOpenTelemetryTracing(builder => builder
                .AddAspNetCoreInstrumentation(options =>{options.RecordException = true;})
                .AddHttpClientInstrumentation()
                .AddMassTransitInstrumentation()
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddTelemetrySdk()
                        .AddService(serviceName: serviceName, serviceVersion: serviceVersion ?? "0.0.0")
                        .AddDigmaAttributes(configure =>
                        {
                            if(commitHash is not null) configure.CommitId = commitHash;
                            configure.SpanMappingPattern = @"(?<ns>[\S\.]+)\/(?<class>\S+)\.(?<method>\S+)";
                            configure.SpanMappingReplacement = @"${ns}.Controllers.${class}.${method}";
                        })
                )
                .AddOtlpExporter(c =>
                {
                    
                    c.Endpoint = new Uri(digmaUrl);
                    c.Protocol = OtlpExportProtocol.Grpc;
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

