using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    // Reference to the UI Button component
    private Button button;

    private void Awake()
    {
        // Get Button component
        button = GetComponent<Button>();

        // Add click listener
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    // Play button click sound
    private void PlayClickSound()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayButtonClickSound();
        }
    }
}