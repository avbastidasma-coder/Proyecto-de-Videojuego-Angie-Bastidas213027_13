using System.Collections;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;

    [Header("Projectile Sound")]
    public AudioClip projectileSound;

    [Range(0f, 1f)]
    public float projectileVolume = 1f;

    [Header("Attack Spawn Points")]
    public Transform spawnAttackUp;
    public Transform spawnAttackDown;

    [Header("Attack Configuration")]
    public int totalAttacks = 3;
    public float firstAttackDelay = 1f;
    public float timeBetweenAttacks = 2f;

    [Header("Attack Order")]
    public bool useRandomOrder = true;

    [Header("Enemy Visual Controller")]
    public EnemyVisualController visualController;

    private int attacksDone = 0;
    private Coroutine attackRoutine;

    private void OnEnable()
    {
        // Reset attack counter
        attacksDone = 0;

        // Automatically get visual controller if missing
        if (visualController == null)
        {
            visualController = GetComponent<EnemyVisualController>();
        }

        // Stop previous coroutine if it exists
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        // Start enemy attack sequence
        attackRoutine = StartCoroutine(AttackSequence());
    }

    private void OnDisable()
    {
        // Stop attack coroutine when object is disabled
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    private IEnumerator AttackSequence()
    {
        // Wait before first attack
        yield return new WaitForSeconds(firstAttackDelay);

        // Launch attacks until the limit is reached
        while (attacksDone < totalAttacks)
        {
            LaunchProjectile();

            attacksDone++;

            yield return new WaitForSeconds(timeBetweenAttacks);
        }

        attackRoutine = null;

        // Notify GameManager when all attacks are finished
        if (GameManager.instance != null)
        {
            GameManager.instance.EnemyFinishedAttacks(gameObject);
        }
    }

    private void LaunchProjectile()
    {
        // Prevent errors if projectile prefab is missing
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Projectile Prefab is missing in " + gameObject.name);
            return;
        }

        // Select attack spawn position
        Transform selectedSpawn = ChooseSpawnPoint();

        // Prevent errors if spawn points are missing
        if (selectedSpawn == null)
        {
            Debug.LogWarning("Spawn points are missing in " + gameObject.name);
            return;
        }

        int attackNumber = attacksDone + 1;

        // Play enemy attack animation
        if (visualController != null)
        {
            visualController.PlayAttack(attackNumber);
        }

        // Play projectile sound
        if (AudioManager.instance != null && projectileSound != null)
        {
            AudioManager.instance.PlaySound(projectileSound, projectileVolume);
        }

        // Create projectile
        GameObject newProjectile = Instantiate(
            projectilePrefab,
            selectedSpawn.position,
            Quaternion.identity
        );

        // Assign projectile contact sound
        EnemyProjectileSound projectileAudio =
            newProjectile.GetComponent<EnemyProjectileSound>();

        if (projectileAudio != null)
        {
            projectileAudio.contactSound = projectileSound;
        }

        Debug.Log("Enemy attack number: " + attackNumber);
    }

    private Transform ChooseSpawnPoint()
    {
        // Use random spawn order
        if (useRandomOrder)
        {
            return Random.value > 0.5f
                ? spawnAttackUp
                : spawnAttackDown;
        }

        // Alternate spawn positions
        if (attacksDone % 2 == 0)
        {
            return spawnAttackUp;
        }

        return spawnAttackDown;
    }
}