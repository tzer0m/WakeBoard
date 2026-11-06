using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using WakeBoard.Models;
using WakeBoard.Services;

namespace WakeBoard.Pages;

/// <summary>
/// The machine status page.
/// </summary>
/// <param name="options">The WakeBoard options.</param>
/// <param name="statusService">The status service.</param>
/// <param name="wakeService">The wake on LAN service.</param>
/// <param name="semaphoreService">The Semaphore service.</param>
[Authorize]
public class IndexModel(IOptions<MachineOptions> options, MachineStatusService statusService, WakeOnLanService wakeService, SemaphoreService semaphoreService) : PageModel
{
    /// <summary>
    /// The configured machines.
    /// </summary>
    public List<MachineConfig> Machines { get; set; } = [];

    /// <summary>
    /// How often the page refreshes machine status in milliseconds.
    /// </summary>
    public int RefreshIntervalMs { get; set; }

    /// <summary>
    /// A message shown after an action.
    /// </summary>
    [TempData]
    public string? Message { get; set; }

    /// <summary>
    /// Loads the machine list.
    /// </summary>
    public void OnGet()
    {
        Machines = options.Value.Machines;
        RefreshIntervalMs = options.Value.RefreshIntervalMs;
    }

    /// <summary>
    /// Returns the current status of every machine as JSON.
    /// </summary>
    public async Task<IActionResult> OnGetStatusAsync()
    {
        List<MachineStatus> statuses = await statusService.CheckAllAsync();
        return new JsonResult(statuses);
    }

    /// <summary>
    /// Wakes a machine.
    /// </summary>
    /// <param name="name">The machine name.</param>
    public async Task<IActionResult> OnPostWakeAsync(string name)
    {
        MachineConfig? machine = options.Value.Machines.FirstOrDefault(item => item.Name == name);
        if (machine is null) { return NotFound(); }
        PowerResult result = await wakeService.WakeAsync(machine);
        return Respond(result.Message);
    }

    /// <summary>
    /// Powers a machine off.
    /// </summary>
    /// <param name="name">The machine name.</param>
    public Task<IActionResult> OnPostPowerOffAsync(string name)
    {
        return RunPowerActionAsync(name, "shutdown");
    }

    /// <summary>
    /// Puts a machine to sleep.
    /// </summary>
    /// <param name="name">The machine name.</param>
    public Task<IActionResult> OnPostSleepAsync(string name)
    {
        return RunPowerActionAsync(name, "sleep");
    }

    private async Task<IActionResult> RunPowerActionAsync(string name, string action)
    {
        MachineConfig? machine = options.Value.Machines.FirstOrDefault(item => item.Name == name);
        if (machine is null) { return NotFound(); }
        if (action == "sleep" && !machine.CanSleep) { return Respond($"{machine.Name} does not support sleep."); }
        PowerResult result = await semaphoreService.RunAsync(machine, action);
        return Respond(result.Message);
    }

    private IActionResult Respond(string message)
    {
        if (Request.Headers.XRequestedWith == "fetch") { return new JsonResult(new { message }); }
        Message = message;
        return RedirectToPage();
    }
}