using UnityEngine;

/// <summary>
/// Utility functions for vectors that Unity doesn't have.
/// </summary>
public static class VectorUtils
{
    /// <summary>
    /// Zeroes the Y-component of a 3D vector, leaving only the horizontal part.
    /// </summary>
    /// <param name="v">The vector</param>
    /// <returns>The horizontal part of the vector</returns>
    public static Vector3 Horizontal(this Vector3 v) => new(v.x, 0, v.z);
    /// <summary>
    /// Takes the horizontal part of a 3D vector and returns it as a 2D vector.
    /// </summary>
    /// <param name="v">The vector</param>
    /// <returns>The horizontal part of the vector</returns>
    public static Vector2 Horizontal2D(this Vector3 v) => new(v.x, v.z);
    
    /// <summary>
    /// Converts a 2D vector into a 3D vector in the XZ plane.
    /// </summary>
    /// <param name="v">The 2D vector</param>
    /// <returns>A 3D vector in the XZ plane</returns>
    public static Vector3 ToHorizontal(this Vector2 v) => new(v.x, 0, v.y);
}