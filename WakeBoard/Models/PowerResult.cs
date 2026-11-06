namespace WakeBoard.Models;

/// <summary>
/// The outcome of a power action.
/// </summary>
public class PowerResult
{
    /// <summary>
    /// Whether the action succeeded.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// A message describing the outcome.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}