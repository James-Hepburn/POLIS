using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
    [Header ("Identity")]
    public string npcName        = "Nikias";
    public int    relationshipGain = 5;

    [Header ("Dialogue")]
    [TextArea] public string dialogueLow      = "";  // below 0
    [TextArea] public string dialogueNeutral  = "";  // 0 to 39
    [TextArea] public string dialogueFriendly = "";  // 40 to 79
    [TextArea] public string dialogueClose    = "";  // 80+

    [Header ("Festival Dialogue")]
    [TextArea] public string dialogueFestivalGeneric = "";  // any festival, no specific line
    [TextArea] public string dialogueCityDionysia    = "";
    [TextArea] public string dialogueThargelia       = "";
    [TextArea] public string dialoguePanathenaia     = "";
    [TextArea] public string dialogueHephaestia      = "";
    [TextArea] public string dialogueThesmophoria    = "";
    [TextArea] public string dialoguePyanopsia       = "";
    [TextArea] public string dialogueLenaia          = "";
    [TextArea] public string dialogueHaloa           = "";

    [Header ("Settings")]
    public float interactionRadius           = 1.5f;
    public float timeCostMinutes             = 30f;
    public float conversationCooldownSeconds = 10f;

    [Header ("Prompt UI")]
    public GameObject promptUI;
    public GameObject romancePromptUI; // [R] Express Interest / Ask for hand / Marry

    // ── Internal ───────────────────────────────────────────────────────────
    private Transform player;
    private bool      playerInRange    = false;
    private bool      talkedToday      = false;
    private int       lastTalkedDay    = -1;
    private float     lastTalkTime     = -999f;

    public bool IsDialogueOpen =>
        NPCDialogueUI.Instance != null && NPCDialogueUI.Instance.IsOpen
        && NPCDialogueUI.Instance.CurrentNPC == this;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        DontDestroyOnLoad (gameObject);
    }

    private void Start ()
    {
        FindPlayer ();
        if (promptUI != null) promptUI.SetActive (false);
    }

    private void OnEnable ()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable ()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        FindPlayer ();
        if (promptUI != null) promptUI.SetActive (false);
    }

    private void FindPlayer ()
    {
        GameObject p = GameObject.FindWithTag ("Player");
        if (p != null) player = p.transform;
    }

    // ══════════════════════════════════════════════════════════════════════
    private void Update ()
    {
        if (player == null) { FindPlayer (); return; }

        // Reset daily cap on new day
        if (TimeManager.Instance != null)
        {
            int today = TimeManager.Instance.GetCurrentDay ();
            if (today != lastTalkedDay)
            {
                talkedToday   = false;
                lastTalkedDay = today;
            }
        }

        float distance    = Vector2.Distance (transform.position, player.position);
        playerInRange     = distance <= interactionRadius;

        bool onCooldown   = (Time.time - lastTalkTime) < conversationCooldownSeconds;
        bool canTalk      = playerInRange && !IsDialogueOpen && !onCooldown && !talkedToday;

        if (promptUI != null)
            promptUI.SetActive (canTalk);

        // Romance prompt — shown alongside talk prompt when relevant
        bool canRomance = playerInRange && !IsDialogueOpen && ShowRomancePrompt ();
        if (romancePromptUI != null)
            romancePromptUI.SetActive (canRomance);

        if (canTalk && Keyboard.current.eKey.wasPressedThisFrame)
            OpenDialogue ();

        // Romance interaction on R key
        if (canRomance && Keyboard.current.rKey.wasPressedThisFrame)
            OpenRomanceInteraction ();
    }

    // ══════════════════════════════════════════════════════════════════════
    private void OpenDialogue ()
    {
        lastTalkTime = Time.time;
        talkedToday  = true;

        if (NPCDialogueUI.Instance != null)
            NPCDialogueUI.Instance.Open (this, GetDialogueLine ());

        if (promptUI != null) promptUI.SetActive (false);

        if (TimeManager.Instance != null)
            TimeManager.Instance.AdvanceTimeByMinutes (timeCostMinutes);

        if (GameState.Instance != null)
        {
            GameState.Instance.ChangeRelationship (npcName, relationshipGain);
            GameState.Instance.AddHonour (1);
        }
    }

    private string GetDialogueLine ()
    {
        // Festival dialogue takes priority over relationship dialogue
        if (FestivalManager.Instance != null && FestivalManager.Instance.IsFestivalDay)
        {
            string festivalLine = GetFestivalDialogueLine (FestivalManager.Instance.CurrentFestival.type);
            if (!string.IsNullOrEmpty (festivalLine))
                return festivalLine;

            // Fall back to generic festival line
            if (!string.IsNullOrEmpty (dialogueFestivalGeneric))
                return dialogueFestivalGeneric;
        }

        // Normal relationship-tier dialogue
        if (GameState.Instance == null) return dialogueNeutral;
        int rel = GameState.Instance.GetRelationship (npcName);
        if (rel < 0)   return dialogueLow;
        if (rel < 40)  return dialogueNeutral;
        if (rel < 80)  return dialogueFriendly;
        return dialogueClose;
    }

    private string GetFestivalDialogueLine (FestivalManager.FestivalType festival)
    {
        switch (festival)
        {
            case FestivalManager.FestivalType.CityDionysia:   return dialogueCityDionysia;
            case FestivalManager.FestivalType.Thargelia:       return dialogueThargelia;
            case FestivalManager.FestivalType.Panathenaia:     return dialoguePanathenaia;
            case FestivalManager.FestivalType.Hephaestia:      return dialogueHephaestia;
            case FestivalManager.FestivalType.Thesmophoria:    return dialogueThesmophoria;
            case FestivalManager.FestivalType.Pyanopsia:       return dialoguePyanopsia;
            case FestivalManager.FestivalType.Lenaia:          return dialogueLenaia;
            case FestivalManager.FestivalType.Haloa:           return dialogueHaloa;
            default:                                           return "";
        }
    }

    private bool ShowRomancePrompt ()
    {
        if (GameState.Instance == null) return false;

        // Candidate — show if can propose courtship or already courting
        if (npcName == "Lydia" || npcName == "Chloe")
        {
            if (GameState.Instance.CanProposeCourtship (npcName)) return true;
            if (GameState.Instance.romanceTarget == npcName
                && GameState.Instance.romanceStage == GameState.RomanceStage.Betrothed
                && GameState.Instance.CanMarry (npcName)) return true;
        }

        // Father — show if player is courting his daughter
        if (npcName == "Nikias" || npcName == "Argos")
        {
            string daughter = npcName == "Nikias" ? "Lydia" : "Chloe";
            if (GameState.Instance.romanceTarget == daughter
                && GameState.Instance.romanceStage == GameState.RomanceStage.Courtship
                && GameState.Instance.CanProposeBetrothal (daughter)) return true;
        }

        return false;
    }

    private void OpenRomanceInteraction ()
    {
        if (GameState.Instance == null) return;

        // Set cooldown so prompts hide after interaction
        lastTalkTime = Time.time;
        talkedToday  = true;
        if (romancePromptUI != null) romancePromptUI.SetActive (false);
        if (promptUI != null)        promptUI.SetActive (false);

        // Candidate interactions
        if (npcName == "Lydia" || npcName == "Chloe")
        {
            if (GameState.Instance.CanProposeCourtship (npcName))
            {
                GameState.Instance.romanceTarget = npcName;
                GameState.Instance.romanceStage  = GameState.RomanceStage.Courtship;
                TimeManager.Instance?.AdvanceTimeByMinutes (30f);
                string line = $"You express your interest in {npcName}. She listens thoughtfully. \"I would like to know you better,\" she says.";
                NPCDialogueUI.Instance?.Open (this, line);
                return;
            }

            if (GameState.Instance.romanceTarget == npcName
                && GameState.Instance.romanceStage == GameState.RomanceStage.Betrothed
                && GameState.Instance.CanMarry (npcName))
            {
                GameState.Instance.SpendDrachma (100f);
                GameState.Instance.romanceStage = GameState.RomanceStage.Married;
                GameState.Instance.goalMarriageComplete = true;
                GameState.Instance.AddHonour (10);
                TimeManager.Instance?.AdvanceTimeByMinutes (120f);
                string line = $"You and {npcName} are married before the gods and the city of Athens. Whatever comes next, you face it together.";
                NPCDialogueUI.Instance?.Open (this, line);
                return;
            }
        }

        // Father interactions
        if (npcName == "Nikias" || npcName == "Argos")
        {
            string daughter = npcName == "Nikias" ? "Lydia" : "Chloe";
            if (GameState.Instance.romanceTarget == daughter
                && GameState.Instance.romanceStage == GameState.RomanceStage.Courtship
                && GameState.Instance.CanProposeBetrothal (daughter))
            {
                GameState.Instance.romanceStage = GameState.RomanceStage.Betrothed;
                GameState.Instance.ChangeRelationship (npcName, 10);
                TimeManager.Instance?.AdvanceTimeByMinutes (45f);
                string line = $"{npcName} studies you for a long moment. \"I have watched you,\" he says. \"You have conducted yourself well. You have my blessing. The dowry is 100 drachma, payable on the wedding day.\"";
                NPCDialogueUI.Instance?.Open (this, line);
                return;
            }

            // Father refuses — relationship too low
            if (GameState.Instance.romanceTarget == daughter
                && GameState.Instance.romanceStage == GameState.RomanceStage.Courtship)
            {
                GameState.Instance.ChangeRelationship (daughter, -5);
                TimeManager.Instance?.AdvanceTimeByMinutes (30f);
                string line = $"{npcName} shakes his head slowly. \"Not yet. Prove yourself further, and perhaps we will speak of this again.\"";
                NPCDialogueUI.Instance?.Open (this, line);
            }
        }
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere (transform.position, interactionRadius);
    }
}