using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonWithSound : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneName;

    [Header("Button Click Delay")]
    public float delayBeforeSceneLoad = 0.25f;

    // Load scene with button sound
    public void LoadSceneWithSound()
    {
        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        // Play button click sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayButtonClickSound();
        }

        // Wait before changing scene
        yield return new WaitForSecondsRealtime(delayBeforeSceneLoad);

        // Restore normal time scale
        Time.timeScale = 1f;

        // Load selected scene
        SceneManager.LoadScene(sceneName);
    }
}