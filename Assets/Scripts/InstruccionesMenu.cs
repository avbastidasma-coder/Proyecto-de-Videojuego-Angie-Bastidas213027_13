using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionsMenu : MonoBehaviour
{
    [Header("Button Click Delay")]
    public float clickDelay = 0.25f;

    // Return to the main menu
    public void GoToMainMenu()
    {
        StartCoroutine(LoadMainMenu());
    }

    private IEnumerator LoadMainMenu()
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

        // Load main menu scene
        SceneManager.LoadScene("Menu");
    }
}