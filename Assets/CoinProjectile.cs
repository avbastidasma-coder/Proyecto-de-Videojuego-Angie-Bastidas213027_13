using UnityEngine;

public class CoinProjectile : MonoBehaviour
{
    public float speed = 3f;

    private bool processed = false;

    private void Update()
    {
        transform.position +=
            Vector3.left *
            speed *
            Time.deltaTime;

        if (transform.position.x < -10f)
        {
            CoinMissed();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (processed)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            processed = true;

            if (GameManager.instance != null)
            {
                GameManager.instance.CoinCollected();
            }

            Destroy(gameObject);
        }
    }

    private void CoinMissed()
    {
        if (processed)
        {
            return;
        }

        processed = true;

        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterCoinFinished(false);
        }

        Destroy(gameObject);
    }
}