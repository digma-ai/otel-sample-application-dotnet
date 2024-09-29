using Microsoft.AspNetCore.Mvc;
using Sample.NotificationService.API.Contracts;
using Sample.NotificationService.API.Services;

namespace Sample.NotificationService.API.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> SendTransferSuccess([FromBody] TransferNotificationRequest request)
    {
        try
        {
            await _notificationService.SendTransferSuccessNotificationAsync(request.AccountId, request.Amount, request.TransferDate);
            return Ok("Success notification sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending transfer success notification");
            return StatusCode(500, "Failed to send success notification.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendTransferFailure([FromBody] FailureNotificationRequest request)
    {
        try
        {
            await _notificationService.SendTransferFailureNotificationAsync(request.AccountId, request.Reason);
            return Ok("Failure notification sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending transfer failure notification");
            return StatusCode(500, "Failed to send failure notification.");
        }
    }
}