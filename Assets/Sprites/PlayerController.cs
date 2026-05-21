using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // =========================================
    // MOVEMENT
    // =========================================

    [Header("Movement")]
    public float jumpForce = 8f;

    // =========================================
    // PLAYER SPRITES
    // =========================================

    [Header("Player Sprites")]
    public Sprite normalSprite;

    public Sprite jumpSprite;

    public Sprite crouchSprite;

    public Sprite damageSprite;

    // =========================================
    // ATTACK
    // =========================================

    [Header("Attack")]
    public Sprite[] attackSprites;

    public float attackFrameTime = 0.08f;

    public GameObject playerProjectilePrefab;

    public Transform attackSpawnPoint;

    // =========================================
    // COMPONENTS
    // =========================================

    [Header("Components")]
    public Rigidbody2D rb;

    public BoxCollider2D playerCollider;

    public SpriteRenderer spriteRenderer;

    // =========================================
    // STATES
    // =========================================

    private bool isGrounded = false;

    private bool isCrouching = false;

    private bool isAttacking = false;

    private bool isTakingDamage = false;

    // =========================================
    // COLLIDER
    // =========================================

    private Vector2 originalColliderSize;

    private Vector2 originalColliderOffset;

    // =========================================
    // START
    // =========================================

    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (playerCollider == null)
        {
            playerCollider =
                GetComponent<BoxCollider2D>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer =
                GetComponent<SpriteRenderer>();
        }

        originalColliderSize =
            playerCollider.size;

        originalColliderOffset =
            playerCollider.offset;

        SetSprite(normalSprite);
    }

    // =========================================
    // UPDATE
    // =========================================

    private void Update()
    {
        if (GameManager.instance == null)
        {
            return;
        }

        if (GameManager.instance.gameOver)
        {
            return;
        }

        if (!GameManager.instance.levelStarted)
        {
            return;
        }

        if (GameManager.instance.isPaused)
        {
            return;
        }

        // Disable normal controls during coin phase
        if (GameManager.instance.coinPhase)
        {
            return;
        }

        // =====================================
        // JUMP
        // =====================================

        if (
            (
                Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.W)
            )
            && isGrounded
        )
        {
            Jump();
        }

        // =====================================
        // CROUCH
        // =====================================

        if (
            Input.GetKeyDown(KeyCode.DownArrow) ||
            Input.GetKeyDown(KeyCode.S)
        )
        {
            Crouch();
        }

        // =====================================
        // STAND UP
        // =====================================

        if (
            Input.GetKeyUp(KeyCode.DownArrow) ||
            Input.GetKeyUp(KeyCode.S)
        )
        {
            StandUp();
        }

        // =====================================
        // ATTACK
        // =====================================

        if (
            Input.GetKeyDown(KeyCode.Space) &&
            !isAttacking
        )
        {
            Attack();
        }
    }

    // =========================================
    // JUMP
    // =========================================

    private void Jump()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance
                .PlayJumpSound();
        }

        rb.linearVelocity =
            new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

        isGrounded = false;

        isCrouching = false;

        RestoreCollider();

        if (!isTakingDamage)
        {
            SetSprite(jumpSprite);
        }
    }

    // =========================================
    // CROUCH
    // =========================================

    private void Crouch()
    {
        if (isCrouching || isAttacking)
        {
            return;
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance
                .PlayCrouchSound();
        }

        isCrouching = true;

        playerCollider.size =
            new Vector2(
                originalColliderSize.x,
                originalColliderSize.y * 0.5f
            );

        playerCollider.offset =
            new Vector2(
                originalColliderOffset.x,
                originalColliderOffset.y - 0.25f
            );

        if (!isTakingDamage)
        {
            SetSprite(crouchSprite);
        }
    }

    // =========================================
    // STAND UP
    // =========================================

    private void StandUp()
    {
        if (!isCrouching)
        {
            return;
        }

        isCrouching = false;

        RestoreCollider();

        if (!isTakingDamage && !isAttacking)
        {
            SetSprite(normalSprite);
        }
    }

    // =========================================
    // ATTACK
    // =========================================

    private void Attack()
    {
        if (!GameManager.instance.canPlayerAttack)
        {
            return;
        }

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        RestoreCollider();

        isCrouching = false;

        if (
            attackSprites != null &&
            attackSprites.Length > 0
        )
        {
            for (
                int i = 0;
                i < attackSprites.Length;
                i++
            )
            {
                SetSprite(attackSprites[i]);

                // Shoot in middle frame
                if (
                    i ==
                    attackSprites.Length / 2
                )
                {
                    LaunchProjectile();
                }

                yield return new WaitForSeconds(
                    attackFrameTime
                );
            }
        }
        else
        {
            LaunchProjectile();

            yield return new WaitForSeconds(
                0.2f
            );
        }

        isAttacking = false;

        yield return null;

        if (!isTakingDamage)
        {
            SetSprite(normalSprite);
        }
    }

    // =========================================
    // SHOOT PROJECTILE
    // =========================================

    private void LaunchProjectile()
    {
        if (
            playerProjectilePrefab == null ||
            attackSpawnPoint == null
        )
        {
            Debug.LogWarning(
                "Missing projectile prefab or spawn point."
            );

            return;
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance
                .PlayPlayerShootSound();
        }

        Instantiate(
            playerProjectilePrefab,
            attackSpawnPoint.position,
            Quaternion.identity
        );
    }

    // =========================================
    // DAMAGE
    // =========================================

    public void ShowDamage()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        StartCoroutine(DamageRoutine());
    }

    private IEnumerator DamageRoutine()
    {
        isTakingDamage = true;

        if (AudioManager.instance != null)
        {
            AudioManager.instance
                .PlayPlayerDamageSound();
        }

        SetSprite(damageSprite);

        yield return new WaitForSeconds(
            0.25f
        );

        isTakingDamage = false;

        if (isCrouching)
        {
            SetSprite(crouchSprite);
        }
        else if (!isGrounded)
        {
            SetSprite(jumpSprite);
        }
        else if (!isAttacking)
        {
            SetSprite(normalSprite);
        }
    }

    // =========================================
    // COLLIDER
    // =========================================

    private void RestoreCollider()
    {
        playerCollider.size =
            originalColliderSize;

        playerCollider.offset =
            originalColliderOffset;
    }

    // =========================================
    // SPRITES
    // =========================================

    private void SetSprite(Sprite sprite)
    {
        if (
            sprite != null &&
            spriteRenderer != null
        )
        {
            spriteRenderer.sprite =
                sprite;
        }
    }

    // =========================================
    // GROUND DETECTION
    // =========================================

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (
            collision.gameObject.CompareTag(
                "Ground"
            )
        )
        {
            isGrounded = true;

            if (
                !isCrouching &&
                !isTakingDamage &&
                !isAttacking
            )
            {
                SetSprite(normalSprite);
            }
        }
    }
}