namespace WakeBoard.Models;

/// <summary>
/// The online state of a machine.
/// </summary>
public class MachineStatus
{
    /// <summary>
    /// The machine name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Whether the machine responded.
    /// </summary>
    public bool Online { get; set; }
}