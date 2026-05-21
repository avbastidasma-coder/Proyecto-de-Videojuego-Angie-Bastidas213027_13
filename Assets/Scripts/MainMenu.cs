using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Button Click Delay")]
    public float clickDelay = 0.25f;

    // Start the game and load the prologue scene
    public void PlayGame()
    {
        StartCoroutine(LoadSceneWithClick("Prologue"));
    }

    // Open instructions scene
    public void GoToInstructions()
    {
        StartCoroutine(LoadSceneWithClick("Instructions"));
    }

    // Exit the game
    public void ExitGame()
    {
        StartCoroutine(ExitGameWithClick());
    }

    private IEnumerator LoadSceneWithClick(string sceneName)
    {
        // Play button click sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayButtonClickSound();
        }

        // Wait before changing scene
        yield return new WaitForSecondsRealtime(clickDelay);

        // Restore normal game speed
        Time.timeScale = 1f;

        // Load selected scene
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator ExitGameWithClick()
    {
        // Play button click sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayButtonClickSound();
        }

        // Wait before closing the game
        yield return new WaitForSecondsRealtime(clickDelay);

        Debug.Log("Closing Game");

        Application.Quit();
    }
}