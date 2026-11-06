using System.Security.Cryptography;
using System.Text;
using WakeBoard.Models;
using WakeBoard.Services;

namespace WakeBoard.Endpoints;

/// <summary>
/// The API key protected endpoint that feeds the Tidgets home screen widget.
/// </summary>
public static class WidgetEndpoint
{
    /// <summary>
    /// Returns the online and offline machine counts as JSON, or 401 if the API key is missing or wrong.
    /// </summary>
    /// <param name="request">The HTTP request, for the API key header.</param>
    /// <param name="statusService">The status service.</param>
    /// <param name="configuration">Used to read the expected API key.</param>
    public static async Task<IResult> GetAsync(HttpRequest request, MachineStatusService statusService, IConfiguration configuration)
    {
        string? expected = configuration["Widget:ApiKey"];
        string provided = request.Headers["X-Api-Key"].ToString();
        if (string.IsNullOrEmpty(expected) || !CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(provided), Encoding.UTF8.GetBytes(expected)))
        {
            return Results.Unauthorized();
        }
        List<MachineStatus> statuses = await statusService.CheckAllAsync();
        int online = statuses.Count(status => status.Online);
        return Results.Ok(new WidgetResponse { Online = online, Offline = statuses.Count - online });
    }
}