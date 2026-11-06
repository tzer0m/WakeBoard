namespace WakeBoard.Models;

/// <summary>
/// The WakeBoard settings.
/// </summary>
public class MachineOptions
{
    /// <summary>
    /// The status check timeout in milliseconds.
    /// </summary>
    public int TimeoutMs { get; set; }

    /// <summary>
    /// How often the page refreshes machine status in milliseconds.
    /// </summary>
    public int RefreshIntervalMs { get; set; }

    /// <summary>
    /// The timeout for the wake command in milliseconds.
    /// </summary>
    public int CommandTimeoutMs { get; set; }

    /// <summary>
    /// The path to the sudo executable.
    /// </summary>
    public string SudoPath { get; set; } = string.Empty;

    /// <summary>
    /// The path to the etherwake executable.
    /// </summary>
    public string EtherWakePath { get; set; } = string.Empty;

    /// <summary>
    /// The configured machines.
    /// </summary>
    public List<MachineConfig> Machines { get; set; } = [];
}