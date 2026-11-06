namespace WakeBoard.Models;

/// <summary>
/// A machine that can be monitored.
/// </summary>
public class MachineConfig
{
    /// <summary>
    /// The display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The hostname or IP address used for status checks.
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// An optional TCP port to check instead of pinging.
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// The MAC address used for wake on LAN.
    /// </summary>
    public string? Mac { get; set; }

    /// <summary>
    /// The network interface used to send the wake packet.
    /// </summary>
    public string? Interface { get; set; }
}