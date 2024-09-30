using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.Digma;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sample.NotificationService.API.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddTransient<IStatusService, StatusService>();
builder.Services.AddHostedService<MonitorHostedService>();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r
        .AddService(
            serviceName: "Sample.NotificatioService.API",
            serviceInstanceId: Environment.MachineName)
        .AddDigmaAttributes(configure =>
        {
            configure.NamespaceRoot = "Sample";
        })
        .AddEnvironmentVariableDetector())
    .WithTracing(b =>
    {
        b.AddSource("*")
            .SetSampler(new AlwaysOnSampler())
            .SetErrorStatusOnException()
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation(config => config.RecordException = true);


        b.AddOtlpExporter(otlpOptions =>
        {
            // Use IConfiguration directly for Otlp exporter endpoint option.
            otlpOptions.Endpoint = new Uri(builder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
            otlpOptions.Protocol = OtlpExportProtocol.Grpc;
        });
    });
   
   



// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();