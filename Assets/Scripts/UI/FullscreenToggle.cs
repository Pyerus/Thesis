    using UnityEngine;
    using UnityEngine.UI; // Required for UI elements

    public class FullscreenToggle : MonoBehaviour
    {
        public Toggle fullscreenToggle; // Assign your UI Toggle here in the Inspector

        void Start()
        {
            // Set the initial state of the toggle based on current fullscreen status
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = Screen.fullScreen;
                // Add a listener to the toggle's OnValueChanged event
                fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            }
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
            // Optionally, you can also set the FullScreenMode
            // Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        }
    }