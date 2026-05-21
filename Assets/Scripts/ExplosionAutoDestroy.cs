using UnityEngine;

public class ExplosionAutoDestroy : MonoBehaviour
{
    [Header("Destroy Settings")]
    public float lifeTime = 0.1f;

    private void Start()
    {
        // Automatically destroy the object after a short time
        Destroy(gameObject, lifeTime);
    }
}