using UnityEngine;
using UnityEngine.InputSystem;

public class NoticeBoard : MonoBehaviour
{
    [Header ("Settings")]
    public float interactionRadius = 1.5f;

    [Header ("UI")]
    public GameObject promptUI;

    private Transform player;
    private bool      playerInRange = false;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        player = GameObject.FindWithTag ("Player")?.transform;
        if (promptUI != null) promptUI.SetActive (false);
    }

    private void Update ()
    {
        if (player == null) return;

        float distance = Vector2.Distance (transform.position, player.position);
        playerInRange  = distance <= interactionRadius;

        if (promptUI != null) promptUI.SetActive (playerInRange);

        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
            OpenBoard ();
    }

    private void OpenBoard ()
    {
        // Generate today's quests if not already done
        if (QuestManager.Instance != null)
            QuestManager.Instance.GenerateDailyQuests ();

        // Open the notice board UI
        if (NoticeBoardUI.Instance != null)
            NoticeBoardUI.Instance.Open ();
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, interactionRadius);
    }
}