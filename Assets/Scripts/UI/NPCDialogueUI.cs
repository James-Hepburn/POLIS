using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class NPCDialogueUI : MonoBehaviour
{
    public static NPCDialogueUI Instance { get; private set; }

    [Header ("UI References")]
    public GameObject      dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI hintText;

    // ── Internal ───────────────────────────────────────────────────────────
    private NPC    currentNPC  = null;
    private bool   isOpen      = false;
    private float  openTime    = -999f;
    private const float closeDelay = 0.4f;

    public bool IsOpen    => isOpen;
    public NPC  CurrentNPC => currentNPC;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);

        if (dialoguePanel != null) dialoguePanel.SetActive (false);
    }

    private void Update ()
    {
        if (!isOpen) return;
        if ((Time.time - openTime) >= closeDelay && Keyboard.current.xKey.wasPressedThisFrame)
            Close ();
    }

    // ══════════════════════════════════════════════════════════════════════
    public void Open (NPC npc, string line)
    {
        Debug.Log ($"NPCDialogueUI.Open called for {npc.npcName}");
        currentNPC = npc;
        isOpen     = true;
        openTime   = Time.time;

        if (nameText    != null) nameText.text    = npc.npcName;
        if (dialogueText != null) dialogueText.text = line;
        if (hintText    != null) hintText.text    = "[E] Close";
        if (dialoguePanel != null) dialoguePanel.SetActive (true);
        Debug.Log($"DialoguePanel active: {dialoguePanel.activeSelf}");
    }

    public void Close ()
    {
        isOpen     = false;
        currentNPC = null;
        if (dialoguePanel != null) dialoguePanel.SetActive (false);
        // Notify the NPC that dialogue closed
        // NPC polls IsOpen so no callback needed
    }
}