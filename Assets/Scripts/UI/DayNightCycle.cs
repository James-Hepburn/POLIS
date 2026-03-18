using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header ("Scene Renderers to Tint")]
    public SpriteRenderer[] backgroundRenderers;

    // ── Colour keyframes ───────────────────────────────────────────────────
    // Progress values match TimeManager.GetDayProgress() — 0.0 to 1.0
    // Dawn (0.0) → Midday (0.28) → Late Afternoon (0.61) → Dusk (0.78) → Night (1.0)

    [Header ("Sky / Camera Background Colours")]
    public Color skyDawn          = new Color (0.20f, 0.13f, 0.08f); // deep warm orange
    public Color skyMidday        = new Color (0.36f, 0.60f, 0.85f); // Aegean blue
    public Color skyLateAfternoon = new Color (0.70f, 0.45f, 0.18f); // amber
    public Color skyDusk          = new Color (0.15f, 0.08f, 0.15f); // deep purple
    public Color skyNight         = new Color (0.03f, 0.03f, 0.10f); // near black

    [Header ("Sprite Tint Colours")]
    public Color tintDawn          = new Color (1.00f, 0.82f, 0.60f); // warm gold
    public Color tintMidday        = new Color (1.00f, 1.00f, 1.00f); // pure white — no tint
    public Color tintLateAfternoon = new Color (1.00f, 0.88f, 0.65f); // soft amber
    public Color tintDusk          = new Color (0.65f, 0.50f, 0.70f); // purple-grey
    public Color tintNight         = new Color (0.25f, 0.28f, 0.45f); // cool blue-grey

    // ── Internal ───────────────────────────────────────────────────────────
    private Camera mainCamera;

    // Keyframe progress stops — must be ascending 0→1
    private readonly float[] stops = { 0f, 0.28f, 0.61f, 0.78f, 1.0f };

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        mainCamera = Camera.main;
    }

    private void Update ()
    {
        if (TimeManager.Instance == null) return;

        float progress = TimeManager.Instance.GetDayProgress ();
        progress = Mathf.Clamp01 (progress);

        Color sky  = EvaluateColour (progress, skyDawn, skyMidday, skyLateAfternoon, skyDusk, skyNight);
        Color tint = EvaluateColour (progress, tintDawn, tintMidday, tintLateAfternoon, tintDusk, tintNight);

        // Apply camera background
        if (mainCamera != null)
            mainCamera.backgroundColor = sky;

        // Apply tint to all registered renderers
        foreach (SpriteRenderer sr in backgroundRenderers)
        {
            if (sr != null)
                sr.color = tint;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // Evaluates a colour at a given progress value across five keyframes
    // ══════════════════════════════════════════════════════════════════════

    private Color EvaluateColour (float t, Color c0, Color c1, Color c2, Color c3, Color c4)
    {
        Color[] colours = { c0, c1, c2, c3, c4 };

        // Find which segment we are in
        for (int i = 0; i < stops.Length - 1; i++)
        {
            if (t <= stops[i + 1])
            {
                float segmentT = Mathf.InverseLerp (stops[i], stops[i + 1], t);
                return Color.Lerp (colours[i], colours[i + 1], segmentT);
            }
        }
        return colours[colours.Length - 1];
    }

    // ══════════════════════════════════════════════════════════════════════
    private void OnValidate ()
    {
        // Live preview in editor — lets you see colours without playing
        if (!Application.isPlaying && Camera.main != null)
            Camera.main.backgroundColor = skyMidday;
    }
}