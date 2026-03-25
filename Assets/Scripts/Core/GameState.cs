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
    public string lastGossipMessage        = "";

    // ── Divine Favour Tracking ─────────────────────────────────────────────
    [Header ("Divine Favour Tracking")]
    public bool prayedToPatronToday = false;

    // Messages generated at end of day for the summary screen to display
    [System.NonSerialized]
    public System.Collections.Generic.List<string> pendingEndOfDayEvents =
        new System.Collections.Generic.List<string> ();

    // ── Romance & Marriage ─────────────────────────────────────────────────
    public enum RomanceStage { None, Friendship, Courtship, Betrothed, Married }

    [Header ("Romance")]
    public RomanceStage romanceStage  = RomanceStage.None;
    public string       romanceTarget = ""; // "Lydia" or "Chloe"

    public bool IsRomanceCandidate (string npcName)
    {
        if (romanceStage == RomanceStage.Married) return false;
        if (!string.IsNullOrEmpty (romanceTarget) && romanceTarget != npcName) return false;
        return npcName == "Lydia" || npcName == "Chloe";
    }

    public string GetFatherOf (string candidate)
    {
        return candidate == "Lydia" ? "Nikias" : candidate == "Chloe" ? "Argos" : "";
    }

    public bool CanProposeCourtship (string candidate)
    {
        if (!IsRomanceCandidate (candidate)) return false;
        if (romanceStage >= RomanceStage.Courtship) return false;
        return GetRelationship (candidate) >= 40;
    }

    public bool CanProposeBetrothal (string candidate)
    {
        if (romanceTarget != candidate) return false;
        if (romanceStage != RomanceStage.Courtship) return false;
        string father = GetFatherOf (candidate);
        return GetRelationship (candidate) >= 60 && GetRelationship (father) >= 30;
    }

    public bool CanMarry (string candidate)
    {
        if (romanceTarget != candidate) return false;
        if (romanceStage != RomanceStage.Betrothed) return false;
        return drachma >= 100f;
    }
    [Header ("Divine Interventions")]
    public bool interventionHermes     = false;
    public bool interventionAres       = false;
    public bool interventionAphrodite  = false;
    public bool interventionApollo     = false;
    public bool interventionHephaestus = false;
    public bool interventionAthena     = false;

    public bool HasIntervention (PatronGod god)
    {
        switch (god)
        {
            case PatronGod.Hermes:     return interventionHermes;
            case PatronGod.Ares:       return interventionAres;
            case PatronGod.Aphrodite:  return interventionAphrodite;
            case PatronGod.Apollo:     return interventionApollo;
            case PatronGod.Hephaestus: return interventionHephaestus;
            case PatronGod.Athena:     return interventionAthena;
            default:                   return false;
        }
    }

    public void SetIntervention (PatronGod god)
    {
        switch (god)
        {
            case PatronGod.Hermes:     interventionHermes     = true; break;
            case PatronGod.Ares:       interventionAres       = true; break;
            case PatronGod.Aphrodite:  interventionAphrodite  = true; break;
            case PatronGod.Apollo:     interventionApollo     = true; break;
            case PatronGod.Hephaestus: interventionHephaestus = true; break;
            case PatronGod.Athena:     interventionAthena     = true; break;
        }
    }

    /// <summary>
    /// Checks all gods and returns the first one that has crossed +50
    /// favour without having fired an intervention yet. Returns null if none.
    /// </summary>
    public PatronGod? CheckPendingIntervention ()
    {
        PatronGod[] gods = {
            PatronGod.Hermes, PatronGod.Ares, PatronGod.Aphrodite,
            PatronGod.Apollo, PatronGod.Hephaestus, PatronGod.Athena
        };
        foreach (PatronGod god in gods)
        {
            if (!HasIntervention (god) && GetFavour (god) >= 50)
                return god;
        }
        return null;
    }

    // ── God Trials ─────────────────────────────────────────────────────────
    [Header ("God Trials")]
    public bool trialHermes     = false;
    public bool trialAres       = false;
    public bool trialAphrodite  = false;
    public bool trialApollo     = false;
    public bool trialHephaestus = false;
    public bool trialAthena     = false;

    public bool HasTrialFired (PatronGod god)
    {
        switch (god)
        {
            case PatronGod.Hermes:     return trialHermes;
            case PatronGod.Ares:       return trialAres;
            case PatronGod.Aphrodite:  return trialAphrodite;
            case PatronGod.Apollo:     return trialApollo;
            case PatronGod.Hephaestus: return trialHephaestus;
            case PatronGod.Athena:     return trialAthena;
            default:                   return false;
        }
    }

    public void SetTrialFired (PatronGod god)
    {
        switch (god)
        {
            case PatronGod.Hermes:     trialHermes     = true; break;
            case PatronGod.Ares:       trialAres       = true; break;
            case PatronGod.Aphrodite:  trialAphrodite  = true; break;
            case PatronGod.Apollo:     trialApollo     = true; break;
            case PatronGod.Hephaestus: trialHephaestus = true; break;
            case PatronGod.Athena:     trialAthena     = true; break;
        }
    }

    [Header ("Life Goals")]
    public bool goalCareerComplete      = false;
    public bool goalMarriageComplete    = false;
    public bool goalWealthComplete      = false;
    public bool goalFavourComplete      = false;
    public bool goalHonourComplete      = false;
    public bool goalFriendshipComplete  = false;

    // How many goals have been completed
    public int GoalsCompleted =>
        (goalCareerComplete     ? 1 : 0) +
        (goalMarriageComplete   ? 1 : 0) +
        (goalWealthComplete     ? 1 : 0) +
        (goalFavourComplete     ? 1 : 0) +
        (goalHonourComplete     ? 1 : 0) +
        (goalFriendshipComplete ? 1 : 0);
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

    // ── Active Quest ───────────────────────────────────────────────────────
    [Header ("Active Quest")]
    public bool   hasActiveQuest          = false;
    public int    activeQuestType         = 0;
    public string activeQuestGiver        = "";
    public string activeQuestTarget       = "";
    public int    activeQuestTargetAmount = 0;
    public int    activeQuestDeadlineDays = 0;
    public int    activeQuestProgress     = 0;
    public bool   activeQuestComplete     = false;
    public string activeQuestDescription  = "";
    public string activeQuestReward       = "";
    public int    activeQuestStartDay     = 0;

    // ── Collectibles ───────────────────────────────────────────────────────
    [Header ("Collectibles")]
    public bool[] collectiblesFound = new bool[20];

    // ── NPC Story Beats ────────────────────────────────────────────────────
    [Header ("Story Beats")]
    public bool[] storyBeatFired  = new bool[18];
    public int[]  storyBeatChoice = new int[18];

    // Outcome flags
    public bool nikiasToldAboutDebt    = false;
    public bool nikiasBetrayedByPlayer = false;
    public bool lydiaAmbitionSupported = false;
    public bool chloeFeltSeen          = false;
    public bool argosRespected         = false;
    public bool eudorosSharedPain      = false;
    public bool phaedraFaithRestored   = false;

    // ── Friendship Events ─────────────────────────────────────────────────
    [Header ("Friendship Events")]
    public bool friendshipNikias    = false;
    public bool friendshipLydia     = false;
    public bool friendshipChloe     = false;
    public bool friendshipArgos     = false;
    public bool friendshipEudoros   = false;
    public bool friendshipPhaedra   = false;
    public bool friendshipDemetrios = false;
    public bool friendshipTheron    = false;
    public bool friendshipKallias   = false;
    public bool friendshipMiriam    = false;
    public bool friendshipStephanos = false;
    public bool friendshipXanthos   = false;

    public bool HasFriendshipEventFired (string npcName)
    {
        switch (npcName)
        {
            case "Nikias":    return friendshipNikias;
            case "Lydia":     return friendshipLydia;
            case "Chloe":     return friendshipChloe;
            case "Argos":     return friendshipArgos;
            case "Eudoros":   return friendshipEudoros;
            case "Phaedra":   return friendshipPhaedra;
            case "Demetrios": return friendshipDemetrios;
            case "Theron":    return friendshipTheron;
            case "Kallias":   return friendshipKallias;
            case "Miriam":    return friendshipMiriam;
            case "Stephanos": return friendshipStephanos;
            case "Xanthos":   return friendshipXanthos;
            default:          return true;
        }
    }

    public void SetFriendshipEventFired (string npcName)
    {
        switch (npcName)
        {
            case "Nikias":    friendshipNikias    = true; break;
            case "Lydia":     friendshipLydia     = true; break;
            case "Chloe":     friendshipChloe     = true; break;
            case "Argos":     friendshipArgos     = true; break;
            case "Eudoros":   friendshipEudoros   = true; break;
            case "Phaedra":   friendshipPhaedra   = true; break;
            case "Demetrios": friendshipDemetrios = true; break;
            case "Theron":    friendshipTheron    = true; break;
            case "Kallias":   friendshipKallias   = true; break;
            case "Miriam":    friendshipMiriam    = true; break;
            case "Stephanos": friendshipStephanos = true; break;
            case "Xanthos":   friendshipXanthos   = true; break;
        }
    }

    public bool HasStoryBeatFired (int beatIndex) =>
        beatIndex >= 0 && beatIndex < storyBeatFired.Length && storyBeatFired[beatIndex];

    public void FireStoryBeat (int beatIndex, int choice)
    {
        if (beatIndex < 0 || beatIndex >= storyBeatFired.Length) return;
        storyBeatFired[beatIndex]  = true;
        storyBeatChoice[beatIndex] = choice;
    }

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

        prayedToPatronToday    = false;
        pendingEndOfDayEvents.Clear ();
        trialHermes     = false;
        trialAres       = false;
        trialAphrodite  = false;
        trialApollo     = false;
        trialHephaestus = false;
        trialAthena     = false;
        collectiblesFound      = new bool[20];
        storyBeatFired         = new bool[18];
        storyBeatChoice        = new int[18];
        nikiasToldAboutDebt    = false;
        nikiasBetrayedByPlayer = false;
        lydiaAmbitionSupported = false;
        chloeFeltSeen          = false;
        argosRespected         = false;
        eudorosSharedPain      = false;
        phaedraFaithRestored   = false;
        hasActiveQuest         = false;
        activeQuestComplete    = false;
        activeQuestProgress    = 0;
        friendshipNikias = friendshipLydia = friendshipChloe = friendshipArgos =
        friendshipEudoros = friendshipPhaedra = friendshipDemetrios = friendshipTheron =
        friendshipKallias = friendshipMiriam = friendshipStephanos = friendshipXanthos = false;

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
        if (amount > 0)
            AudioManager.Instance?.PlayHonourGained ();
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
        if (amount > 0)
            AudioManager.Instance?.PlayDivineFavourIncrease ();
        GodTrialManager.Instance?.CheckForTrial (god, GetFavour (god));
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
    // Life Goals
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Returns the career goal label for the current profession.
    /// </summary>
    public string GetCareerGoalName ()
    {
        switch (currentProfession)
        {
            case Profession.Merchant:    return "Become a Ship Owner";
            case Profession.Soldier:     return "Achieve the rank of Strategos";
            case Profession.Philosopher: return "Found your own school";
            case Profession.Craftsman:   return "Establish your own workshop";
            case Profession.Priest:      return "Rise to High Priest";
            default:                     return "Reach the pinnacle of your craft";
        }
    }

    /// <summary>
    /// Evaluates all life goals and updates completion flags.
    /// Called at end of day.
    /// </summary>
    public void EvaluateLifeGoals ()
    {
        if (!goalCareerComplete && careerLevel >= 3)
            goalCareerComplete = true;

        if (!goalWealthComplete && drachma >= 500f)
            goalWealthComplete = true;

        if (!goalFavourComplete && GetFavour (patronGod) >= 80)
            goalFavourComplete = true;

        if (!goalHonourComplete && honour >= 80)
            goalHonourComplete = true;

        if (!goalFriendshipComplete)
        {
            int[] relationships = {
                relationshipNikias,   relationshipDemetrios, relationshipTheron,
                relationshipArgos,    relationshipEudoros,   relationshipChloe,
                relationshipKallias,  relationshipLydia,     relationshipMiriam,
                relationshipPhaedra,  relationshipStephanos, relationshipXanthos
            };
            int count = 0;
            foreach (int r in relationships)
                if (r >= 60) count++;
            if (count >= 5)
                goalFriendshipComplete = true;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // Legacy System
    // ══════════════════════════════════════════════════════════════════════

    public int ScoreProsperity ()
    {
        int score = 0;
        if (careerLevel >= 3)  score += 40;
        if (drachma >= 500f)   score += 30;
        else if (drachma >= 200f) score += 15;
        else if (drachma >= 100f) score += 10;
        if (romanceStage == RomanceStage.Married) score += 20;
        return Mathf.Min (score, 100);
    }

    public int ScoreBonds ()
    {
        int score = 0;
        if (romanceStage == RomanceStage.Married) score += 30;
        int[] rels = {
            relationshipNikias,   relationshipDemetrios, relationshipTheron,
            relationshipArgos,    relationshipEudoros,   relationshipChloe,
            relationshipKallias,  relationshipLydia,     relationshipMiriam,
            relationshipPhaedra,  relationshipStephanos, relationshipXanthos
        };
        int closeCount = 0;
        foreach (int r in rels)
            if (r >= 80) closeCount++;
        score += Mathf.Min (closeCount * 10, 50);
        if (GoalsCompleted >= 3) score += 20;
        return Mathf.Min (score, 100);
    }

    public int ScorePiety ()
    {
        int score = 0;
        if (GetFavour (patronGod) >= 80) score += 40;
        else if (GetFavour (patronGod) >= 50) score += 20;
        int interventions = (interventionHermes     ? 1 : 0)
                          + (interventionAres        ? 1 : 0)
                          + (interventionAphrodite   ? 1 : 0)
                          + (interventionApollo      ? 1 : 0)
                          + (interventionHephaestus  ? 1 : 0)
                          + (interventionAthena      ? 1 : 0);
        score += Mathf.Min (interventions * 10, 60);
        return Mathf.Min (score, 100);
    }

    public int ScoreRenown ()
    {
        int score = 0;
        if (honour >= 80)      score += 40;
        else if (honour >= 50) score += 20;
        if (careerLevel >= 3)  score += 20;
        if (GoalsCompleted >= 6) score += 20;
        else if (GoalsCompleted >= 3) score += 10;
        return Mathf.Min (score, 100);
    }

    public int ScoreTotal () =>
        ScoreProsperity () + ScoreBonds () + ScorePiety () + ScoreRenown ();

    public string GetEpitaph ()
    {
        int total = ScoreTotal ();
        string name = playerName;
        if (total >= 400)
            return $"Here lies {name}. Athens will speak his name for a generation.";
        if (total >= 350)
            return $"Here lies {name}. His life was a credit to his city and his gods.";
        if (total >= 300)
            return $"Here lies {name}. He built something worth remembering.";
        if (total >= 200)
            return $"Here lies {name}. Athens remembers his name, if not always why.";
        if (total >= 100)
            return $"Here lies {name}. He walked the streets of Athens and left them unchanged.";
        return $"Here lies {name}. He lived and was forgotten.";
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
        public string lastGossipMessage;

        // Time
        public float currentHour;
        public int   currentDay;
        public int   currentYear;
        public int   currentSeason;

        // Romance
        public int    romanceStage;
        public string romanceTarget;

        // Divine Interventions
        public bool interventionHermes;
        public bool interventionAres;
        public bool interventionAphrodite;
        public bool interventionApollo;
        public bool interventionHephaestus;
        public bool interventionAthena;

        // Life Goals
        public bool goalCareerComplete;
        public bool goalMarriageComplete;
        public bool goalWealthComplete;
        public bool goalFavourComplete;
        public bool goalHonourComplete;
        public bool goalFriendshipComplete;

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

        // God trials
        public bool trialHermes;
        public bool trialAres;
        public bool trialAphrodite;
        public bool trialApollo;
        public bool trialHephaestus;
        public bool trialAthena;

        // Friendship events
        public bool friendshipNikias;
        public bool friendshipLydia;
        public bool friendshipChloe;
        public bool friendshipArgos;
        public bool friendshipEudoros;
        public bool friendshipPhaedra;
        public bool friendshipDemetrios;
        public bool friendshipTheron;
        public bool friendshipKallias;
        public bool friendshipMiriam;
        public bool friendshipStephanos;
        public bool friendshipXanthos;

        // Collectibles
        public bool[] collectiblesFound;

        // Story beats
        public bool[] storyBeatFired;
        public int[]  storyBeatChoice;
        public bool   nikiasToldAboutDebt;
        public bool   nikiasBetrayedByPlayer;
        public bool   lydiaAmbitionSupported;
        public bool   chloeFeltSeen;
        public bool   argosRespected;
        public bool   eudorosSharedPain;
        public bool   phaedraFaithRestored;

        // Active quest
        public bool   hasActiveQuest;
        public int    activeQuestType;
        public string activeQuestGiver;
        public string activeQuestTarget;
        public int    activeQuestTargetAmount;
        public int    activeQuestDeadlineDays;
        public int    activeQuestProgress;
        public bool   activeQuestComplete;
        public string activeQuestDescription;
        public string activeQuestReward;
        public int    activeQuestStartDay;
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
            lastGossipMessage           = lastGossipMessage,
            romanceStage                = (int) romanceStage,
            romanceTarget               = romanceTarget,
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
            goalCareerComplete     = goalCareerComplete,
            goalMarriageComplete   = goalMarriageComplete,
            goalWealthComplete     = goalWealthComplete,
            goalFavourComplete     = goalFavourComplete,
            goalHonourComplete     = goalHonourComplete,
            goalFriendshipComplete = goalFriendshipComplete,
            interventionHermes     = interventionHermes,
            interventionAres       = interventionAres,
            interventionAphrodite  = interventionAphrodite,
            interventionApollo     = interventionApollo,
            interventionHephaestus = interventionHephaestus,
            interventionAthena     = interventionAthena,
            trialHermes            = trialHermes,
            trialAres              = trialAres,
            trialAphrodite         = trialAphrodite,
            trialApollo            = trialApollo,
            trialHephaestus        = trialHephaestus,
            trialAthena            = trialAthena,
            collectiblesFound      = collectiblesFound,
            storyBeatFired         = storyBeatFired,
            storyBeatChoice        = storyBeatChoice,
            nikiasToldAboutDebt    = nikiasToldAboutDebt,
            nikiasBetrayedByPlayer = nikiasBetrayedByPlayer,
            lydiaAmbitionSupported = lydiaAmbitionSupported,
            chloeFeltSeen          = chloeFeltSeen,
            argosRespected         = argosRespected,
            eudorosSharedPain      = eudorosSharedPain,
            phaedraFaithRestored   = phaedraFaithRestored,
            hasActiveQuest         = hasActiveQuest,
            activeQuestType        = activeQuestType,
            activeQuestGiver       = activeQuestGiver,
            activeQuestTarget      = activeQuestTarget,
            activeQuestTargetAmount = activeQuestTargetAmount,
            activeQuestDeadlineDays = activeQuestDeadlineDays,
            activeQuestProgress    = activeQuestProgress,
            activeQuestComplete    = activeQuestComplete,
            activeQuestDescription = activeQuestDescription,
            activeQuestReward      = activeQuestReward,
            activeQuestStartDay    = activeQuestStartDay,
            friendshipNikias    = friendshipNikias,
            friendshipLydia     = friendshipLydia,
            friendshipChloe     = friendshipChloe,
            friendshipArgos     = friendshipArgos,
            friendshipEudoros   = friendshipEudoros,
            friendshipPhaedra   = friendshipPhaedra,
            friendshipDemetrios = friendshipDemetrios,
            friendshipTheron    = friendshipTheron,
            friendshipKallias   = friendshipKallias,
            friendshipMiriam    = friendshipMiriam,
            friendshipStephanos = friendshipStephanos,
            friendshipXanthos   = friendshipXanthos,
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
        lastGossipMessage           = data.lastGossipMessage;
        romanceStage                = (RomanceStage) data.romanceStage;
        romanceTarget               = data.romanceTarget;
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
        goalCareerComplete     = data.goalCareerComplete;
        goalMarriageComplete   = data.goalMarriageComplete;
        goalWealthComplete     = data.goalWealthComplete;
        goalFavourComplete     = data.goalFavourComplete;
        goalHonourComplete     = data.goalHonourComplete;
        goalFriendshipComplete = data.goalFriendshipComplete;
        interventionHermes     = data.interventionHermes;
        interventionAres       = data.interventionAres;
        interventionAphrodite  = data.interventionAphrodite;
        interventionApollo     = data.interventionApollo;
        interventionHephaestus = data.interventionHephaestus;
        interventionAthena     = data.interventionAthena;
        trialHermes            = data.trialHermes;
        trialAres              = data.trialAres;
        trialAphrodite         = data.trialAphrodite;
        trialApollo            = data.trialApollo;
        trialHephaestus        = data.trialHephaestus;
        trialAthena            = data.trialAthena;
        friendshipNikias    = data.friendshipNikias;
        friendshipLydia     = data.friendshipLydia;
        friendshipChloe     = data.friendshipChloe;
        friendshipArgos     = data.friendshipArgos;
        friendshipEudoros   = data.friendshipEudoros;
        friendshipPhaedra   = data.friendshipPhaedra;
        friendshipDemetrios = data.friendshipDemetrios;
        friendshipTheron    = data.friendshipTheron;
        friendshipKallias   = data.friendshipKallias;
        friendshipMiriam    = data.friendshipMiriam;
        friendshipStephanos = data.friendshipStephanos;
        friendshipXanthos   = data.friendshipXanthos;

        // Restore collectibles
        if (data.collectiblesFound != null && data.collectiblesFound.Length == 20)
            collectiblesFound = data.collectiblesFound;
        else
            collectiblesFound = new bool[20];

        // Restore story beats
        if (data.storyBeatFired != null && data.storyBeatFired.Length == 18)
            storyBeatFired = data.storyBeatFired;
        else
            storyBeatFired = new bool[18];

        if (data.storyBeatChoice != null && data.storyBeatChoice.Length == 18)
            storyBeatChoice = data.storyBeatChoice;
        else
            storyBeatChoice = new int[18];

        nikiasToldAboutDebt    = data.nikiasToldAboutDebt;
        nikiasBetrayedByPlayer = data.nikiasBetrayedByPlayer;
        lydiaAmbitionSupported = data.lydiaAmbitionSupported;
        chloeFeltSeen          = data.chloeFeltSeen;
        argosRespected         = data.argosRespected;
        eudorosSharedPain      = data.eudorosSharedPain;
        phaedraFaithRestored   = data.phaedraFaithRestored;

        // Restore active quest
        hasActiveQuest          = data.hasActiveQuest;
        activeQuestType         = data.activeQuestType;
        activeQuestGiver        = data.activeQuestGiver        ?? "";
        activeQuestTarget       = data.activeQuestTarget       ?? "";
        activeQuestTargetAmount = data.activeQuestTargetAmount;
        activeQuestDeadlineDays = data.activeQuestDeadlineDays;
        activeQuestProgress     = data.activeQuestProgress;
        activeQuestComplete     = data.activeQuestComplete;
        activeQuestDescription  = data.activeQuestDescription  ?? "";
        activeQuestReward       = data.activeQuestReward       ?? "";
        activeQuestStartDay     = data.activeQuestStartDay;

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

            AudioManager.Instance?.PlayCareerLevelUp ();

            // Show notification
            if (CareerNotification.Instance != null)
                CareerNotification.Instance.ShowLevelUp (careerLevel, currentProfession);
        }
    }

    /// <summary>
    /// Returns a multiplier applied to drachma and XP rewards at career level 3.
    /// Each profession has a unique pinnacle bonus reflecting their mastery.
    /// </summary>
    public float GetCareerMultiplier ()
    {
        if (careerLevel < 3) return 1.0f;

        switch (currentProfession)
        {
            case Profession.Merchant:    return 1.5f;  // Ship Owner — trade empire, better margins
            case Profession.Soldier:     return 1.4f;  // Strategos — command authority, honour flows
            case Profession.Philosopher: return 1.45f; // Renowned Sophist — patrons pay handsomely
            case Profession.Craftsman:   return 1.5f;  // Master — finest work commands highest price
            case Profession.Priest:      return 1.3f;  // High Priest — divine channel, offerings flow
            default:                     return 1.0f;
        }
    }

    /// <summary>
    /// At career level 3, soldiers gain bonus honour per work session.
    /// At level 3, priests gain bonus favour per work session.
    /// </summary>
    public int GetCareerHonourBonus ()
    {
        if (careerLevel < 3) return 0;
        return currentProfession == Profession.Soldier ? 2 : 0;
    }

    public int GetCareerFavourBonus ()
    {
        if (careerLevel < 3) return 0;
        return currentProfession == Profession.Priest ? 3 : 0;
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

        if (modifiedAmount > 0)
            AudioManager.Instance?.PlayRelationshipGain ();
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