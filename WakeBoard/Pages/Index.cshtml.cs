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
[Authorize]
public class IndexModel(IOptions<MachineOptions> options, MachineStatusService statusService, WakeOnLanService wakeService) : PageModel
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
        Message = result.Message;
        return RedirectToPage();
    }
}