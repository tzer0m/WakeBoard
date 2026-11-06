namespace WakeBoard.Models;

/// <summary>
/// The machine counts shown by the Tidgets home screen widget.
/// </summary>
public class WidgetResponse
{
    /// <summary>
    /// The number of machines that are online.
    /// </summary>
    public int Online { get; set; }

    /// <summary>
    /// The number of machines that are offline.
    /// </summary>
    public int Offline { get; set; }
}