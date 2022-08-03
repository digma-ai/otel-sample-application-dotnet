using MassTransit;

namespace Sample.MoneyTransfer.API.Utils;

public interface IMessagePublisher
{
    Task Publish<T>(T message, CancellationToken cancellationToken = default)
        where T : class;
}
public class MessagePublisher:IMessagePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MessagePublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }
    public async Task Publish<T>(T message, CancellationToken cancellationToken = default)
        where T : class
    {
        await _publishEndpoint.Publish(message, cancellationToken);
    }
}

public class DoNothingMessagePublisher : IMessagePublisher
{
    public async Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        await Task.CompletedTask;
    }
}