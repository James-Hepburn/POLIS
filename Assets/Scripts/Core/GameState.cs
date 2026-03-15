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
    public int skinToneIndex  = 0;   // Index into skin tone options
    public int hairColorIndex = 0;   // Index into hair colour options
    public int hairStyleIndex = 0;   // Index into hair style options
    public int facialHairIndex= 0;   // 0 = none, 1 = stubble, 2 = beard
    public int buildIndex     = 0;   // 0 = lean, 1 = average, 2 = broad

    // ── Economy ────────────────────────────────────────────────────────────
    [Header ("Economy")]
    public float drachma  = 50f;     // Starting drachma
    public int   honour   = 10;      // Starting honour (Timē)
    public int   houseLevel = 1;     // 1 = basic insulae

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
    public int careerLevel = 1;      // 1 = entry, 2 = mid, 3 = pinnacle
    public int careerXP    = 0;

    // ── Game Flags ─────────────────────────────────────────────────────────
    [Header ("Game Flags")]
    public bool isNewGame  = true;
    public bool gameStarted = false;

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
    // Setup — called by character creation when player hits Start
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

        // Starting drachma and honour same for everyone
        drachma    = 50f;
        honour     = 10;
        houseLevel = 1;
        careerLevel = 1;
        careerXP   = 0;

        // Set patron god favour to +20, others to 0
        ResetDivineFavour ();
        SetPatronFavour (god, 20);

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
    // Career helpers
    // ══════════════════════════════════════════════════════════════════════

    public void AddCareerXP (int amount)
    {
        careerXP += amount;
        if (careerXP >= 100 && careerLevel < 3)
        {
            careerLevel++;
            careerXP = 0;
            Debug.Log ($"Career level up! Now level {careerLevel}");
        }
    }
}