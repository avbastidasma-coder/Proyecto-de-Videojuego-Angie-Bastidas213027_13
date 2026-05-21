using System.Collections;
using UnityEngine;

public class EnemyVisualController : MonoBehaviour
{
    [Header("Components")]
    public SpriteRenderer spriteRenderer;

    [Header("Enemy Sprites")]
    public Sprite neutralSprite;

    [Header("Attack Sprites")]
    public Sprite attackSprite1;
    public Sprite attackSprite2;

    [Header("Damage and Defeat Sprites")]
    public Sprite damageSprite;
    public Sprite defeatSprite;

    [Header("Animation Timers")]
    public float attackPoseTime = 0.4f;
    public float damagePoseTime = 0.8f;
    public float defeatPoseTime = 1.2f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        // Automatically get SpriteRenderer if missing
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        ShowNeutral();
    }

    // =========================
    // BASIC VISUAL STATES
    // =========================

    public void ShowNeutral()
    {
        if (spriteRenderer != null && neutralSprite != null)
        {
            spriteRenderer.sprite = neutralSprite;
        }
    }

    public void ShowAttack()
    {
        ShowAttack(1);
    }

    public void ShowAttack(int attackNumber)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        // Alternate attack sprites
        if (attackNumber % 2 == 0)
        {
            if (attackSprite2 != null)
            {
                spriteRenderer.sprite = attackSprite2;
            }
            else if (attackSprite1 != null)
            {
                spriteRenderer.sprite = attackSprite1;
            }
        }
        else
        {
            if (attackSprite1 != null)
            {
                spriteRenderer.sprite = attackSprite1;
            }
            else if (attackSprite2 != null)
            {
                spriteRenderer.sprite = attackSprite2;
            }
        }
    }

    public void ShowDamage()
    {
        if (spriteRenderer != null && damageSprite != null)
        {
            spriteRenderer.sprite = damageSprite;
        }
    }

    public void ShowDefeat()
    {
        if (spriteRenderer != null && defeatSprite != null)
        {
            spriteRenderer.sprite = defeatSprite;
        }
    }

    // =========================
    // ATTACK ANIMATION
    // =========================

    public void PlayAttack()
    {
        PlayAttack(1);
    }

    public void PlayAttack(int attackNumber)
    {
        // Stop current animation if running
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(AttackSequence(attackNumber));
    }

    private IEnumerator AttackSequence(int attackNumber)
    {
        ShowAttack(attackNumber);

        yield return new WaitForSeconds(attackPoseTime);

        ShowNeutral();

        currentRoutine = null;
    }

    // =========================
    // DAMAGE AND DEFEAT
    // =========================

    public void PlayDefeatSequence()
    {
        // Stop current animation if running
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(DefeatSequence());
    }

    private IEnumerator DefeatSequence()
    {
        ShowDamage();

        yield return new WaitForSeconds(damagePoseTime);

        ShowDefeat();

        yield return new WaitForSeconds(defeatPoseTime);

        currentRoutine = null;
    }

    // =========================
    // DAMAGE ALIASES
    // =========================

    public void PlayDamage()
    {
        PlayDefeatSequence();
    }

    public void ReceiveDamage()
    {
        PlayDefeatSequence();
    }

    public void TakeDamage()
    {
        PlayDefeatSequence();
    }

    public void Die()
    {
        PlayDefeatSequence();
    }
}