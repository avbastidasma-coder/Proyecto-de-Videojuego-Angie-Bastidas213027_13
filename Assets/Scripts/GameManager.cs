using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // =========================================
    // SINGLETON
    // =========================================

    public static GameManager instance;

    // =========================================
    // GAMEPLAY STATE
    // =========================================

    [Header("Gameplay State")]
    public bool levelStarted = false;
    public bool canPlayerAttack = false;
    public bool coinPhase = false;
    public bool gameOver = false;
    public bool isPaused = false;

    // =========================================
    // PLAYER
    // =========================================

    [Header("Player")]
    public int playerLives = 3;

    // =========================================
    // ENEMY
    // =========================================

    [Header("Enemy")]
    public GameObject enemyObject;
    public EnemyVisualController enemyVisualController;

    // =========================================
    // COINS
    // =========================================

    [Header("Coin System")]
    public GameObject coinPrefab;

    public Transform coinSpawnUp;
    public Transform coinSpawnDown;

    public int coinsToSpawnPerPhase = 3;

    public float delayAfterCoinFinished = 0.5f;

    private int collectedCoins = 0;
    private int finishedCoins = 0;

    // =========================================
    // HUD
    // =========================================

    [Header("HUD")]
    public GameObject hudPanel;

    public TextMeshProUGUI livesText;
    public TextMeshProUGUI coinsText;

    // =========================================
    // PANELS
    // =========================================

    [Header("Panels")]
    public GameObject levelIntroPanel;

    public RectTransform levelIntroPanelRect;

    public GameObject instructionCoinsPanel;

    public GameObject pausePanel;
    public GameObject defeatPanel;
    public GameObject victoryPanel;

    // =========================================
    // PANEL ANIMATION
    // =========================================

    [Header("Panel Animation")]
    public float panelExitDistance = 1200f;

    public float panelExitTime = 1f;

    public float delayBeforeGameplay = 3f;

    // =========================================
    // INTERNAL STATE
    // =========================================

    private bool enemyDefeated = false;

    // =========================================
    // AWAKE
    // =========================================

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================================
    // START
    // =========================================

    private void Start()
    {
        levelStarted = false;
        canPlayerAttack = false;
        coinPhase = false;
        gameOver = false;
        isPaused = false;

        UpdateHUD();

        // Show intro panel
        if (levelIntroPanel != null)
        {
            levelIntroPanel.SetActive(true);
        }

        // Hide HUD
        if (hudPanel != null)
        {
            hudPanel.SetActive(false);
        }

        // Hide other panels
        if (instructionCoinsPanel != null)
        {
            instructionCoinsPanel.SetActive(false);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (defeatPanel != null)
        {
            defeatPanel.SetActive(false);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        // Disable enemy attacks at start
        if (enemyObject != null)
        {
            EnemyAttackController attackController =
                enemyObject.GetComponent<EnemyAttackController>();

            if (attackController != null)
            {
                attackController.enabled = false;
            }
        }
    }

    // =========================================
    // START GAMEPLAY
    // =========================================

    public void StartGameplay()
    {
        StartCoroutine(StartGameplayRoutine());
    }

    private IEnumerator StartGameplayRoutine()
    {
        levelStarted = true;
        gameOver = false;
        coinPhase = false;
        canPlayerAttack = false;

        // Show HUD
        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
        }

        // Animate panel upward
        if (levelIntroPanelRect != null)
        {
            Vector2 startPos =
                levelIntroPanelRect.anchoredPosition;

            Vector2 endPos =
                startPos + new Vector2(0f, panelExitDistance);

            float elapsed = 0f;

            while (elapsed < panelExitTime)
            {
                elapsed += Time.deltaTime;

                float t = elapsed / panelExitTime;

                levelIntroPanelRect.anchoredPosition =
                    Vector2.Lerp(startPos, endPos, t);

                yield return null;
            }

            levelIntroPanelRect.anchoredPosition = endPos;
        }

        // Hide panel
        if (levelIntroPanel != null)
        {
            levelIntroPanel.SetActive(false);
        }

        // Wait before gameplay
        yield return new WaitForSeconds(delayBeforeGameplay);

        // Activate enemy attacks
        if (enemyObject != null)
        {
            EnemyAttackController attackController =
                enemyObject.GetComponent<EnemyAttackController>();

            if (attackController != null)
            {
                attackController.enabled = true;
            }
        }
    }

    // =========================================
    // ENEMY FINISHED ATTACKS
    // =========================================

    public void EnemyFinishedAttacks(GameObject enemy)
    {
        if (gameOver)
        {
            return;
        }

        canPlayerAttack = true;

        Debug.Log("Player can attack now.");
    }

    // =========================================
    // PLAYER ATTACK ENEMY
    // =========================================

    public void PlayerAttackEnemy(GameObject enemy)
    {
        if (gameOver)
        {
            return;
        }

        if (!canPlayerAttack)
        {
            return;
        }

        if (enemyDefeated)
        {
            return;
        }

        enemyDefeated = true;

        canPlayerAttack = false;

        // Disable enemy attacks
        EnemyAttackController attackController =
            enemy.GetComponent<EnemyAttackController>();

        if (attackController != null)
        {
            attackController.enabled = false;
        }

        // Enemy animation
        if (enemyVisualController != null)
        {
            enemyVisualController.PlayDefeatSequence();
        }

        // Sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayEnemyDamageSound();
        }

        StartCoroutine(ShowCoinInstructionsRoutine());
    }

    // =========================================
    // COIN INSTRUCTIONS
    // =========================================

    private IEnumerator ShowCoinInstructionsRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        Time.timeScale = 0f;

        if (instructionCoinsPanel != null)
        {
            instructionCoinsPanel.SetActive(true);
        }
    }

    // =========================================
    // START COIN PHASE
    // =========================================

    public void StartCoinPhaseFromButton()
    {
        Time.timeScale = 1f;

        if (instructionCoinsPanel != null)
        {
            instructionCoinsPanel.SetActive(false);
        }

        StartCoroutine(StartCoinPhaseRoutine());
    }

    private IEnumerator StartCoinPhaseRoutine()
    {
        coinPhase = true;

        collectedCoins = 0;
        finishedCoins = 0;

        UpdateHUD();

        for (int i = 0; i < coinsToSpawnPerPhase; i++)
        {
            SpawnCoin();

            yield return new WaitForSeconds(1f);
        }
    }

    private void SpawnCoin()
    {
        if (coinPrefab == null)
        {
            return;
        }

        Transform selectedSpawn =
            Random.value > 0.5f
            ? coinSpawnUp
            : coinSpawnDown;

        Instantiate(
            coinPrefab,
            selectedSpawn.position,
            Quaternion.identity
        );
    }

    // =========================================
    // COINS
    // =========================================

    public void CoinCollected()
    {
        if (!coinPhase)
        {
            return;
        }

        collectedCoins++;

        UpdateHUD();

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayCoinSound();
        }

        RegisterCoinFinished(true);
    }

    public void RegisterCoinFinished(bool collected)
    {
        if (!coinPhase)
        {
            return;
        }

        finishedCoins++;

        StartCoroutine(CheckCoinPhaseFinished());
    }

    private IEnumerator CheckCoinPhaseFinished()
    {
        yield return new WaitForSeconds(delayAfterCoinFinished);

        if (finishedCoins >= coinsToSpawnPerPhase)
        {
            StartVictory();
        }
    }

    // =========================================
    // PLAYER HIT
    // =========================================

    public void PlayerHit()
    {
        if (gameOver)
        {
            return;
        }

        playerLives--;

        UpdateHUD();

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayPlayerDamageSound();
        }

        if (playerLives <= 0)
        {
            StartDefeat();
        }
    }

    // =========================================
    // DEFEAT
    // =========================================

    private void StartDefeat()
    {
        gameOver = true;

        levelStarted = false;
        canPlayerAttack = false;
        coinPhase = false;

        Time.timeScale = 0f;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayGameOverSound();
        }

        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
        }
    }

    // =========================================
    // VICTORY
    // =========================================

    private void StartVictory()
    {
        gameOver = true;

        levelStarted = false;
        canPlayerAttack = false;
        coinPhase = false;

        Time.timeScale = 0f;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayVictorySound();
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
    }

    // =========================================
    // PAUSE
    // =========================================

    public void TogglePause()
    {
        if (gameOver)
        {
            return;
        }

        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }
    }

    // =========================================
    // HUD
    // =========================================

    private void UpdateHUD()
    {
        if (livesText != null)
        {
            livesText.text = "X " + playerLives;
        }

        if (coinsText != null)
        {
            coinsText.text = "X " + collectedCoins;
        }
    }

    // =========================================
    // BUTTONS
    // =========================================

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}