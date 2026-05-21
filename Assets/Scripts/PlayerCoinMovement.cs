using UnityEngine;

public class CoinPhasePlayerMovement : MonoBehaviour
{
    // =========================================
    // REFERENCES
    // =========================================

    [Header("References")]
    public MonoBehaviour normalPlayerController;

    public SpriteRenderer spriteRenderer;

    // =========================================
    // SPRITES
    // =========================================

    [Header("Sprites")]
    public Sprite idleSprite;

    public Sprite jumpSprite;

    public Sprite crouchSprite;

    // =========================================
    // MOVEMENT
    // =========================================

    [Header("Jump")]
    public float jumpHeight = 1.8f;

    public float jumpSpeed = 10f;

    [Header("Crouch")]
    public float crouchDepth = 1.4f;

    public float crouchSpeed = 18f;

    // =========================================
    // INTERNAL
    // =========================================

    private Rigidbody2D rb;

    private bool coinPhaseActive = false;

    private bool isJumping = false;

    private bool isFalling = false;

    private bool isCrouching = false;

    private float baseY;

    private float fixedX;

    private float targetY;

    private float originalGravity;

    private RigidbodyType2D originalBodyType;

    // =========================================
    // START
    // =========================================

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        fixedX = transform.position.x;

        baseY = transform.position.y;

        targetY = baseY;

        if (rb != null)
        {
            originalGravity = rb.gravityScale;

            originalBodyType = rb.bodyType;
        }

        if (spriteRenderer == null)
        {
            spriteRenderer =
                GetComponent<SpriteRenderer>();
        }
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

        // =====================================
        // ENTER COIN PHASE
        // =====================================

        if (
            GameManager.instance.coinPhase &&
            !coinPhaseActive
        )
        {
            EnterCoinPhase();
        }

        // =====================================
        // EXIT COIN PHASE
        // =====================================

        if (
            !GameManager.instance.coinPhase &&
            coinPhaseActive
        )
        {
            ExitCoinPhase();
        }

        // =====================================
        // ACTIVE
        // =====================================

        if (!coinPhaseActive)
        {
            return;
        }

        HandleInput();

        HandleMovement();

        UpdateSprite();
    }

    // =========================================
    // ENTER
    // =========================================

    private void EnterCoinPhase()
    {
        coinPhaseActive = true;

        fixedX = transform.position.x;

        baseY = transform.position.y;

        targetY = baseY;

        isJumping = false;

        isFalling = false;

        isCrouching = false;

        // Disable normal movement
        if (normalPlayerController != null)
        {
            normalPlayerController.enabled =
                false;
        }

        // Rigidbody setup
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;

            rb.gravityScale = 0f;

            rb.bodyType =
                RigidbodyType2D.Kinematic;
        }
    }

    // =========================================
    // EXIT
    // =========================================

    private void ExitCoinPhase()
    {
        coinPhaseActive = false;

        // Restore normal movement
        if (normalPlayerController != null)
        {
            normalPlayerController.enabled =
                true;
        }

        // Restore rigidbody
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;

            rb.gravityScale =
                originalGravity;

            rb.bodyType =
                originalBodyType;
        }

        // Restore position
        Vector3 pos = transform.position;

        pos.y = baseY;

        pos.x = fixedX;

        transform.position = pos;

        SetIdleSprite();
    }

    // =========================================
    // INPUT
    // =========================================

    private void HandleInput()
    {
        // JUMP
        if (
            Input.GetKeyDown(KeyCode.UpArrow) &&
            !isJumping &&
            !isFalling
        )
        {
            StartJump();
        }

        // CROUCH
        if (
            Input.GetKey(KeyCode.DownArrow) &&
            !isJumping &&
            !isFalling
        )
        {
            isCrouching = true;

            targetY =
                baseY - crouchDepth;
        }
        else if (
            !isJumping &&
            !isFalling
        )
        {
            isCrouching = false;

            targetY = baseY;
        }
    }

    // =========================================
    // JUMP
    // =========================================

    private void StartJump()
    {
        isJumping = true;

        isFalling = false;

        isCrouching = false;

        targetY =
            baseY + jumpHeight;

        if (AudioManager.instance != null)
        {
            AudioManager.instance
                .PlayJumpSound();
        }
    }

    // =========================================
    // MOVEMENT
    // =========================================

    private void HandleMovement()
    {
        Vector3 pos = transform.position;

        pos.x = fixedX;

        // =====================================
        // JUMP UP
        // =====================================

        if (isJumping)
        {
            pos.y =
                Mathf.MoveTowards(
                    transform.position.y,
                    targetY,
                    jumpSpeed *
                    Time.deltaTime
                );

            if (
                Mathf.Abs(
                    pos.y - targetY
                ) < 0.02f
            )
            {
                isJumping = false;

                isFalling = true;

                targetY = baseY;
            }
        }

        // =====================================
        // FALL
        // =====================================

        else if (isFalling)
        {
            pos.y =
                Mathf.MoveTowards(
                    transform.position.y,
                    baseY,
                    jumpSpeed *
                    Time.deltaTime
                );

            if (
                Mathf.Abs(
                    pos.y - baseY
                ) < 0.02f
            )
            {
                isFalling = false;

                pos.y = baseY;
            }
        }

        // =====================================
        // CROUCH
        // =====================================

        else
        {
            pos.y =
                Mathf.MoveTowards(
                    transform.position.y,
                    targetY,
                    crouchSpeed *
                    Time.deltaTime
                );
        }

        transform.position = pos;
    }

    // =========================================
    // SPRITES
    // =========================================

    private void UpdateSprite()
    {
        if (isJumping || isFalling)
        {
            SetJumpSprite();

            return;
        }

        if (isCrouching)
        {
            SetCrouchSprite();

            return;
        }

        SetIdleSprite();
    }

    private void SetIdleSprite()
    {
        if (
            spriteRenderer != null &&
            idleSprite != null
        )
        {
            spriteRenderer.sprite =
                idleSprite;
        }
    }

    private void SetJumpSprite()
    {
        if (
            spriteRenderer != null &&
            jumpSprite != null
        )
        {
            spriteRenderer.sprite =
                jumpSprite;
        }
    }

    private void SetCrouchSprite()
    {
        if (
            spriteRenderer != null &&
            crouchSprite != null
        )
        {
            spriteRenderer.sprite =
                crouchSprite;
        }
    }
}