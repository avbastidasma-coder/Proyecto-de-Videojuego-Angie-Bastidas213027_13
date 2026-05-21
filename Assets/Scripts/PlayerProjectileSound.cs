using UnityEngine;

public class PlayerProjectileSound : MonoBehaviour
{
    private bool hasPlayedExplosion = false;

    private void Start()
    {
        // Play projectile shoot sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayPlayerShootSound();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayExplosion(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayExplosion(collision.gameObject);
    }

    private void PlayExplosion(GameObject targetObject)
    {
        // Prevent multiple explosion sounds
        if (hasPlayedExplosion)
        {
            return;
        }

        // Check if projectile hit an enemy
        if (targetObject.CompareTag("Enemy"))
        {
            hasPlayedExplosion = true;

            // Play explosion sound
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayPlayerExplosionSound();
            }
        }
    }
}