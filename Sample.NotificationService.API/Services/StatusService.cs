namespace Sample.NotificationService.API.Services;

public interface IStatusService
{
    public Task Check();
}
public class StatusService: IStatusService
{
    public async Task Check()
    {
        throw new NotImplementedException();
    }
}