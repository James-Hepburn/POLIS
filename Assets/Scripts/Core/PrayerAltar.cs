using UnityEngine;
using UnityEngine.InputSystem;

public class PrayerAltar : MonoBehaviour
{
    [Header ("Settings")]
    public float interactionRadius = 2f;

    [Header ("UI")]
    public GameObject promptUI;
    public GameObject altarUI;

    // ── Internal ───────────────────────────────────────────────────────────
    private Transform player;
    private bool      playerInRange = false;
    private bool      altarOpen     = false;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        player = GameObject.FindWithTag ("Player")?.transform;
        if (promptUI != null) promptUI.SetActive (false);
        if (altarUI != null)  altarUI.SetActive (false);
    }

    private void Update ()
    {
        if (player == null) return;

        float distance = Vector2.Distance (transform.position, player.position);
        playerInRange  = distance <= interactionRadius;

        if (promptUI != null)
            promptUI.SetActive (playerInRange && !altarOpen);

        if (playerInRange && !altarOpen && Keyboard.current.eKey.wasPressedThisFrame)
            OpenAltar ();
    }

    public void OpenAltar ()
    {
        altarOpen = true;
        if (promptUI != null) promptUI.SetActive (false);
        if (altarUI != null)  altarUI.SetActive (true);
    }

    public void CloseAltar ()
    {
        altarOpen = false;
        if (altarUI != null) altarUI.SetActive (false);
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, interactionRadius);
    }
}