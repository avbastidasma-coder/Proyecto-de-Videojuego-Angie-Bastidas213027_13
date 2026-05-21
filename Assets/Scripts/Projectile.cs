using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    private bool alreadyHit = false;

    private void Update()
    {
        // Move projectile
        transform.position +=
            Vector3.left * speed * Time.deltaTime;

        // Destroy outside screen
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alreadyHit)
        {
            return;
        }

        // Detect player
        if (collision.CompareTag("Player"))
        {
            alreadyHit = true;

            Debug.Log("Player was hit");

            // Damage animation
            PlayerController player =
                collision.GetComponent<PlayerController>();

            if (player != null)
            {
                player.ShowDamage();
            }

            // Remove life
            if (GameManager.instance != null)
            {
                GameManager.instance.PlayerHit();
            }

            Destroy(gameObject);
        }
    }
}