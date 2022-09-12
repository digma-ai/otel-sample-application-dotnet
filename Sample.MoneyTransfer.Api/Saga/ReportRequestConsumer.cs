using MassTransit;
using Sample.Contracts;
namespace Sample.Consumer;
public class ReportRequestReceivedConsumer : IConsumer<IReportRequestReceivedEvent>
{
    public async Task Consume(ConsumeContext<IReportRequestReceivedEvent> context)
    {
        var reportId = context.Message.ReportId;
        await Console.Out.WriteLineAsync(
            $"Report request is received, report id is; {reportId}. Correlation Id: {context.Message.CorrelationId}");
        //Get report from Db, file, etc...
        if (reportId.StartsWith("report-", StringComparison.Ordinal))
        {
            await context.Publish<IReportCreatedEvent>(new
            {
                context.Message.CorrelationId,
                context.Message.CustomerId,
                context.Message.ReportId,
                BlobUri = "https://google.com",
                CreationTime = DateTime.Now
            });
        }
        else
        {
            await context.Publish<IReportFailedEvent>(new
            {
                context.Message.CorrelationId,
                context.Message.CustomerId,
                context.Message.ReportId,
                FaultMessage = "Report name is invalid! Please retry again!",
                FaultTime = DateTime.Now
            });
        }
    }
}