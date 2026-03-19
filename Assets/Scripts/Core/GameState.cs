using UnityEngine;

public class GameState : MonoBehaviour
{
    // ── Singleton ──────────────────────────────────────────────────────────
    public static GameState Instance { get; private set; }

    // ── Player Identity ────────────────────────────────────────────────────
    [Header ("Player Identity")]
    public string playerName = "Alexios";

    public enum Profession
    {
        Merchant,
        Soldier,
        Philosopher,
        Craftsman,
        Priest
    }

    public enum PatronGod
    {
        Hermes,
        Ares,
        Aphrodite,
        Apollo,
        Hephaestus,
        Athena
    }

    public Profession currentProfession = Profession.Merchant;
    public PatronGod patronGod         = PatronGod.Hermes;

    // ── Appearance ─────────────────────────────────────────────────────────
    [Header ("Appearance")]
    public int skinToneIndex  = 0;
    public int hairColorIndex = 0;
    public int hairStyleIndex = 0;
    public int facialHairIndex= 0;
    public int buildIndex     = 0;

    // ── Economy ────────────────────────────────────────────────────────────
    [Header ("Economy")]
    public float drachma  = 50f;
    public int   honour   = 10;
    public int   houseLevel = 1;

    // ── Divine Favour ──────────────────────────────────────────────────────
    [Header ("Divine Favour")]
    public int favourHermes     = 0;
    public int favourAres       = 0;
    public int favourAphrodite  = 0;
    public int favourApollo     = 0;
    public int favourHephaestus = 0;
    public int favourAthena     = 0;

    // ── Career Progress ────────────────────────────────────────────────────
    [Header ("Career")]
    public int careerLevel = 1;
    public int careerXP    = 0;

    // ── Game Flags ─────────────────────────────────────────────────────────
    [Header ("Game Flags")]
    public bool isNewGame  = true;
    public bool gameStarted = false;
    public int  lastCompletedDay = 1;
    public string lastCompletedDayFestival = "";

    // ── Divine Favour Tracking ─────────────────────────────────────────────
    [Header ("Divine Favour Tracking")]
    public bool prayedToPatronToday = false;

    // Messages generated at end of day for the summary screen to display
    [System.NonSerialized]
    public System.Collections.Generic.List<string> pendingEndOfDayEvents =
        new System.Collections.Generic.List<string> ();

    // ── Relationships ──────────────────────────────────────────────────────
    [Header ("Relationships")]
    public int relationshipNikias      = 0;
    public int relationshipDemetrios   = 0;
    public int relationshipTheron      = 0;
    public int relationshipArgos       = 0;
    public int relationshipEudoros     = 0;
    public int relationshipChloe       = 0;
    public int relationshipKallias     = 0;
    public int relationshipLydia       = 0;
    public int relationshipMiriam      = 0;
    public int relationshipPhaedra     = 0;
    public int relationshipStephanos   = 0;
    public int relationshipXanthos     = 0;

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
    }

    // ══════════════════════════════════════════════════════════════════════
    // Setup
    // ══════════════════════════════════════════════════════════════════════

    public void InitialiseNewGame (
        Profession profession,
        PatronGod  god,
        int        skinTone,
        int        hairColor,
        int        hairStyle,
        int        facialHair,
        int        build)
    {
        currentProfession = profession;
        patronGod         = god;
        skinToneIndex     = skinTone;
        hairColorIndex    = hairColor;
        hairStyleIndex    = hairStyle;
        facialHairIndex   = facialHair;
        buildIndex        = build;

        drachma    = 50f;
        honour     = 10;
        houseLevel = 1;
        careerLevel = 1;
        careerXP   = 0;

        ResetDivineFavour ();
        SetPatronFavour (god, 20);

        prayedToPatronToday = false;
        pendingEndOfDayEvents.Clear ();

        isNewGame   = false;
        gameStarted = true;

        Debug.Log ($"New game started. Profession: {profession}, Patron: {god}");
    }

    // ══════════════════════════════════════════════════════════════════════
    // Economy helpers
    // ══════════════════════════════════════════════════════════════════════

    public void AddDrachma (float amount)
    {
        drachma += amount;
        drachma  = Mathf.Max (0, drachma);
        Debug.Log ($"Drachma: {drachma}");
    }

    public bool SpendDrachma (float amount)
    {
        if (drachma < amount)
        {
            Debug.Log ("Not enough drachma.");
            return false;
        }
        drachma -= amount;
        return true;
    }

    public void AddHonour (int amount)
    {
        honour = Mathf.Clamp (honour + amount, 0, 100);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Divine favour helpers
    // ══════════════════════════════════════════════════════════════════════

    private void ResetDivineFavour ()
    {
        favourHermes = favourAres = favourAphrodite = 0;
        favourApollo = favourHephaestus = favourAthena = 0;
    }

    private void SetPatronFavour (PatronGod god, int amount)
    {
        switch (god)
        {
            case PatronGod.Hermes:     favourHermes     = amount; break;
            case PatronGod.Ares:       favourAres       = amount; break;
            case PatronGod.Aphrodite:  favourAphrodite  = amount; break;
            case PatronGod.Apollo:     favourApollo     = amount; break;
            case PatronGod.Hephaestus: favourHephaestus = amount; break;
            case PatronGod.Athena:     favourAthena     = amount; break;
        }
    }

    public void ChangeFavour (PatronGod god, int amount)
    {
        switch (god)
        {
            case PatronGod.Hermes:     favourHermes     = Mathf.Clamp (favourHermes     + amount, -100, 100); break;
            case PatronGod.Ares:       favourAres       = Mathf.Clamp (favourAres       + amount, -100, 100); break;
            case PatronGod.Aphrodite:  favourAphrodite  = Mathf.Clamp (favourAphrodite  + amount, -100, 100); break;
            case PatronGod.Apollo:     favourApollo     = Mathf.Clamp (favourApollo     + amount, -100, 100); break;
            case PatronGod.Hephaestus: favourHephaestus = Mathf.Clamp (favourHephaestus + amount, -100, 100); break;
            case PatronGod.Athena:     favourAthena     = Mathf.Clamp (favourAthena     + amount, -100, 100); break;
        }
    }

    public int GetFavour (PatronGod god)
    {
        switch (god)
        {
            case PatronGod.Hermes:     return favourHermes;
            case PatronGod.Ares:       return favourAres;
            case PatronGod.Aphrodite:  return favourAphrodite;
            case PatronGod.Apollo:     return favourApollo;
            case PatronGod.Hephaestus: return favourHephaestus;
            case PatronGod.Athena:     return favourAthena;
            default:                   return 0;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // End-of-day favour processing
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Called by TimeManager.EndDay(). Applies patron decay if the player
    /// did not pray today, fires negative favour events, then resets daily flags.
    /// </summary>
    public void ProcessEndOfDayFavour ()
    {
        pendingEndOfDayEvents.Clear ();

        // ── Patron decay ───────────────────────────────────────────────────
        if (!prayedToPatronToday)
        {
            int decay = 3;
            ChangeFavour (patronGod, -decay);
            int current = GetFavour (patronGod);
            pendingEndOfDayEvents.Add (
                $"{patronGod} grows distant — you did not pray today. (Favour: {current})");
            Debug.Log ($"Patron decay: {patronGod} -{decay}. Now {current}.");
        }

        // ── Negative favour events ─────────────────────────────────────────
        CheckNegativeFavour (PatronGod.Hermes,     favourHermes);
        CheckNegativeFavour (PatronGod.Ares,       favourAres);
        CheckNegativeFavour (PatronGod.Aphrodite,  favourAphrodite);
        CheckNegativeFavour (PatronGod.Apollo,     favourApollo);
        CheckNegativeFavour (PatronGod.Hephaestus, favourHephaestus);
        CheckNegativeFavour (PatronGod.Athena,     favourAthena);

        // ── Reset daily flags ──────────────────────────────────────────────
        prayedToPatronToday = false;
    }

    private void CheckNegativeFavour (PatronGod god, int favour)
    {
        if (favour >= -20) return;

        // Apply a domain-appropriate mechanical penalty
        switch (god)
        {
            case PatronGod.Hermes:
                float lost = 5f;
                AddDrachma (-lost);
                pendingEndOfDayEvents.Add (
                    $"Hermes turns his back — a deal goes poorly. Lost ₯{lost:F0}. (Favour: {favour})");
                break;

            case PatronGod.Ares:
                AddHonour (-3);
                pendingEndOfDayEvents.Add (
                    $"Ares is displeased — your reputation for courage suffers. (-3 Honour) (Favour: {favour})");
                break;

            case PatronGod.Aphrodite:
                // Relationship decay on a random tracked NPC
                string[] npcs = { 
                    "Nikias", "Demetrios", "Theron", "Argos", "Eudoros", "Chloe", 
                    "Kallias", "Lydia", "Miriam", "Phaedra", "Stephanos", "Xanthos" 
                };
                string target = npcs[Random.Range (0, npcs.Length)];
                ChangeRelationship (target, -5);
                pendingEndOfDayEvents.Add (
                    $"Aphrodite is displeased — {target} grows cooler toward you. (-5 Relationship) (Favour: {favour})");
                break;

            case PatronGod.Apollo:
                careerXP = Mathf.Max (0, careerXP - 5);
                pendingEndOfDayEvents.Add (
                    $"Apollo withholds his clarity — your studies feel fruitless. (-5 Career XP) (Favour: {favour})");
                break;

            case PatronGod.Hephaestus:
                float lostCraft = 4f;
                AddDrachma (-lostCraft);
                pendingEndOfDayEvents.Add (
                    $"Hephaestus is displeased — your tools fail you. Lost ₯{lostCraft:F0}. (Favour: {favour})");
                break;

            case PatronGod.Athena:
                AddHonour (-2);
                pendingEndOfDayEvents.Add (
                    $"Athena withdraws her favour — your standing in the city quietly erodes. (-2 Honour) (Favour: {favour})");
                break;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // Save / Load
    // ══════════════════════════════════════════════════════════════════════

    [System.Serializable]
    public class SaveData
    {
        // Identity
        public int profession;
        public int patronGod;

        // Appearance
        public int skinToneIndex;
        public int hairColorIndex;
        public int hairStyleIndex;
        public int facialHairIndex;
        public int buildIndex;

        // Economy
        public float drachma;
        public int   honour;
        public int   houseLevel;

        // Divine Favour
        public int favourHermes;
        public int favourAres;
        public int favourAphrodite;
        public int favourApollo;
        public int favourHephaestus;
        public int favourAthena;

        // Career
        public int careerLevel;
        public int careerXP;

        // Game Flags
        public int    lastCompletedDay;
        public bool   prayedToPatronToday;
        public string lastCompletedDayFestival;

        // Time
        public float currentHour;
        public int   currentDay;
        public int   currentYear;
        public int   currentSeason;

        // Relationships
        public int relationshipNikias;
        public int relationshipDemetrios;
        public int relationshipTheron;
        public int relationshipArgos;
        public int relationshipEudoros;
        public int relationshipChloe;
        public int relationshipKallias;
        public int relationshipLydia;
        public int relationshipMiriam;
        public int relationshipPhaedra;
        public int relationshipStephanos;
        public int relationshipXanthos;
    }

    public void Save ()
    {
        SaveData data = new SaveData
        {
            profession        = (int) currentProfession,
            patronGod         = (int) patronGod,
            skinToneIndex     = skinToneIndex,
            hairColorIndex    = hairColorIndex,
            hairStyleIndex    = hairStyleIndex,
            facialHairIndex   = facialHairIndex,
            buildIndex        = buildIndex,
            drachma           = drachma,
            honour            = honour,
            houseLevel        = houseLevel,
            favourHermes      = favourHermes,
            favourAres        = favourAres,
            favourAphrodite   = favourAphrodite,
            favourApollo      = favourApollo,
            favourHephaestus  = favourHephaestus,
            favourAthena      = favourAthena,
            careerLevel       = careerLevel,
            careerXP          = careerXP,
            lastCompletedDay            = lastCompletedDay,
            prayedToPatronToday         = prayedToPatronToday,
            lastCompletedDayFestival    = lastCompletedDayFestival,
            currentHour       = TimeManager.Instance != null ? TimeManager.Instance.GetCurrentHour () : 6f,
            currentDay        = TimeManager.Instance != null ? TimeManager.Instance.GetCurrentDay ()  : 1,
            currentYear       = TimeManager.Instance != null ? TimeManager.Instance.GetCurrentYear () : 1,
            currentSeason     = TimeManager.Instance != null ? (int) TimeManager.Instance.GetCurrentSeason () : 0,
            relationshipNikias    = relationshipNikias,
            relationshipDemetrios = relationshipDemetrios,
            relationshipTheron    = relationshipTheron,
            relationshipArgos     = relationshipArgos,
            relationshipEudoros   = relationshipEudoros,
            relationshipChloe     = relationshipChloe,
            relationshipKallias   = relationshipKallias,
            relationshipLydia     = relationshipLydia,
            relationshipMiriam    = relationshipMiriam,
            relationshipPhaedra   = relationshipPhaedra,
            relationshipStephanos = relationshipStephanos,
            relationshipXanthos   = relationshipXanthos,
        };

        string json = JsonUtility.ToJson (data, prettyPrint: true);
        System.IO.File.WriteAllText (SavePath, json);
        Debug.Log ($"Game saved to {SavePath}");
    }

    public bool Load ()
    {
        if (!System.IO.File.Exists (SavePath))
        {
            Debug.Log ("No save file found.");
            return false;
        }

        string json   = System.IO.File.ReadAllText (SavePath);
        SaveData data = JsonUtility.FromJson<SaveData> (json);

        currentProfession = (Profession) data.profession;
        patronGod         = (PatronGod)  data.patronGod;
        skinToneIndex     = data.skinToneIndex;
        hairColorIndex    = data.hairColorIndex;
        hairStyleIndex    = data.hairStyleIndex;
        facialHairIndex   = data.facialHairIndex;
        buildIndex        = data.buildIndex;
        drachma           = data.drachma;
        honour            = data.honour;
        houseLevel        = data.houseLevel;
        favourHermes      = data.favourHermes;
        favourAres        = data.favourAres;
        favourAphrodite   = data.favourAphrodite;
        favourApollo      = data.favourApollo;
        favourHephaestus  = data.favourHephaestus;
        favourAthena      = data.favourAthena;
        careerLevel       = data.careerLevel;
        careerXP          = data.careerXP;
        lastCompletedDay            = data.lastCompletedDay;
        prayedToPatronToday         = data.prayedToPatronToday;
        lastCompletedDayFestival    = data.lastCompletedDayFestival;
        relationshipNikias    = data.relationshipNikias;
        relationshipDemetrios = data.relationshipDemetrios;
        relationshipTheron    = data.relationshipTheron;
        relationshipArgos     = data.relationshipArgos;
        relationshipEudoros   = data.relationshipEudoros;
        relationshipChloe     = data.relationshipChloe;
        relationshipKallias   = data.relationshipKallias;
        relationshipLydia     = data.relationshipLydia;
        relationshipMiriam    = data.relationshipMiriam;
        relationshipPhaedra   = data.relationshipPhaedra;
        relationshipStephanos = data.relationshipStephanos;
        relationshipXanthos   = data.relationshipXanthos;

        // Restore time state
        if (TimeManager.Instance != null)
            TimeManager.Instance.LoadTimeState (data.currentHour, data.currentDay, data.currentYear, (TimeManager.Season) data.currentSeason);

        isNewGame   = false;
        gameStarted = true;
        pendingEndOfDayEvents.Clear ();

        Debug.Log ("Game loaded successfully.");
        return true;
    }

    public static bool SaveExists ()
    {
        string path = System.IO.Path.Combine (Application.persistentDataPath, "polis_save.json");
        bool exists = System.IO.File.Exists (path);
        Debug.Log ($"SaveExists check — path: {path}, exists: {exists}");
        return exists;
    }

    public static void DeleteSave ()
    {
        string path = System.IO.Path.Combine (Application.persistentDataPath, "polis_save.json");
        if (System.IO.File.Exists (path))
            System.IO.File.Delete (path);
    }

    private static string SavePath =>
        System.IO.Path.Combine (Application.persistentDataPath, "polis_save.json");


    public void AddCareerXP (int amount)
    {
        careerXP += amount;
        if (careerXP >= 100 && careerLevel < 3)
        {
            careerLevel++;
            careerXP = 0;
            Debug.Log ($"Career level up! Now level {careerLevel}");

            // Honour bonus on level up
            AddHonour (5);

            // Show notification
            if (CareerNotification.Instance != null)
                CareerNotification.Instance.ShowLevelUp (careerLevel, currentProfession);
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // Relationship helpers
    // ══════════════════════════════════════════════════════════════════════

    public void ChangeRelationship (string npcName, int amount)
    {
        // Apply Aphrodite favour modifier to relationship gains
        int bonus = FavourModifiers.GetRelationshipBonus (favourAphrodite);
        int modifiedAmount = amount > 0 ? amount + bonus : amount;

        switch (npcName)
        {
            case "Nikias":      relationshipNikias    = Mathf.Clamp (relationshipNikias    + modifiedAmount, -100, 100); break;
            case "Demetrios":   relationshipDemetrios = Mathf.Clamp (relationshipDemetrios + modifiedAmount, -100, 100); break;
            case "Theron":      relationshipTheron    = Mathf.Clamp (relationshipTheron    + modifiedAmount, -100, 100); break;
            case "Argos":       relationshipArgos     = Mathf.Clamp (relationshipArgos     + modifiedAmount, -100, 100); break;
            case "Eudoros":     relationshipEudoros   = Mathf.Clamp (relationshipEudoros   + modifiedAmount, -100, 100); break;
            case "Chloe":       relationshipChloe     = Mathf.Clamp (relationshipChloe     + modifiedAmount, -100, 100); break;
            case "Kallias":     relationshipKallias   = Mathf.Clamp (relationshipKallias   + modifiedAmount, -100, 100); break;
            case "Lydia":       relationshipLydia     = Mathf.Clamp (relationshipLydia     + modifiedAmount, -100, 100); break;
            case "Miriam":      relationshipMiriam    = Mathf.Clamp (relationshipMiriam    + modifiedAmount, -100, 100); break;
            case "Phaedra":     relationshipPhaedra   = Mathf.Clamp (relationshipPhaedra   + modifiedAmount, -100, 100); break;
            case "Stephanos":   relationshipStephanos = Mathf.Clamp (relationshipStephanos + modifiedAmount, -100, 100); break;
            case "Xanthos":     relationshipXanthos   = Mathf.Clamp (relationshipXanthos   + modifiedAmount, -100, 100); break;
        }
        Debug.Log ($"Relationship with {npcName}: {GetRelationship (npcName)}");
    }

    public int GetRelationship (string npcName)
    {
        switch (npcName)
        {
            case "Nikias":    return relationshipNikias;
            case "Demetrios": return relationshipDemetrios;
            case "Theron":    return relationshipTheron;
            case "Argos":     return relationshipArgos;
            case "Eudoros":   return relationshipEudoros;
            case "Chloe":     return relationshipChloe;
            case "Kallias":   return relationshipKallias;
            case "Lydia":     return relationshipLydia;
            case "Miriam":    return relationshipMiriam;
            case "Phaedra":   return relationshipPhaedra;
            case "Stephanos": return relationshipStephanos;
            case "Xanthos":   return relationshipXanthos;
            default:          return 0;
        }
    }
}