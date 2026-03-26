using UnityEngine;
using UnityEngine.InputSystem;

public class PropertyBroker : MonoBehaviour, IInteractable
{
    [Header ("Settings")]
    public float interactionRadius = 1.5f;

    [Header ("Prompt UI")]
    public GameObject promptUI;

    // ── IInteractable ──────────────────────────────────────────────────────
    public Vector2 WorldPosition => (Vector2) transform.position;

    public bool IsEligible =>
        playerInRange
        && !(PropertyBrokerUI.Instance != null && PropertyBrokerUI.Instance.IsOpen);

    public void ShowPrompt (bool show)
    {
        if (promptUI != null) promptUI.SetActive (show);
    }

    // ── Internal ───────────────────────────────────────────────────────────
    private Transform player;
    private bool      playerInRange = false;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        player = GameObject.FindWithTag ("Player")?.transform;
        if (promptUI != null) promptUI.SetActive (false);

        if (InteractionPromptManager.Instance != null)
            InteractionPromptManager.Instance.Register (this);
    }

    private void OnDestroy ()
    {
        if (InteractionPromptManager.Instance != null)
            InteractionPromptManager.Instance.Unregister (this);
    }

    private void Update ()
    {
        if (player == null) return;

        float distance = Vector2.Distance (transform.position, player.position);
        playerInRange  = distance <= interactionRadius;

        if (InteractionPromptManager.Instance == null)
            ShowPrompt (IsEligible);

        if (IsEligible && playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
            Open ();
    }

    private void Open ()
    {
        if (PropertyBrokerUI.Instance != null)
            PropertyBrokerUI.Instance.Open ();
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere (transform.position, interactionRadius);
    }
}