using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class QuestCompleteUI : MonoBehaviour
{
    public static QuestCompleteUI Instance { get; private set; }

    [Header ("UI References")]
    public GameObject      panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI giverText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI dismissText;

    private bool isOpen     = false;
    private bool canDismiss = false;

    public bool IsOpen => isOpen;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);
        if (panel != null) panel.SetActive (false);
    }

    private void Update ()
    {
        if (isOpen && canDismiss && Keyboard.current.aKey.wasPressedThisFrame)
            Close ();
    }

    // ══════════════════════════════════════════════════════════════════════
    public void Show (string giverName, string rewardDescription)
    {
        if (panel == null) return;

        isOpen     = true;
        canDismiss = false;

        if (titleText  != null) titleText.text  = "Quest Complete";
        if (giverText  != null) giverText.text  = $"{giverName} thanks you for your service.";
        if (rewardText != null) rewardText.text = rewardDescription;
        if (dismissText != null) dismissText.text = "[A] Accept Reward";

        panel.SetActive (true);
        AudioManager.Instance?.PlayHonourGained ();
        StartCoroutine (EnableDismiss ());
    }

    private IEnumerator EnableDismiss ()
    {
        yield return null;
        yield return null;
        canDismiss = true;
    }

    private void Close ()
    {
        isOpen     = false;
        canDismiss = false;
        if (panel != null) panel.SetActive (false);
    }
}