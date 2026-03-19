using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [Header ("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI taglineText;
    public Slider          loadingBar;
    public TextMeshProUGUI loadingText;

    [Header ("Scene Indices")]
    public int mainMenuSceneIndex = 1;

    [Header ("Settings")]
    public float minimumLoadTime = 5f;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        if (titleText   != null) titleText.text   = "POLIS";
        if (taglineText != null) taglineText.text = "Live a life. Build a legacy. Appease the gods.";
        if (loadingBar  != null) loadingBar.value = 0f;
        if (loadingText != null) loadingText.text = "Loading...";

        StartCoroutine (LoadMainMenu ());
    }

    private IEnumerator LoadMainMenu ()
    {
        float startTime = Time.time;

        AsyncOperation op = SceneManager.LoadSceneAsync (mainMenuSceneIndex);
        op.allowSceneActivation = false;

        // Run until both the scene is ready AND minimum time has elapsed
        while (!op.isDone)
        {
            float loadProgress = Mathf.Clamp01 (op.progress / 0.9f);
            float timeProgress = Mathf.Clamp01 ((Time.time - startTime) / minimumLoadTime);

            // Show the slower of the two so the bar never jumps ahead of the timer
            float displayProgress = Mathf.Min (loadProgress, timeProgress);

            if (loadingBar  != null) loadingBar.value = displayProgress;
            if (loadingText != null) loadingText.text = $"Loading... {Mathf.RoundToInt (displayProgress * 100f)}%";

            bool sceneReady  = op.progress >= 0.9f;
            bool timeElapsed = (Time.time - startTime) >= minimumLoadTime;

            if (sceneReady && timeElapsed)
            {
                if (loadingBar  != null) loadingBar.value = 1f;
                if (loadingText != null) loadingText.text = "Ready.";
                yield return new WaitForSeconds (0.3f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}