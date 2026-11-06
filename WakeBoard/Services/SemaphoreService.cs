using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WakeBoard.Models;

namespace WakeBoard.Services;

/// <summary>
/// Powers machines off or puts them to sleep by running a Semaphore task.
/// </summary>
/// <param name="client">The HTTP client for the Semaphore API.</param>
/// <param name="options">The Semaphore options.</param>
public class SemaphoreService(HttpClient client, IOptions<SemaphoreOptions> options)
{
    /// <summary>
    /// Starts the power action task for the given machine.
    /// </summary>
    /// <param name="machine">The machine to act on.</param>
    /// <param name="action">The action to run, either shutdown or sleep.</param>
    public async Task<PowerResult> RunAsync(MachineConfig machine, string action)
    {
        if (string.IsNullOrWhiteSpace(machine.SemaphoreHost)) { return new PowerResult { Success = false, Message = $"{machine.Name} has no Semaphore host configured." }; }
        SemaphoreOptions semaphoreOptions = options.Value;
        if (client.BaseAddress is null || semaphoreOptions.ProjectId == 0 || semaphoreOptions.TemplateId == 0) { return new PowerResult { Success = false, Message = "Semaphore is not configured." }; }
        Dictionary<string, object> body = new() { ["template_id"] = semaphoreOptions.TemplateId, ["limit"] = machine.SemaphoreHost, ["environment"] = JsonSerializer.Serialize(new { power_action = action }) };
        if (machine.SemaphoreInventoryId is not null) { body["inventory_id"] = machine.SemaphoreInventoryId.Value; }
        try
        {
            using HttpResponseMessage response = await client.PostAsJsonAsync($"api/project/{semaphoreOptions.ProjectId}/tasks", body);
            if (response.IsSuccessStatusCode) { return new PowerResult { Success = true, Message = $"{(action == "shutdown" ? "Power off" : "Sleep")} started for {machine.Name}." }; }
            return new PowerResult { Success = false, Message = $"Semaphore returned {(int)response.StatusCode} for {machine.Name}." };
        }
        catch (Exception exception)
        {
            return new PowerResult { Success = false, Message = $"Could not reach Semaphore for {machine.Name}: {exception.Message}" };
        }
    }
}