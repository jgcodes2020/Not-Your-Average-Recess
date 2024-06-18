using UnityEngine;

public partial class PlayerController
{
    public AudioClip tagSfx;

    private void PlayTagSound()
    {
        AudioSource.PlayClipAtPoint(tagSfx, Vector3.zero, 1.0f);
    }
}