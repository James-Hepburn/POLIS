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

    [Header ("Settings")]
    public float interactionRadius           = 1.5f;
    public float timeCostMinutes             = 30f;
    public float conversationCooldownSeconds = 10f;

    [Header ("Prompt UI")]
    public GameObject promptUI;   // small world-space "press E" prompt above NPC head

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

        if (canTalk && Keyboard.current.eKey.wasPressedThisFrame)
            OpenDialogue ();
    }

    // ══════════════════════════════════════════════════════════════════════
    private void OpenDialogue ()
    {
        Debug.Log($"NPCDialogueUI.Instance is null: {NPCDialogueUI.Instance == null}");
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
        if (GameState.Instance == null) return dialogueNeutral;
        int rel = GameState.Instance.GetRelationship (npcName);
        if (rel < 0)   return dialogueLow;
        if (rel < 40)  return dialogueNeutral;
        if (rel < 80)  return dialogueFriendly;
        return dialogueClose;
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere (transform.position, interactionRadius);
    }
}