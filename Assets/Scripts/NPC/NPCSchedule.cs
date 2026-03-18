using UnityEngine;

public class NPCSchedule : MonoBehaviour
{
    // ── Schedule Entry ─────────────────────────────────────────────────────
    [System.Serializable]
    public struct ScheduleEntry
    {
        public float startHour;    // e.g. 6f = 6:00am
        public float endHour;      // e.g. 12f = 12:00pm
        public Vector2 position;   // world position for this slot
    }

    [Header ("Schedule")]
    public ScheduleEntry[] schedule;

    [Header ("Home")]
    public Vector2 homePosition;   // position when no schedule entry is active (sleep hours)

    // ── Internal ───────────────────────────────────────────────────────────
    private NPC npcComponent;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        npcComponent = GetComponent<NPC> ();
    }

    private void Update ()
    {
        if (TimeManager.Instance == null) return;

        float hour = TimeManager.Instance.GetCurrentHour ();
        ApplySchedule (hour);
    }

    // ══════════════════════════════════════════════════════════════════════
    private void ApplySchedule (float hour)
    {
        // Find the active schedule entry for the current hour
        foreach (ScheduleEntry entry in schedule)
        {
            if (hour >= entry.startHour && hour < entry.endHour)
            {
                SetActive (true, entry.position);
                return;
            }
        }

        // No active entry — NPC is home (asleep or indoors)
        SetActive (false, homePosition);
    }

    private void SetActive (bool visible, Vector2 position)
    {
        // Move to position
        transform.position = new Vector3 (position.x, position.y, transform.position.z);

        // Show or hide all renderers and the NPC component
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();
        foreach (SpriteRenderer sr in renderers)
            sr.enabled = visible;

        // Disable NPC interaction when hidden
        if (npcComponent != null)
            npcComponent.enabled = visible;
    }

    // ══════════════════════════════════════════════════════════════════════
    private void OnDrawGizmosSelected ()
    {
        // Draw home position
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere (new Vector3 (homePosition.x, homePosition.y, 0), 0.3f);

        // Draw schedule positions
        if (schedule == null) return;
        Gizmos.color = Color.yellow;
        foreach (ScheduleEntry entry in schedule)
            Gizmos.DrawWireSphere (new Vector3 (entry.position.x, entry.position.y, 0), 0.3f);
    }
}