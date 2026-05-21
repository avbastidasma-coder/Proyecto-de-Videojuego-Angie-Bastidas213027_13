using UnityEngine;

public class PlayerCoinMovement : MonoBehaviour
{
    // =========================================
    // NORMAL PLAYER SCRIPT
    // =========================================

    [Header("Normal Player Controller")]
    public MonoBehaviour normalPlayerController;

    // =========================================
    // PLAYER SPRITES
    // =========================================

    [Header("Player Sprites")]
    public SpriteRenderer spriteRenderer;

    public Sprite normalSprite;

    public Sprite jumpSprite;

    public Sprite crouchSprite;

    // =========================================
    // COIN PHASE MOVEMENT
    // =========================================

    [Header("Jump Settings")]
    public float jumpHeightY = 1.4f;

    public float jumpSpeed = 6f;

    [Header("Crouch Settings")]
    public float crouchDownY = 1f;

    public float crouchSpeed = 14f;

    // =========================================
    // COMPONENTS
    // =========================================

    private Rigidbody2D rb;

    // =========================================
    // STATES
    // =========================================

    private bool wasInCoinPhase = false;

    private bool isJumping = false;

    private bool isFallingFromJump = false;

    private bool isCrouching = false;

    // =========================================
    // POSITION DATA
    // =========================================

    private float originalX;

    private float baseY;

    private float targetY;

    // =========================================
    // RIGIDBODY DATA
    // =========================================

    private float originalGravity;

    private RigidbodyType2D originalBodyType;

    // =========================================
    // SPRITE DATA
    // =========================================

    private Sprite originalSprite;

    // =========================================
    // AWAKE
    // =========================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
        {
            spriteRenderer =
                GetComponent<SpriteRenderer>();
        }
    }

    // =========================================
    // START
    // =========================================

    private void Start()
    {
        originalX = transform.position.x;

        baseY = transform.position.y;

        targetY = baseY;

        if (rb != null)
        {
            originalGravity =
                rb.gravityScale;

            originalBodyType =
                rb.bodyType;
        }

        if (spriteRenderer != null)
        {
            originalSprite =
                spriteRenderer.sprite;
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
        // COIN PHASE ACTIVE
        // =====================================

        if (GameManager.instance.coinPhase)
        {
            if (!wasInCoinPhase)
            {
                EnterCoinPhase();
            }

            ReadCoinControls();

            MoveDuringCoinPhase();

            UpdatePlayerSprite();
        }

        // =====================================
        // NORMAL GAMEPLAY
        // =====================================

        else
        {
            if (wasInCoinPhase)
            {
                ExitCoinPhase();
            }
        }
    }

    // =========================================
    // ENTER COIN PHASE
    // =========================================

    private void EnterCoinPhase()
    {
        wasInCoinPhase = true;

        isJumping = false;

        isFallingFromJump = false;

        isCrouching = false;

        originalX = transform.position.x;

        baseY = transform.position.y;

        targetY = baseY;

        // Disable normal controller
        if (normalPlayerController != null)
        {
            normalPlayerController.enabled =
                false;
        }

        // Configure rigidbody
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;

            rb.gravityScale = 0f;

            rb.bodyType =
                RigidbodyType2D.Kinematic;
        }

        ChangeToNormalSprite();
    }

    // =========================================
    // EXIT COIN PHASE
    // =========================================

    private void ExitCoinPhase()
    {
        wasInCoinPhase = false;

        isJumping = false;

        isFallingFromJump = false;

        isCrouching = false;

        // Restore position
        Vector3 finalPosition =
            transform.position;

        finalPosition.x = originalX;

        finalPosition.y = baseY;

        transform.position =
            finalPosition;

        // Restore rigidbody
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;

            rb.gravityScale =
                originalGravity;

            rb.bodyType =
                originalBodyType;
        }

        // Enable normal controller
        if (normalPlayerController != null)
        {
            normalPlayerController.enabled =
                true;
        }

        RestoreNormalSprite();
    }

    // =========================================
    // INPUT
    // =========================================

    private void ReadCoinControls()
    {
        // JUMP
        if (
            Input.GetKeyDown(KeyCode.UpArrow) &&
            !isJumping &&
            !isFallingFromJump &&
            !isCrouching
        )
        {
            StartJump();
        }

        // CROUCH SOUND
        if (
            Input.GetKeyDown(KeyCode.DownArrow) &&
            !isJumping &&
            !isFallingFromJump
        )
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance
                    .PlayCrouchSound();
            }
        }

        // CROUCH
        if (
            Input.GetKey(KeyCode.DownArrow) &&
            !isJumping &&
            !isFallingFromJump
        )
        {
            isCrouching = true;

            targetY =
                baseY - crouchDownY;
        }

        // RETURN TO NORMAL
        else if (
            !isJumping &&
            !isFallingFromJump
        )
        {
            isCrouching = false;

            targetY = baseY;
        }
    }

    // =========================================
    // START JUMP
    // =========================================

    private void StartJump()
    {
        isJumping = true;

        isFallingFromJump = false;

        isCrouching = false;

        targetY =
            baseY + jumpHeightY;

        if (AudioManager.instance != null)
        {
            AudioManager.instance
                .PlayJumpSound();
        }

        ChangeToJumpSprite();
    }

    // =========================================
    // MOVEMENT
    // =========================================

    private void MoveDuringCoinPhase()
    {
        Vector3 newPosition =
            transform.position;

        // Lock X position
        newPosition.x = originalX;

        // =====================================
        // JUMP UP
        // =====================================

        if (isJumping)
        {
            newPosition.y =
                Mathf.MoveTowards(
                    transform.position.y,
                    targetY,
                    jumpSpeed *
                    Time.deltaTime
                );

            if (
                Mathf.Abs(
                    newPosition.y - targetY
                ) < 0.01f
            )
            {
                isJumping = false;

                isFallingFromJump = true;

                targetY = baseY;
            }
        }

        // =====================================
        // FALL DOWN
        // =====================================

        else if (isFallingFromJump)
        {
            newPosition.y =
                Mathf.MoveTowards(
                    transform.position.y,
                    baseY,
                    jumpSpeed *
                    Time.deltaTime
                );

            if (
                Mathf.Abs(
                    newPosition.y - baseY
                ) < 0.01f
            )
            {
                isFallingFromJump = false;

                newPosition.y = baseY;

                targetY = baseY;
            }
        }

        // =====================================
        // CROUCH MOVEMENT
        // =====================================

        else
        {
            newPosition.y =
                Mathf.MoveTowards(
                    transform.position.y,
                    targetY,
                    crouchSpeed *
                    Time.deltaTime
                );
        }

        transform.position =
            newPosition;
    }

    // =========================================
    // SPRITES
    // =========================================

    private void UpdatePlayerSprite()
    {
        if (
            isJumping ||
            isFallingFromJump
        )
        {
            ChangeToJumpSprite();

            return;
        }

        if (isCrouching)
        {
            ChangeToCrouchSprite();

            return;
        }

        ChangeToNormalSprite();
    }

    private void ChangeToNormalSprite()
    {
        if (
            spriteRenderer != null &&
            normalSprite != null
        )
        {
            spriteRenderer.sprite =
                normalSprite;
        }
    }

    private void ChangeToJumpSprite()
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

    private void ChangeToCrouchSprite()
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

    private void RestoreNormalSprite()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (normalSprite != null)
        {
            spriteRenderer.sprite =
                normalSprite;
        }
        else if (originalSprite != null)
        {
            spriteRenderer.sprite =
                originalSprite;
        }
    }
}