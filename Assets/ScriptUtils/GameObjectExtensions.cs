using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// Lists the children of a GameObject.
    /// </summary>
    /// <param name="gameObject">the GameObject to check</param>
    /// <returns>an iterator over the children of the GameObject</returns>
    public static IEnumerable<GameObject> GetChildren(this GameObject gameObject)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            yield return gameObject.transform.GetChild(i).gameObject;
        }
    }

    /// <summary>
    /// Finds a child GameObject by name.
    /// </summary>
    /// <param name="gameObject">the GameObject to check</param>
    /// <param name="name">the name of the child to locate</param>
    /// <returns>The child, or null if none exist with the desired name</returns>
    public static GameObject FindChild(this GameObject gameObject, string name)
    {
        return gameObject.transform.Find(name).gameObject;
    }

    /// <summary>
    /// Lists the children of a transform.
    /// </summary>
    /// <param name="transform">The Transform to check</param>
    /// <returns>an iterator over the children of the Transform</returns>
    public static IEnumerable<Transform> GetChildren(this Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            yield return transform.GetChild(i);
        }
    }
}