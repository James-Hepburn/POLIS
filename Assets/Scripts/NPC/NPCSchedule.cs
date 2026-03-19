using UnityEngine;

public class NPCSchedule : MonoBehaviour
{
    // ── Schedule Entry ─────────────────────────────────────────────────────
    [System.Serializable]
    public struct ScheduleEntry
    {
        public float   startHour;
        public float   endHour;
        public Vector2 position;
    }

    [Header ("Schedule")]
    public ScheduleEntry[] schedule;

    [Header ("Home")]
    public Vector2 homePosition;

    [Header ("Festival Override")]
    public bool    hasFestivalOverride = false;
    public FestivalManager.FestivalType festivalType;
    public Vector2 festivalPosition;
    public float   festivalStartHour = 8f;
    public float   festivalEndHour   = 22f;

    [Header ("Movement")]
    public float moveSpeed        = 1.5f;
    public float arrivalThreshold = 0.05f;

    [Header ("Idle Wander")]
    public float wanderRadius     = 0.3f;
    public float wanderInterval   = 3f;

    [Header ("Bob")]
    public float bobHeight        = 0.04f;
    public float bobSpeed         = 8f;

    // ── State ──────────────────────────────────────────────────────────────
    private enum NPCState
    {
        Hidden,           // asleep, not visible
        WalkingToSpot,    // just woke up, walking from home to schedule position
        AtSpot,           // arrived at schedule position, idle wandering
        WalkingHome,      // schedule ended, walking back to home position
    }

    // ── Internal ───────────────────────────────────────────────────────────
    private NPC            npcComponent;
    private SpriteRenderer sr;

    private NPCState state         = NPCState.Hidden;
    private Vector2  currentTarget;
    private Vector2  scheduleTarget;

    private float    wanderTimer   = 0f;
    private Vector2  wanderOffset  = Vector2.zero;
    private float    baseY;

    private float    transitionDelay     = 0f;
    private bool     transitionPending   = false;
    private NPCState transitionNextState;
    private float    transitionTime;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        npcComponent = GetComponent<NPC> ();
        sr           = GetComponent<SpriteRenderer> ();
    }

    private void Start ()
    {
        // On scene load — snap to correct state immediately, no walking
        float hour = TimeManager.Instance != null ? TimeManager.Instance.GetCurrentHour () : 0f;
        bool shouldBeVisible = GetScheduledPosition (hour, out Vector2 spotPos);

        if (shouldBeVisible)
        {
            // Snap directly to spot, skip the morning walk on load
            SnapTo (spotPos);
            SetState (NPCState.AtSpot);
            scheduleTarget = spotPos;
            currentTarget  = spotPos;
        }
        else
        {
            SnapTo (homePosition);
            SetState (NPCState.Hidden);
        }
    }

    private void Update ()
    {
        if (TimeManager.Instance == null) return;

        float hour           = TimeManager.Instance.GetCurrentHour ();
        bool  shouldBeActive = GetScheduledPosition (hour, out Vector2 spotPos);

        // ── Decide if a transition should be queued ────────────────────────
        if (shouldBeActive && state == NPCState.Hidden && !transitionPending)
        {
            // Time to wake up — queue with random delay
            QueueTransition (NPCState.WalkingToSpot, Random.Range (0f, 4f));
            scheduleTarget = spotPos;
        }
        else if (!shouldBeActive && (state == NPCState.AtSpot || state == NPCState.WalkingToSpot) && !transitionPending)
        {
            // Time to go home
            bool inDialogue = npcComponent != null && npcComponent.IsDialogueOpen;
            if (!inDialogue)
                QueueTransition (NPCState.WalkingHome, Random.Range (0f, 2f));
        }
        else if (shouldBeActive && scheduleTarget != spotPos && state == NPCState.AtSpot && !transitionPending)
        {
            // Schedule slot changed mid-day — walk to new spot
            scheduleTarget = spotPos;
            QueueTransition (NPCState.WalkingToSpot, Random.Range (0f, 2f));
        }

        // ── Execute pending transition ─────────────────────────────────────
        if (transitionPending && Time.time >= transitionTime)
        {
            bool inDialogue = npcComponent != null && npcComponent.IsDialogueOpen;
            if (!inDialogue)
            {
                transitionPending = false;
                EnterState (transitionNextState);
            }
        }

        // ── Run current state ──────────────────────────────────────────────
        switch (state)
        {
            case NPCState.WalkingToSpot:
                MoveToward (scheduleTarget + wanderOffset);
                if (HasArrived (scheduleTarget))
                    SetState (NPCState.AtSpot);
                break;

            case NPCState.AtSpot:
                UpdateWander ();
                MoveToward (currentTarget);
                break;

            case NPCState.WalkingHome:
                MoveToward (homePosition);
                if (HasArrived (homePosition))
                {
                    SnapTo (homePosition);
                    SetState (NPCState.Hidden);
                }
                break;
        }

        // ── Bob while moving ───────────────────────────────────────────────
        bool isMoving = state == NPCState.WalkingToSpot || state == NPCState.WalkingHome
                     || (state == NPCState.AtSpot && Vector2.Distance (
                             new Vector2 (transform.position.x, baseY), currentTarget) > arrivalThreshold);

        float bobY = isMoving ? Mathf.Sin (Time.time * bobSpeed) * bobHeight : 0f;
        transform.position = new Vector3 (transform.position.x, baseY + bobY, transform.position.z);
    }

    // ══════════════════════════════════════════════════════════════════════
    // State management
    // ══════════════════════════════════════════════════════════════════════

    private void EnterState (NPCState newState)
    {
        switch (newState)
        {
            case NPCState.WalkingToSpot:
                // Appear at home, then walk to spot
                SnapTo (homePosition);
                SetVisibility (true);
                currentTarget = scheduleTarget;
                wanderOffset  = Vector2.zero;
                break;

            case NPCState.AtSpot:
                wanderTimer  = wanderInterval;
                currentTarget = scheduleTarget;
                break;

            case NPCState.WalkingHome:
                currentTarget = homePosition;
                break;

            case NPCState.Hidden:
                SetVisibility (false);
                break;
        }
        state = newState;
    }

    private void SetState (NPCState newState)
    {
        state = newState;
        if (newState == NPCState.Hidden)
            SetVisibility (false);
    }

    private void QueueTransition (NPCState nextState, float delay)
    {
        transitionPending   = true;
        transitionNextState = nextState;
        transitionTime      = Time.time + delay;
    }

    // ══════════════════════════════════════════════════════════════════════
    // Movement helpers
    // ══════════════════════════════════════════════════════════════════════

    private void MoveToward (Vector2 target)
    {
        Vector2 pos2D = new Vector2 (transform.position.x, baseY);
        if (Vector2.Distance (pos2D, target) <= arrivalThreshold) return;

        Vector2 dir    = (target - pos2D).normalized;
        Vector2 newPos = pos2D + dir * moveSpeed * Time.deltaTime;
        baseY          = newPos.y;
        transform.position = new Vector3 (newPos.x, newPos.y, transform.position.z);
    }

    private bool HasArrived (Vector2 target)
    {
        return Vector2.Distance (new Vector2 (transform.position.x, baseY), target) <= arrivalThreshold;
    }

    private void UpdateWander ()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f)
        {
            wanderTimer   = wanderInterval + Random.Range (-1f, 1f);
            wanderOffset  = Random.insideUnitCircle * wanderRadius;
            currentTarget = scheduleTarget + wanderOffset;
        }
    }

    private void SnapTo (Vector2 position)
    {
        transform.position = new Vector3 (position.x, position.y, transform.position.z);
        baseY              = position.y;
        currentTarget      = position;
    }

    private void SetVisibility (bool visible)
    {
        if (sr != null)           sr.enabled           = visible;
        if (npcComponent != null) npcComponent.enabled = visible;
    }

    // ══════════════════════════════════════════════════════════════════════
    private bool GetScheduledPosition (float hour, out Vector2 position)
    {
        // Festival override takes priority over normal schedule
        if (hasFestivalOverride
            && FestivalManager.Instance != null
            && FestivalManager.Instance.IsFestivalDay
            && FestivalManager.Instance.CurrentFestival.type == festivalType
            && hour >= festivalStartHour && hour < festivalEndHour)
        {
            position = festivalPosition;
            return true;
        }

        if (schedule != null)
        {
            foreach (ScheduleEntry entry in schedule)
            {
                if (hour >= entry.startHour && hour < entry.endHour)
                {
                    position = entry.position;
                    return true;
                }
            }
        }
        position = homePosition;
        return false;
    }

    // ══════════════════════════════════════════════════════════════════════
    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere (new Vector3 (homePosition.x, homePosition.y, 0), 0.3f);

        if (schedule == null) return;
        Gizmos.color = Color.yellow;
        foreach (ScheduleEntry entry in schedule)
            Gizmos.DrawWireSphere (new Vector3 (entry.position.x, entry.position.y, 0), 0.3f);
    }
}