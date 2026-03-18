using UnityEngine;
using UnityEngine.InputSystem;

public class NPC : MonoBehaviour
{
    [Header ("Identity")]
    public string npcName        = "Nikias";
    public int    relationshipGain = 5;

    [Header ("Dialogue")]
    [TextArea] public string dialogueLow    = "";  // below 0
    [TextArea] public string dialogueNeutral = ""; // 0 to 39
    [TextArea] public string dialogueFriendly = ""; // 40 to 79
    [TextArea] public string dialogueClose   = "";  // 80+

    [Header ("Settings")]
    public float interactionRadius  = 1.5f;
    public float timeCostMinutes    = 30f;
    public float conversationCooldownSeconds = 10f;

    [Header ("UI")]
    public GameObject promptUI;
    public GameObject dialogueUI;
    public TMPro.TextMeshProUGUI dialogueText;
    public TMPro.TextMeshProUGUI npcNameText;

    // ── Internal ───────────────────────────────────────────────────────────
    private Transform player;
    private bool      playerInRange   = false;
    private bool      dialogueOpen    = false;
    private float     lastTalkTime    = -999f;
    private bool      talkedToday     = false;
    private int       lastTalkedDay   = -1;
    private float     dialogueOpenTime = -999f;
    private const float closeDelay    = 0.4f;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        player = GameObject.FindWithTag ("Player")?.transform;
        if (promptUI != null)  promptUI.SetActive (false);
        if (dialogueUI != null) dialogueUI.SetActive (false);
    }

    private void Update ()
    {
        if (player == null) return;

        // Reset daily cap on new day
        if (TimeManager.Instance != null)
        {
            int today = TimeManager.Instance.GetCurrentDay ();
            if (today != lastTalkedDay)
            {
                talkedToday  = false;
                lastTalkedDay = today;
            }
        }

        float distance = Vector2.Distance (transform.position, player.position);
        playerInRange  = distance <= interactionRadius;

        bool onCooldown = (Time.time - lastTalkTime) < conversationCooldownSeconds;
        bool canTalk    = playerInRange && !dialogueOpen && !onCooldown && !talkedToday;
        bool canClose   = dialogueOpen && (Time.time - dialogueOpenTime) >= closeDelay;

        // Show/hide prompt
        if (promptUI != null)
            promptUI.SetActive (canTalk);

        // Open dialogue
        if (canTalk && Keyboard.current.eKey.wasPressedThisFrame)
            OpenDialogue ();

        // Close dialogue — only after delay has elapsed
        else if (canClose && Keyboard.current.eKey.wasPressedThisFrame)
            CloseDialogue ();
    }

    // ══════════════════════════════════════════════════════════════════════
    private void OpenDialogue ()
    {
        Debug.Log ("OpenDialogue called");
        dialogueOpen     = true;
        lastTalkTime     = Time.time;
        dialogueOpenTime = Time.time;
        talkedToday      = true;

        if (promptUI != null)  promptUI.SetActive (false);
        if (dialogueUI != null)
        {
            Debug.Log ($"dialogueUI is: {dialogueUI.name}");
            dialogueUI.SetActive (true);
        }
        else
        {
            Debug.Log ("dialogueUI is NULL");
        }

        if (npcNameText != null)
            npcNameText.text = npcName;

        if (dialogueText != null)
            dialogueText.text = GetDialogueLine ();

        // Advance time and relationship
        if (TimeManager.Instance != null)
            TimeManager.Instance.AdvanceTimeByMinutes (timeCostMinutes);

        if (GameState.Instance != null)
        {
            GameState.Instance.ChangeRelationship (npcName, relationshipGain);
            GameState.Instance.AddHonour (1);
        }
    }

    private void CloseDialogue ()
    {
        dialogueOpen = false;
        if (dialogueUI != null) dialogueUI.SetActive (false);
    }

    private string GetDialogueLine ()
    {
        if (GameState.Instance == null) return dialogueNeutral;

        int rel = GameState.Instance.GetRelationship (npcName);

        if (rel < 0)    return dialogueLow;
        if (rel < 40)   return dialogueNeutral;
        if (rel < 80)   return dialogueFriendly;
        return dialogueClose;
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere (transform.position, interactionRadius);
    }
}