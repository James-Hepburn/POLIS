using UnityEngine;

/// <summary>
/// Manages Athenian politics — seasonal elections, civic roles, and daily bonuses.
/// Called from TimeManager at season start and day start.
/// </summary>
public class PoliticsManager : MonoBehaviour
{
    public static PoliticsManager Instance { get; private set; }

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Called at the start of each season
    // ══════════════════════════════════════════════════════════════════════

    public void OnSeasonStart ()
    {
        if (GameState.Instance == null) return;

        // Reset the run flag for the new season
        GameState.Instance.hasRunThisSeason = false;

        // Check if civic role has expired
        if (GameState.Instance.civicRole > 0)
        {
            int currentDay = TimeManager.Instance != null ? TimeManager.Instance.GetCurrentDay () : 1;
            if (currentDay >= GameState.Instance.civicRoleExpireDay)
            {
                GameState.Instance.pendingEndOfDayEvents.Add (
                    $"Your term as {GetRoleName (GameState.Instance.civicRole)} has ended. Athens thanks you for your service.");
                GameState.Instance.civicRole = 0;
                GameState.Instance.civicRoleExpireDay = 0;
            }
        }

        // Prompt election if eligible
        if (GameState.Instance.honour >= 60
            && !GameState.Instance.hasRunThisSeason
            && ElectionUI.Instance != null)
        {
            ElectionUI.Instance.ShowElectionPrompt ();
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // Called when player chooses to run
    // ══════════════════════════════════════════════════════════════════════

    public void EnterElection ()
    {
        if (GameState.Instance == null) return;
        GameState.Instance.hasRunThisSeason = true;
        GameState.Instance.electionPending  = true;
        Debug.Log ("Player entered the election.");
    }

    // ══════════════════════════════════════════════════════════════════════
    // Called the morning after entering election
    // ══════════════════════════════════════════════════════════════════════

    public void ResolveElection ()
    {
        if (GameState.Instance == null) return;
        GameState.Instance.electionPending = false;

        int score = CalculateElectionScore ();
        int previousRole = GameState.Instance.civicRole;

        Debug.Log ($"Election score: {score}");

        if (score >= 85)
        {
            AwardRole (3, previousRole); // Archon
        }
        else if (score >= 65)
        {
            AwardRole (2, previousRole); // Magistrate
        }
        else if (score >= 40)
        {
            AwardRole (1, previousRole); // Councillor
        }
        else
        {
            // Lost the election
            GameState.Instance.AddHonour (-3);
            ElectionUI.Instance?.ShowElectionResult (false, 0, score);
            GameState.Instance.pendingEndOfDayEvents.Add (
                "You were not elected. The city was not ready to recognise you. (-3 honour)");
        }
    }

    private void AwardRole (int role, int previousRole)
    {
        if (GameState.Instance == null) return;

        GameState.Instance.civicRole = role;
        GameState.Instance.civicRoleExpireDay = (TimeManager.Instance != null
            ? TimeManager.Instance.GetCurrentDay () : 1) + 28;

        // Bonus honour if promoted
        if (role > previousRole && previousRole > 0)
            GameState.Instance.AddHonour (5);

        ElectionUI.Instance?.ShowElectionResult (true, role, CalculateElectionScore ());

        GameState.Instance.pendingEndOfDayEvents.Add (
            $"You have been elected {GetRoleName (role)}. Athens has spoken. Your term lasts one season.");
    }

    // ══════════════════════════════════════════════════════════════════════
    // Called at end of each day if player holds a civic role
    // ══════════════════════════════════════════════════════════════════════

    public void ApplyCivicBonus ()
    {
        if (GameState.Instance == null) return;
        if (GameState.Instance.civicRole == 0) return;

        switch (GameState.Instance.civicRole)
        {
            case 1: // Councillor
                GameState.Instance.AddHonour (5);
                GameState.Instance.AddDrachma (8f);
                GameState.Instance.pendingEndOfDayEvents.Add (
                    "As Councillor, you receive your daily stipend. (+5 honour, +8 drachma)");
                break;

            case 2: // Magistrate
                GameState.Instance.AddHonour (8);
                GameState.Instance.AddDrachma (15f);
                GameState.Instance.pendingEndOfDayEvents.Add (
                    "As Magistrate, you receive your daily stipend. (+8 honour, +15 drachma)");
                break;

            case 3: // Archon
                GameState.Instance.AddHonour (12);
                GameState.Instance.AddDrachma (25f);
                GameState.Instance.pendingEndOfDayEvents.Add (
                    "As Archon, you receive your daily stipend. (+12 honour, +25 drachma)");
                break;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // Election score calculation
    // ══════════════════════════════════════════════════════════════════════

    private int CalculateElectionScore ()
    {
        if (GameState.Instance == null) return 0;

        // Honour component (40%) — normalised to 0-100
        float honourScore = Mathf.Clamp01 (GameState.Instance.honour / 100f) * 100f * 0.4f;

        // Relationship component (30%) — average of all 12 NPCs, normalised
        float relTotal =
            GameState.Instance.relationshipNikias    +
            GameState.Instance.relationshipDemetrios +
            GameState.Instance.relationshipTheron    +
            GameState.Instance.relationshipArgos     +
            GameState.Instance.relationshipEudoros   +
            GameState.Instance.relationshipChloe     +
            GameState.Instance.relationshipKallias   +
            GameState.Instance.relationshipLydia     +
            GameState.Instance.relationshipMiriam    +
            GameState.Instance.relationshipPhaedra   +
            GameState.Instance.relationshipStephanos +
            GameState.Instance.relationshipXanthos;
        float relAverage   = relTotal / 12f;
        float relScore     = Mathf.Clamp01 ((relAverage + 100f) / 200f) * 100f * 0.3f;

        // Divine favour component (30%) — average of all 6 gods, normalised
        float favourTotal =
            GameState.Instance.favourHermes     +
            GameState.Instance.favourAres       +
            GameState.Instance.favourAphrodite  +
            GameState.Instance.favourApollo     +
            GameState.Instance.favourHephaestus +
            GameState.Instance.favourAthena;
        float favourAverage = favourTotal / 6f;
        float favourScore   = Mathf.Clamp01 ((favourAverage + 100f) / 200f) * 100f * 0.3f;

        return Mathf.RoundToInt (honourScore + relScore + favourScore);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Helpers
    // ══════════════════════════════════════════════════════════════════════

    public static string GetRoleName (int role)
    {
        switch (role)
        {
            case 1: return "Councillor";
            case 2: return "Magistrate";
            case 3: return "Archon";
            default: return "Citizen";
        }
    }

    public bool HasCivicRole => GameState.Instance != null && GameState.Instance.civicRole > 0;
}