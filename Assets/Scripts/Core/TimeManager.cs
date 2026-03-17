using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    // ── Singleton ──────────────────────────────────────────────────────────
    public static TimeManager Instance { get; private set; }

    // ── Time Settings ──────────────────────────────────────────────────────
    [Header ("Day Settings")]
    [Range (6, 8)]
    public int dayStartHour = 6;       // Dawn
    [Range (22, 24)]
    public int dayEndHour = 24;        // Midnight

    [Header ("Time Scale")]
    public float minutesPerRealSecond = 10f;  // How fast time passes

    // ── Internal State ─────────────────────────────────────────────────────
    private float currentHour;         // e.g. 6.5 = 6:30am
    private int currentDay = 1;
    private int currentYear = 1;
    private bool dayActive = false;
    private bool timePaused = true;    // Time only moves when player acts

    // ── Season ─────────────────────────────────────────────────────────────
    public enum Season { Spring, Summer, Autumn, Winter }
    private Season currentSeason = Season.Spring;
    private const int daysPerSeason = 28;

    // ── Events ─────────────────────────────────────────────────────────────
    [HideInInspector] public UnityEvent onDayStart;
    [HideInInspector] public UnityEvent onDayEnd;
    [HideInInspector] public UnityEvent<Season> onSeasonChange;
    [HideInInspector] public UnityEvent onTimeChanged;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        // Singleton setup — only one TimeManager ever exists
        if (Instance != null && Instance != this)
        {
            Destroy (gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad (gameObject);
    }

    private void Start ()
    {
        StartDay ();
    }

    private void Update ()
    {
        if (!dayActive || timePaused) return;
        AdvanceTime ();
    }

    // ══════════════════════════════════════════════════════════════════════
    // Core time loop
    // ══════════════════════════════════════════════════════════════════════

    private void AdvanceTime ()
    {
        float hoursThisFrame = (minutesPerRealSecond / 60f) * Time.deltaTime;
        currentHour += hoursThisFrame;
        onTimeChanged?.Invoke ();

        if (currentHour >= dayEndHour)
            EndDay ();
    }

    public void StartDay ()
    {
        currentHour = dayStartHour;
        dayActive = true;
        timePaused = true;   // Time starts paused — resumes when player moves
        onDayStart?.Invoke ();
        Debug.Log ($"Day {currentDay} begins. Season: {currentSeason}. Year: {currentYear}");
    }

    public void EndDay ()
    {
        dayActive  = false;
        timePaused = true;
        onDayEnd?.Invoke ();
        Debug.Log ($"Day {currentDay} ends.");

        // Advance the day counter now so summary shows correct next-day info
        AdvanceDay ();

        // Load end of day scene
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.TransitionToScene (endOfDaySceneIndex);
    }

    [Header ("Scene Indices")]
    public int endOfDaySceneIndex = 9;

    private void AdvanceDay ()
    {
        currentDay++;
        CheckSeasonChange ();
        StartDay ();
    }

    private void CheckSeasonChange ()
    {
        int totalDays = (currentDay - 1);
        Season newSeason = (Season)((totalDays / daysPerSeason) % 4);

        if (newSeason != currentSeason)
        {
            currentSeason = newSeason;
            onSeasonChange?.Invoke (currentSeason);
            Debug.Log ($"Season changed to: {currentSeason}");
        }

        // New year every 4 seasons
        if (currentDay % (daysPerSeason * 4) == 1 && currentDay > 1)
        {
            currentYear++;
            Debug.Log ($"Year {currentYear} begins.");
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // Public controls — called by player actions
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Call this whenever the player does something that should cost time.
    /// Pass in minutes — e.g. AdvanceTimeByMinutes (30) for a conversation.
    /// </summary>
    public void AdvanceTimeByMinutes (float minutes)
    {
        if (!dayActive) return;
        currentHour += minutes / 60f;
        onTimeChanged?.Invoke ();

        if (currentHour >= dayEndHour)
            EndDay ();
    }

    /// <summary>
    /// Pause or resume the passive time flow.
    /// </summary>
    public void SetTimePaused (bool paused)
    {
        timePaused = paused;
    }

    // ══════════════════════════════════════════════════════════════════════
    // Getters
    // ══════════════════════════════════════════════════════════════════════

    public float GetCurrentHour () => currentHour;
    public int GetCurrentDay () => currentDay;
    public int GetCurrentYear () => currentYear;
    public Season GetCurrentSeason () => currentSeason;
    public bool IsDayActive () => dayActive;

    /// <summary>
    /// Returns time as a readable string e.g. "06:30"
    /// </summary>
    public string GetTimeString ()
    {
        int hours = Mathf.FloorToInt (currentHour);
        int minutes = Mathf.FloorToInt ((currentHour - hours) * 60f);
        return $"{hours:D2}:{minutes:D2}";
    }

    /// <summary>
    /// Returns 0.0 (dawn) to 1.0 (midnight) — useful for lighting.
    /// </summary>
    public float GetDayProgress ()
    {
        return (currentHour - dayStartHour) / (dayEndHour - dayStartHour);
    }
}