using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Instrumentation.Digma;

using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.Digma.Diagnostic;


namespace Sample.MoneyTransfer.Gateway;

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

            Console.WriteLine($"DEPLOYMENT_COMMIT_ID={Environment.GetEnvironmentVariable("DEPLOYMENT_COMMIT_ID")}");
            Console.WriteLine($"DEPLOYMENT_ENVIORNMENT={Environment.GetEnvironmentVariable("DEPLOYMENT_ENV")}");

            //Configure opentelemetry
            builder.Services.AddOpenTelemetryTracing(builder => builder
                .AddAspNetCoreInstrumentation(options =>
                    {options.RecordException = true;})
                .AddHttpClientInstrumentation()
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddTelemetrySdk()
                        .AddService(serviceName: serviceName, serviceVersion: serviceVersion ?? "0.0.0")
                        .AddDigmaAttributes(configure =>
                        {
                            configure.Environment = "Prod";
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



