using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header ("Fade Settings")]
    public float fadeDuration = 0.5f;

    private CanvasGroup fadeCanvasGroup;
    private bool isTransitioning = false;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this)
        {
            Destroy (gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad (gameObject);

        // Create fade canvas
        SetupFadeCanvas ();
    }

    private void SetupFadeCanvas ()
    {
        // Create a canvas for the fade overlay
        GameObject canvasObj = new GameObject ("FadeCanvas");
        canvasObj.transform.SetParent (transform);

        Canvas canvas = canvasObj.AddComponent<Canvas> ();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler> ();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster> ();

        // Create black panel
        GameObject panel = new GameObject ("FadePanel");
        panel.transform.SetParent (canvasObj.transform, false);

        UnityEngine.UI.Image image = panel.AddComponent<UnityEngine.UI.Image> ();
        image.color = Color.black;

        RectTransform rect = panel.GetComponent<RectTransform> ();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        fadeCanvasGroup = panel.AddComponent<CanvasGroup> ();
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }

    // ══════════════════════════════════════════════════════════════════════
    public void TransitionToScene (int sceneIndex)
    {
        if (isTransitioning) return;
        StartCoroutine (DoTransition (sceneIndex));
    }

    private IEnumerator DoTransition (int sceneIndex)
    {
        isTransitioning = true;
        fadeCanvasGroup.blocksRaycasts = true;

        // Fade to black
        yield return StartCoroutine (Fade (0f, 1f));

        // Load scene
        SceneManager.LoadScene (sceneIndex);

        // Wait a frame for scene to load
        yield return null;

        // Fade back in
        yield return StartCoroutine (Fade (1f, 0f));

        fadeCanvasGroup.blocksRaycasts = false;
        isTransitioning = false;
    }

    private IEnumerator Fade (float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp (from, to, elapsed / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = to;
    }
}