using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class BuildingEntrance : MonoBehaviour, IInteractable
{
    [Header ("Building Settings")]
    public string buildingName       = "Home";
    public int    interiorSceneIndex = 2;
    public float  interactionRadius  = 1.5f;

    [Header ("UI")]
    public GameObject promptUI;

    private Transform player;
    private bool      playerInRange = false;

    // ── IInteractable ──────────────────────────────────────────────────────
    public Vector2 WorldPosition => (Vector2) transform.position;
    public bool    IsEligible    => playerInRange;

    public void ShowPrompt (bool show)
    {
        if (promptUI != null) promptUI.SetActive (show);
    }

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
        if (player == null)
        {
            Debug.LogError ("Player reference is null!");
            return;
        }

        float distance = Vector2.Distance (transform.position, player.position);
        playerInRange  = distance <= interactionRadius;

        // Fallback — if no manager present (e.g. interior scenes), self-manage
        if (InteractionPromptManager.Instance == null)
            ShowPrompt (playerInRange);

        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
            EnterBuilding ();
    }

    private void EnterBuilding ()
    {
        PlayerPositionMemory.LastAthensPosition = player.position;
        PlayerPositionMemory.HasSavedPosition   = true;

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.TransitionToScene (interiorSceneIndex);
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, interactionRadius);
    }
}