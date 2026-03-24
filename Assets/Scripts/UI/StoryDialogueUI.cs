using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System;

public class StoryDialogueUI : MonoBehaviour
{
    public static StoryDialogueUI Instance { get; private set; }

    [Header ("UI References")]
    public GameObject      panel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueText;
    public Button          choiceAButton;
    public Button          choiceBButton;
    public TextMeshProUGUI choiceAText;
    public TextMeshProUGUI choiceBText;
    public TextMeshProUGUI hintText;

    // ── Internal ───────────────────────────────────────────────────────────
    private bool   isOpen         = false;
    private bool   choicesActive  = false;
    private float  openTime       = -999f;
    private Action pendingChoiceA = null;
    private Action pendingChoiceB = null;

    public bool IsOpen => isOpen;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);

        if (panel != null) panel.SetActive (false);
    }

    private void Start ()
    {
        if (choiceAButton != null) choiceAButton.onClick.AddListener (OnChoiceA);
        if (choiceBButton != null) choiceBButton.onClick.AddListener (OnChoiceB);
    }

    private void Update ()
    {
        if (!isOpen || choicesActive) return;
        if ((Time.time - openTime) >= 0.4f && Keyboard.current.xKey.wasPressedThisFrame)
            Close ();
    }

    // ══════════════════════════════════════════════════════════════════════
    // Open with choices
    // ══════════════════════════════════════════════════════════════════════

    public void Open (string npcName, string dialogue,
                      string labelA, string labelB,
                      Action onChoiceA, Action onChoiceB)
    {
        isOpen        = true;
        choicesActive = true;
        openTime      = Time.time;

        pendingChoiceA = onChoiceA;
        pendingChoiceB = onChoiceB;

        if (npcNameText  != null) npcNameText.text  = npcName;
        if (dialogueText != null) dialogueText.text = dialogue;
        if (choiceAText  != null) choiceAText.text  = labelA;
        if (choiceBText  != null) choiceBText.text  = labelB;
        if (hintText     != null) hintText.text     = "";

        if (choiceAButton != null) choiceAButton.gameObject.SetActive (true);
        if (choiceBButton != null) choiceBButton.gameObject.SetActive (true);

        if (panel != null) panel.SetActive (true);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Show follow-up after choice — hides buttons, shows close hint
    // ══════════════════════════════════════════════════════════════════════

    public void ShowFollowUp (string line)
    {
        choicesActive = false;
        openTime      = Time.time;

        if (dialogueText  != null) dialogueText.text = line;
        if (hintText      != null) hintText.text     = "[X] Close";

        if (choiceAButton != null) choiceAButton.gameObject.SetActive (false);
        if (choiceBButton != null) choiceBButton.gameObject.SetActive (false);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Choice handlers
    // ══════════════════════════════════════════════════════════════════════

    private void OnChoiceA ()
    {
        Action cb      = pendingChoiceA;
        pendingChoiceA = null;
        pendingChoiceB = null;
        cb?.Invoke ();
    }

    private void OnChoiceB ()
    {
        Action cb      = pendingChoiceB;
        pendingChoiceA = null;
        pendingChoiceB = null;
        cb?.Invoke ();
    }

    // ══════════════════════════════════════════════════════════════════════
    public void Close ()
    {
        isOpen         = false;
        choicesActive  = false;
        pendingChoiceA = null;
        pendingChoiceB = null;
        if (panel != null) panel.SetActive (false);
    }
}