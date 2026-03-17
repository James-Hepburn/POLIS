using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RestTrigger : MonoBehaviour
{
    [Header ("Settings")]
    public float interactionRadius = 1.5f;

    [Header ("UI")]
    public GameObject promptUI;

    private Transform       player;
    private bool            playerInRange = false;
    private TextMeshProUGUI promptText;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        player = GameObject.FindWithTag ("Player")?.transform;

        if (promptUI != null)
        {
            promptUI.SetActive (false);
            promptText = promptUI.GetComponentInChildren<TextMeshProUGUI> ();
            if (promptText != null)
                promptText.text = "[E] Rest for the night";
        }
    }

    private void Update ()
    {
        if (player == null) return;

        float distance = Vector2.Distance (transform.position, player.position);
        playerInRange  = distance <= interactionRadius;

        if (promptUI != null)
            promptUI.SetActive (playerInRange);

        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
            Rest ();
    }

    private void Rest ()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.EndDay ();
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere (transform.position, interactionRadius);
    }
}