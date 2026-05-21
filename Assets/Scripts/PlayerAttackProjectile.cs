using UnityEngine;

public class PlayerAttackProjectile : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6f;
    public float lifeTime = 3f;
    public Vector2 direction = Vector2.right;

    [Header("Explosion Settings")]
    public GameObject explosionPrefab;
    public float explosionScale = 5f;

    [Header("Collision Settings")]
    public string enemyTag = "Enemy";
    public bool destroyOnHit = true;

    private bool hasHit = false;

    private void Start()
    {
        // Warning if explosion prefab is missing
        if (explosionPrefab == null)
        {
            Debug.LogWarning(
                "Explosion Prefab is missing in projectile: " + gameObject.name
            );
        }

        // Destroy projectile automatically after lifetime
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // Move projectile
        transform.Translate(
            direction.normalized * speed * Time.deltaTime,
            Space.World
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckHit(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckHit(collision.gameObject);
    }

    private void CheckHit(GameObject otherObject)
    {
        // Prevent multiple hits
        if (hasHit)
        {
            return;
        }

        // Ignore non-enemy objects
        if (!otherObject.CompareTag(enemyTag))
        {
            return;
        }

        hasHit = true;

        // Play explosion sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayPlayerExplosionSound();
        }

        // Create visual explosion effect
        CreateExplosion();

        // Notify GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.PlayerAttackEnemy(otherObject);
        }

        // Destroy projectile after impact
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    private void CreateExplosion()
    {
        // Prevent missing prefab errors
        if (explosionPrefab == null)
        {
            Debug.LogWarning(
                "Explosion Prefab is missing in: " + gameObject.name
            );

            return;
        }

        // Create explosion effect
        GameObject explosion = Instantiate(
            explosionPrefab,
            transform.position,
            Quaternion.identity
        );

        // Adjust explosion scale
        explosion.transform.localScale = new Vector3(
            explosionScale,
            explosionScale,
            1f
        );
    }
}