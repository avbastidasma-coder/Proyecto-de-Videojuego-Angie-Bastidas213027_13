using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrologueMenu : MonoBehaviour
{
    [Header("Button Click Delay")]
    public float clickDelay = 0.25f;

    // Load gameplay scene
    public void GoToGameplay()
    {
        Debug.Log("Next button pressed");

        StartCoroutine(LoadSceneWithClick("Gameplay"));
    }

    // Return to main menu
    public void GoToMainMenu()
    {
        Debug.Log("Home button pressed");

        StartCoroutine(LoadSceneWithClick("Menu"));
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

        // Restore normal time scale
        Time.timeScale = 1f;

        // Load selected scene
        SceneManager.LoadScene(sceneName);
    }
}