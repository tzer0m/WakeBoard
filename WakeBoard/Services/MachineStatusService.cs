using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.Extensions.Options;
using WakeBoard.Models;

namespace WakeBoard.Services;

/// <summary>
/// Checks whether machines are online.
/// </summary>
/// <param name="options">The WakeBoard options.</param>
public class MachineStatusService(IOptions<MachineOptions> options)
{
    /// <summary>
    /// Checks a single machine.
    /// </summary>
    /// <param name="machine">The machine to check.</param>
    public async Task<MachineStatus> CheckAsync(MachineConfig machine)
    {
        bool online = machine.Port.HasValue ? await CheckTcpAsync(machine.Host, machine.Port.Value) : await CheckPingAsync(machine.Host);
        return new MachineStatus { Name = machine.Name, Online = online };
    }

    /// <summary>
    /// Checks all configured machines in parallel.
    /// </summary>
    public async Task<List<MachineStatus>> CheckAllAsync()
    {
        MachineStatus[] results = await Task.WhenAll(options.Value.Machines.Select(CheckAsync));
        return [.. results];
    }

    /// <summary>
    /// Checks whether a TCP port accepts connections.
    /// </summary>
    /// <param name="host">The hostname or IP address.</param>
    /// <param name="port">The TCP port to connect to.</param>
    private async Task<bool> CheckTcpAsync(string host, int port)
    {
        try
        {
            using TcpClient client = new();
            using CancellationTokenSource cancellation = new(options.Value.TimeoutMs);
            await client.ConnectAsync(host, port, cancellation.Token);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Checks whether a host responds to a ping.
    /// </summary>
    /// <param name="host">The hostname or IP address.</param>
    private async Task<bool> CheckPingAsync(string host)
    {
        try
        {
            using Ping ping = new();
            PingReply reply = await ping.SendPingAsync(host, options.Value.TimeoutMs);
            return reply.Status == IPStatus.Success;
        }
        catch (Exception)
        {
            return false;
        }
    }
}