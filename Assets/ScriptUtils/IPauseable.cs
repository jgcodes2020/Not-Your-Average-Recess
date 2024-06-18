
/// <summary>
/// Interface representing an object that can be paused.
/// </summary>
public interface IPauseable
{
    /// <summary>
    /// Property controlling whether the object is paused or not.
    /// </summary>
    bool Paused { get; set; }
}