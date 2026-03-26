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
    private bool hasLoadedSave = false;  // prevents Start() overwriting loaded state

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
        if (!hasLoadedSave)
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
        dayActive   = true;
        timePaused  = true;
        onDayStart?.Invoke ();
        Debug.Log ($"Day {currentDay} begins. Season: {currentSeason}. Year: {currentYear}");

        // Check for festival today and fire notifications
        if (FestivalManager.Instance != null)
        {
            FestivalManager.Instance.CheckFestivalForCurrentDay ();

            // Notify if today is a festival
            if (FestivalManager.Instance.IsFestivalDay
                && FestivalNotification.Instance != null)
            {
                FestivalNotification.Instance.ShowToday (
                    FestivalManager.Instance.CurrentFestival);
            }

            // Notify if tomorrow is a festival
            var tomorrow = FestivalManager.Instance.GetTomorrowsFestival ();
            if (tomorrow.type != FestivalManager.FestivalType.None
                && FestivalNotification.Instance != null)
            {
                FestivalNotification.Instance.ShowTomorrowDelayed (tomorrow, 6f);
            }
        }
    }

    public void EndDay ()
    {
        dayActive  = false;
        timePaused = true;
        onDayEnd?.Invoke ();
        Debug.Log ($"Day {currentDay} ends.");

        // Store the day that just ended before advancing
        GameState.Instance.lastCompletedDay = currentDay;

        // Store festival name for the summary screen before advancing the day
        if (FestivalManager.Instance != null && FestivalManager.Instance.IsFestivalDay)
            GameState.Instance.lastCompletedDayFestival = FestivalManager.Instance.CurrentFestival.displayName;
        else
            GameState.Instance.lastCompletedDayFestival = "";

        // Process divine favour decay and negative favour events
        GameState.Instance.ProcessEndOfDayFavour ();

        AudioManager.Instance?.PlayDayEndSleep ();

        // Process daily gossip from Stephanos
        GossipManager.ProcessDailyGossip ();

        // Process quest end of day
        if (QuestManager.Instance != null)
            QuestManager.Instance.ProcessEndOfDay ();

        // Marriage daily wealth bonus
        if (GameState.Instance.romanceStage == GameState.RomanceStage.Married)
        {
            GameState.Instance.AddDrachma (5f);
            GameState.Instance.goalMarriageComplete = true;
        }

        // Shop passive income
        if (GameState.Instance.hasShop)
            ApplyShopIncome ();

        // Evaluate life goals
        GameState.Instance.EvaluateLifeGoals ();

        // Advance the day counter now so summary shows correct next-day info
        AdvanceDay ();

        // Load end of day scene
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.TransitionToScene (endOfDaySceneIndex);
    }

    private void ApplyShopIncome ()
    {
        if (GameState.Instance == null) return;
        float multiplier = GameState.Instance.GetCareerMultiplier ();
        switch (GameState.Instance.currentProfession){
        case GameState.Profession.Merchant:
            GameState.Instance.AddDrachma (15f * multiplier);
            GameState.Instance.pendingEndOfDayEvents.Add (
            $"Your {PropertyBrokerUI.GetShopName ()} earns ₯{(int)(15f * multiplier)} today.");
            break;
        case GameState.Profession.Soldier:
            GameState.Instance.AddDrachma (10f * multiplier);
            GameState.Instance.AddHonour (2);
            GameState.Instance.pendingEndOfDayEvents.Add (
            $"Your {PropertyBrokerUI.GetShopName ()} earns ₯{(int)(10f * multiplier)} and 2 honour today.");
            break;
        case GameState.Profession.Philosopher:
            GameState.Instance.AddDrachma (12f * multiplier);
            GameState.Instance.AddCareerXP (5);
            GameState.Instance.pendingEndOfDayEvents.Add (
            $"Your {PropertyBrokerUI.GetShopName ()} earns ₯{(int)(12f * multiplier)} and 5 career XP today.");
            break;
        case GameState.Profession.Craftsman:
            GameState.Instance.AddDrachma (15f * multiplier);
            GameState.Instance.pendingEndOfDayEvents.Add (
            $"Your {PropertyBrokerUI.GetShopName ()} earns ₯{(int)(15f * multiplier)} today.");
            break;
        case GameState.Profession.Priest:
            GameState.Instance.AddDrachma (10f * multiplier);
            GameState.Instance.ChangeFavour (GameState.Instance.patronGod, 5);
            GameState.Instance.pendingEndOfDayEvents.Add (
            $"Your {PropertyBrokerUI.GetShopName ()} earns ₯{(int)(10f * multiplier)} and 5 patron favour today.");
            break;
        }
    }

    private void CheckLoanDeadline ()
    {
        if (GameState.Instance == null) return;
        if (!GameState.Instance.hasActiveLoan) return;
        if (currentDay < GameState.Instance.loanDeadlineDay) return;

        // Deadline reached and not repaid
        if (!GameState.Instance.loanExtended)
        {
            // First miss — give one extension
            GameState.Instance.loanExtended    = true;
            GameState.Instance.loanDeadlineDay = currentDay + 3;
            GameState.Instance.ChangeRelationship ("Kallias", -20);
            GameState.Instance.AddHonour (-5);
            GameState.Instance.pendingEndOfDayEvents.Add (
            "You have missed your repayment deadline. Kallias is furious. (-20 Kallias relationship, -5 honour) You have 3 more days.");
        }
        else
        {
            // Second miss — default
            GameState.Instance.hasActiveLoan   = false;
            GameState.Instance.loanDefaulted   = true;
            GameState.Instance.loanAmount      = 0f;
            GameState.Instance.loanRepayAmount = 0f;
            GameState.Instance.ChangeRelationship ("Kallias", -30);
            GameState.Instance.AddHonour (-10);
            GameState.Instance.pendingEndOfDayEvents.Add (
            "You have defaulted on your loan. Kallias will never lend to you again. (-30 Kallias relationship, -10 honour)");
        }
    }

    [Header ("Scene Indices")]
    public int endOfDaySceneIndex = 11;

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

    public void LoadTimeState (float hour, int day, int year, Season season)
    {
        currentHour   = Mathf.Floor (hour);
        currentDay    = day;
        currentYear   = year;
        currentSeason = season;
        dayActive     = true;
        timePaused    = true;
        hasLoadedSave = true;
        Debug.Log ($"Time state loaded — Day {currentDay}, {currentSeason}, Year {currentYear}");
    }

    public float GetCurrentHour () => currentHour;
    public int GetCurrentDay () => currentDay;
    public int GetCurrentYear () => currentYear;
    public Season GetCurrentSeason () => currentSeason;
    public bool IsDayActive () => dayActive;

    /// <summary>
    /// Returns the day number within the current season (1–28).
    /// </summary>
    public int GetDayOfSeason ()
    {
        return ((currentDay - 1) % daysPerSeason) + 1;
    }

    /// <summary>
    /// Returns the day number within the season for a given absolute day.
    /// Used by EndOfDaySummary to show the correct day for a completed day.
    /// </summary>
    public int GetDayOfSeason (int absoluteDay)
    {
        return ((absoluteDay - 1) % daysPerSeason) + 1;
    }

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