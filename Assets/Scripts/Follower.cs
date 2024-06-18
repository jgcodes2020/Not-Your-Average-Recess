using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform source;
    
    private void Update()
    {
        transform.position = source.position;
    }
}