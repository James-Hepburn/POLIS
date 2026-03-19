using UnityEngine;

public class FestivalManager : MonoBehaviour
{
    // ── Singleton ──────────────────────────────────────────────────────────
    public static FestivalManager Instance { get; private set; }

    // ── Festival Definition ────────────────────────────────────────────────
    public enum FestivalType
    {
        None,
        CityDionysia,
        Thargelia,
        Panathenaia,
        Hephaestia,
        Thesmophoria,
        Pyanopsia,
        Lenaia,
        Haloa
    }

    public enum FestivalBenefit
    {
        None,
        DoubleFavour,       // double prayer/offering gains for a specific god
        DoubleRelationship, // double relationship gains all day
        PassiveHonour,      // honour granted at day start
        DoubleRelationshipAndHonour
    }

    [System.Serializable]
    public struct FestivalData
    {
        public FestivalType        type;
        public string              displayName;
        public TimeManager.Season  season;
        public int                 dayOfSeason;   // 1–28
        public FestivalBenefit     benefit;
        public GameState.PatronGod relevantGod;   // used for DoubleFavour festivals
        public int                 passiveHonour; // used for PassiveHonour festivals
        public string              description;
    }

    // ── Festival Calendar ──────────────────────────────────────────────────
    public FestivalData[] festivals = new FestivalData[]
    {
        new FestivalData {
            type         = FestivalType.CityDionysia,
            displayName  = "City Dionysia",
            season       = TimeManager.Season.Spring,
            dayOfSeason  = 10,
            benefit      = FestivalBenefit.DoubleRelationship,
            passiveHonour = 0,
            description  = "The great theatre festival. The city celebrates with song and drama."
        },
        new FestivalData {
            type         = FestivalType.Thargelia,
            displayName  = "Thargelia",
            season       = TimeManager.Season.Spring,
            dayOfSeason  = 22,
            benefit      = FestivalBenefit.DoubleFavour,
            relevantGod  = GameState.PatronGod.Apollo,
            passiveHonour = 0,
            description  = "A purification festival. Apollo's favour flows more freely today."
        },
        new FestivalData {
            type         = FestivalType.Panathenaia,
            displayName  = "Panathenaia",
            season       = TimeManager.Season.Summer,
            dayOfSeason  = 10,
            benefit      = FestivalBenefit.DoubleFavour,
            relevantGod  = GameState.PatronGod.Athena,
            passiveHonour = 0,
            description  = "The great festival of Athena. Athens celebrates its patron goddess."
        },
        new FestivalData {
            type         = FestivalType.Hephaestia,
            displayName  = "Hephaestia",
            season       = TimeManager.Season.Summer,
            dayOfSeason  = 22,
            benefit      = FestivalBenefit.DoubleFavour,
            relevantGod  = GameState.PatronGod.Hephaestus,
            passiveHonour = 0,
            description  = "The craftsmen's festival. Hephaestus smiles on those who work with their hands."
        },
        new FestivalData {
            type         = FestivalType.Thesmophoria,
            displayName  = "Thesmophoria",
            season       = TimeManager.Season.Autumn,
            dayOfSeason  = 10,
            benefit      = FestivalBenefit.PassiveHonour,
            passiveHonour = 5,
            description  = "A sacred women's festival. You observe with respect and gain honour."
        },
        new FestivalData {
            type         = FestivalType.Pyanopsia,
            displayName  = "Pyanopsia",
            season       = TimeManager.Season.Autumn,
            dayOfSeason  = 22,
            benefit      = FestivalBenefit.DoubleFavour,
            relevantGod  = GameState.PatronGod.Apollo,
            passiveHonour = 0,
            description  = "Apollo's harvest festival. A quiet, reflective celebration."
        },
        new FestivalData {
            type         = FestivalType.Lenaia,
            displayName  = "Lenaia",
            season       = TimeManager.Season.Winter,
            dayOfSeason  = 10,
            benefit      = FestivalBenefit.DoubleRelationship,
            passiveHonour = 0,
            description  = "An intimate winter theatre festival. The gossip flows as freely as the wine."
        },
        new FestivalData {
            type         = FestivalType.Haloa,
            displayName  = "Haloa",
            season       = TimeManager.Season.Winter,
            dayOfSeason  = 22,
            benefit      = FestivalBenefit.DoubleRelationshipAndHonour,
            passiveHonour = 3,
            description  = "Midwinter feasting and unusual social freedoms. A night to remember."
        },
    };

    // ── Internal ───────────────────────────────────────────────────────────
    private FestivalData currentFestival;
    private bool         festivalActive    = false;
    private bool         bonusAppliedToday = false;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);
    }

    private void Start ()
    {
        CheckFestivalForCurrentDay ();
    }

    // ══════════════════════════════════════════════════════════════════════
    // Public API
    // ══════════════════════════════════════════════════════════════════════

    public bool IsFestivalDay => festivalActive;
    public FestivalData CurrentFestival => currentFestival;

    public bool IsDoubleFavourDay (GameState.PatronGod god)
    {
        return festivalActive
            && currentFestival.benefit == FestivalBenefit.DoubleFavour
            && currentFestival.relevantGod == god;
    }

    public bool IsDoubleRelationshipDay ()
    {
        return festivalActive
            && (currentFestival.benefit == FestivalBenefit.DoubleRelationship
             || currentFestival.benefit == FestivalBenefit.DoubleRelationshipAndHonour);
    }

    // Returns the festival on the next day, or None
    public FestivalData GetTomorrowsFestival ()
    {
        if (TimeManager.Instance == null) return new FestivalData { type = FestivalType.None };

        int   tomorrowDay    = TimeManager.Instance.GetCurrentDay () + 1;
        TimeManager.Season season = TimeManager.Instance.GetCurrentSeason ();

        // Work out what day of the season tomorrow is
        // Days cycle 1–28 per season
        int dayOfSeason = ((tomorrowDay - 1) % 28) + 1;

        // Season might tick over
        TimeManager.Season tomorrowSeason = (TimeManager.Season)(((tomorrowDay - 1) / 28) % 4);

        foreach (FestivalData f in festivals)
            if (f.season == tomorrowSeason && f.dayOfSeason == dayOfSeason)
                return f;

        return new FestivalData { type = FestivalType.None };
    }

    // Called by TimeManager at start of each day
    public void CheckFestivalForCurrentDay ()
    {
        festivalActive    = false;
        bonusAppliedToday = false;

        if (TimeManager.Instance == null) return;

        int currentDay    = TimeManager.Instance.GetCurrentDay ();
        int dayOfSeason   = ((currentDay - 1) % 28) + 1;
        TimeManager.Season season = TimeManager.Instance.GetCurrentSeason ();

        foreach (FestivalData f in festivals)
        {
            if (f.season == season && f.dayOfSeason == dayOfSeason)
            {
                currentFestival = f;
                festivalActive  = true;
                ApplyDayStartBonus ();
                Debug.Log ($"Festival day: {f.displayName}");
                return;
            }
        }
    }

    private void ApplyDayStartBonus ()
    {
        if (bonusAppliedToday) return;
        if (GameState.Instance == null) return;

        if (currentFestival.benefit == FestivalBenefit.PassiveHonour
         || currentFestival.benefit == FestivalBenefit.DoubleRelationshipAndHonour)
        {
            GameState.Instance.AddHonour (currentFestival.passiveHonour);
            Debug.Log ($"{currentFestival.displayName}: +{currentFestival.passiveHonour} honour granted.");
        }

        bonusAppliedToday = true;
    }

    // Returns all festivals in calendar order for the UI
    public FestivalData[] GetAllFestivals () => festivals;
}