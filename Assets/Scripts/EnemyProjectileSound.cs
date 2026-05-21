using UnityEngine;

public class EnemyProjectileSound : MonoBehaviour
{
    [Header("Collision Sound")]
    public AudioClip contactSound;

    private bool hasPlayedSound = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayContactSound();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayContactSound();
    }

    private void PlayContactSound()
    {
        // Prevent the sound from playing multiple times
        if (hasPlayedSound)
        {
            return;
        }

        hasPlayedSound = true;

        // Play collision sound
        if (AudioManager.instance != null && contactSound != null)
        {
            AudioManager.instance.PlaySound(contactSound);
        }
    }
}