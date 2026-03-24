using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class CollectibleUI : MonoBehaviour
{
    public static CollectibleUI Instance { get; private set; }

    [Header ("UI References")]
    public GameObject      panel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI effectText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI dismissText;

    [Header ("Settings")]
    public float autoDismissSeconds = 0f; // 0 = manual dismiss only

    private bool isOpen      = false;
    private bool canDismiss  = false;

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
        if (isOpen && canDismiss && Keyboard.current.xKey.wasPressedThisFrame)
            Close ();
    }

    // ══════════════════════════════════════════════════════════════════════
    public void Show (string itemName, string description, string effectString)
    {
        if (panel == null) return;

        isOpen     = true;
        canDismiss = false;

        if (itemNameText    != null) itemNameText.text    = itemName;
        if (descriptionText != null) descriptionText.text = description;
        if (effectText      != null) effectText.text      = effectString;

        int collected = CollectibleManager.Instance != null
            ? CollectibleManager.Instance.CountCollected ()
            : 0;
        if (countText != null)
            countText.text = $"{collected} / {CollectibleManager.TotalCollectibles} Relics Found";

        if (dismissText != null) dismissText.text = "[X] Close";

        panel.SetActive (true);
        StartCoroutine (EnableDismiss ());

        if (autoDismissSeconds > 0f)
            StartCoroutine (AutoDismiss ());
    }

    private IEnumerator EnableDismiss ()
    {
        yield return null;
        yield return null;
        canDismiss = true;
    }

    private IEnumerator AutoDismiss ()
    {
        yield return new WaitForSeconds (autoDismissSeconds);
        if (isOpen) Close ();
    }

    private void Close ()
    {
        isOpen     = false;
        canDismiss = false;
        if (panel != null) panel.SetActive (false);
    }
}