namespace WakeBoard.Models;

/// <summary>
/// The Semaphore settings used for power off and sleep.
/// </summary>
public class SemaphoreOptions
{
    /// <summary>
    /// The base URL of the Semaphore instance.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// The Semaphore API token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the Semaphore project.
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// The ID of the power action task template.
    /// </summary>
    public int TemplateId { get; set; }
}