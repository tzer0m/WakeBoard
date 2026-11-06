using System.Diagnostics;
using Microsoft.Extensions.Options;
using WakeBoard.Models;

namespace WakeBoard.Services;

/// <summary>
/// Wakes machines using etherwake.
/// </summary>
/// <param name="options">The WakeBoard options.</param>
public class WakeOnLanService(IOptions<MachineOptions> options)
{
    /// <summary>
    /// Sends a wake packet to the given machine.
    /// </summary>
    /// <param name="machine">The machine to wake.</param>
    public async Task<PowerResult> WakeAsync(MachineConfig machine)
    {
        if (string.IsNullOrWhiteSpace(machine.Mac) || string.IsNullOrWhiteSpace(machine.Interface)) { return new PowerResult { Success = false, Message = $"{machine.Name} has no MAC address or interface configured." }; }
        MachineOptions machineOptions = options.Value;
        ProcessStartInfo startInfo = new(machineOptions.SudoPath) { RedirectStandardError = true };
        startInfo.ArgumentList.Add("-n");
        startInfo.ArgumentList.Add(machineOptions.EtherWakePath);
        startInfo.ArgumentList.Add("-i");
        startInfo.ArgumentList.Add(machine.Interface);
        startInfo.ArgumentList.Add(machine.Mac);
        try
        {
            using Process? process = Process.Start(startInfo);
            if (process is null) { return new PowerResult { Success = false, Message = $"Could not start the wake command for {machine.Name}." }; }
            using CancellationTokenSource cancellation = new(machineOptions.CommandTimeoutMs);
            string error = await process.StandardError.ReadToEndAsync(cancellation.Token);
            await process.WaitForExitAsync(cancellation.Token);
            if (process.ExitCode == 0) { return new PowerResult { Success = true, Message = $"Wake packet sent to {machine.Name}." }; }
            return new PowerResult { Success = false, Message = $"Wake failed for {machine.Name}: {error.Trim()}" };
        }
        catch (Exception exception)
        {
            return new PowerResult { Success = false, Message = $"Wake failed for {machine.Name}: {exception.Message}" };
        }
    }
}