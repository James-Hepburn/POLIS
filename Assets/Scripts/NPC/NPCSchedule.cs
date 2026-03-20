using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCSchedule : MonoBehaviour
{
    // ── Schedule Entry ─────────────────────────────────────────────────────
    [System.Serializable]
    public struct ScheduleEntry
    {
        public float   startHour;
        public float   endHour;
        public string  sceneName;          // "Athens", "AgoraInterior", etc.
        public Vector2 position;           // position in that scene
        public bool    isInsideBuilding;   // if true in Athens: walk to entrance then hide
        public Vector2 entrancePosition;   // Athens entrance to walk to before hiding
    }

    [Header ("Schedule")]
    public ScheduleEntry[] schedule;

    [Header ("Home")]
    public Vector2 homePosition;           // position in Athens to return to at night
    public string  homeSceneName = "Athens";

    [Header ("Festival Override")]
    public bool                          hasFestivalOverride = false;
    public FestivalManager.FestivalType  festivalType;
    public string                        festivalSceneName   = "Athens";
    public Vector2                       festivalPosition;
    public float                         festivalStartHour   = 8f;
    public float                         festivalEndHour     = 22f;

    [Header ("Movement")]
    public float moveSpeed        = 1.5f;
    public float arrivalThreshold = 0.05f;

    [Header ("Idle Wander")]
    public float wanderRadius  = 0.3f;
    public float wanderInterval = 3f;

    [Header ("Bob")]
    public float bobHeight = 0.04f;
    public float bobSpeed  = 8f;

    // ── State ──────────────────────────────────────────────────────────────
    private enum NPCState
    {
        Hidden,
        WalkingToSpot,
        AtSpot,
        WalkingToEntrance,   // walking to building entrance before disappearing
        InsideBuilding,      // inside a building (hidden in Athens, visible in interior)
        WalkingHome,
    }

    // ── Internal ───────────────────────────────────────────────────────────
    private NPC            npcComponent;
    private SpriteRenderer sr;

    private NPCState state          = NPCState.Hidden;
    private Vector2  currentTarget;
    private Vector2  scheduleTarget;
    private string   currentSceneName = "";

    private float    wanderTimer    = 0f;
    private Vector2  wanderOffset   = Vector2.zero;
    private float    baseY;

    private bool     transitionPending   = false;
    private NPCState transitionNextState;
    private float    transitionTime;

    private ScheduleEntry activeEntry;
    private bool          hasActiveEntry = false;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        npcComponent = GetComponent<NPC> ();
        sr           = GetComponent<SpriteRenderer> ();
    }

    private void OnEnable ()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable ()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;
        SnapToCorrectStateForScene ();
    }

    private void Start ()
    {
        currentSceneName = SceneManager.GetActiveScene ().name;
        SnapToCorrectStateForScene ();
    }

    // ══════════════════════════════════════════════════════════════════════
    // On scene load — snap immediately to correct state, no walking
    // ══════════════════════════════════════════════════════════════════════

    private void SnapToCorrectStateForScene ()
    {
        float hour = TimeManager.Instance != null ? TimeManager.Instance.GetCurrentHour () : 0f;
        hasActiveEntry = GetActiveEntry (hour, out activeEntry);

        ApplyScaleForScene (currentSceneName);

        if (!hasActiveEntry)
        {
            // Sleeping — only visible in Athens at home position
            if (currentSceneName == homeSceneName)
            {
                SnapTo (homePosition);
                SetState (NPCState.Hidden);
            }
            else
            {
                SetVisibility (false);
            }
            return;
        }

        // Active entry exists
        if (currentSceneName == activeEntry.sceneName)
        {
            // We are in the right scene
            if (activeEntry.isInsideBuilding && activeEntry.sceneName == "Athens")
            {
                // Should be inside building — hide in Athens
                SetVisibility (false);
                SetState (NPCState.InsideBuilding);
            }
            else
            {
                SnapTo (activeEntry.position);
                scheduleTarget = activeEntry.position;
                currentTarget  = activeEntry.position;
                SetVisibility (true);
                SetState (NPCState.AtSpot);
            }
        }
        else
        {
            // Wrong scene — hide
            SetVisibility (false);
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    private void Update ()
    {
        if (TimeManager.Instance == null) return;

        // If NPC has no active entry, they're sleeping — nothing to do
        // If NPC's active entry is in a different scene, hide and do nothing
        float hour         = TimeManager.Instance.GetCurrentHour ();
        bool  newHasEntry  = GetActiveEntry (hour, out ScheduleEntry newEntry);

        if (!newHasEntry)
        {
            // Sleeping — ensure hidden
            if (state != NPCState.Hidden && state != NPCState.WalkingHome)
            {
                bool inDialogue = npcComponent != null && npcComponent.IsDialogueOpen;
                if (!inDialogue)
                    QueueTransition (NPCState.WalkingHome, Random.Range (0f, 2f));
            }
            // Don't process movement in wrong scene
            if (currentSceneName != homeSceneName)
            {
                SetVisibility (false);
                return;
            }
        }
        else if (newEntry.sceneName != currentSceneName)
        {
            // NPC belongs in a different scene — hide and stop
            SetVisibility (false);
            hasActiveEntry = newHasEntry;
            activeEntry    = newEntry;
            return;
        }
        bool  entryChanged     = newHasEntry != hasActiveEntry
                              || (newHasEntry && (newEntry.sceneName != activeEntry.sceneName
                                              || newEntry.position   != activeEntry.position));

        if (entryChanged && !transitionPending)
        {
            bool inDialogue = npcComponent != null && npcComponent.IsDialogueOpen;
            if (!inDialogue)
            {
                hasActiveEntry = newHasEntry;
                activeEntry    = newEntry;
                QueueTransitionForEntry (newHasEntry, newEntry);
            }
        }

        // Execute pending transition
        if (transitionPending && Time.time >= transitionTime)
        {
            bool inDialogue = npcComponent != null && npcComponent.IsDialogueOpen;
            if (!inDialogue)
            {
                transitionPending = false;
                EnterState (transitionNextState);
            }
        }

        // Run current state
        switch (state)
        {
            case NPCState.WalkingToSpot:
                MoveToward (scheduleTarget + wanderOffset);
                if (HasArrived (scheduleTarget))
                    EnterState (NPCState.AtSpot);
                break;

            case NPCState.AtSpot:
                UpdateWander ();
                MoveToward (currentTarget);
                break;

            case NPCState.WalkingToEntrance:
                MoveToward (activeEntry.entrancePosition);
                if (HasArrived (activeEntry.entrancePosition))
                    EnterState (NPCState.InsideBuilding);
                break;

            case NPCState.WalkingHome:
                MoveToward (homePosition);
                if (HasArrived (homePosition))
                {
                    SnapTo (homePosition);
                    EnterState (NPCState.Hidden);
                }
                break;
        }

        // Bob
        bool isMoving = state == NPCState.WalkingToSpot
                     || state == NPCState.WalkingHome
                     || state == NPCState.WalkingToEntrance
                     || (state == NPCState.AtSpot && Vector2.Distance (
                             new Vector2 (transform.position.x, baseY), currentTarget) > arrivalThreshold);

        float bobY = isMoving ? Mathf.Sin (Time.time * bobSpeed) * bobHeight : 0f;
        transform.position = new Vector3 (transform.position.x, baseY + bobY, transform.position.z);
    }

    // ══════════════════════════════════════════════════════════════════════
    // State management
    // ══════════════════════════════════════════════════════════════════════

    private void QueueTransitionForEntry (bool hasEntry, ScheduleEntry entry)
    {
        if (!hasEntry)
        {
            // Time to go home
            if (currentSceneName == homeSceneName)
                QueueTransition (NPCState.WalkingHome, Random.Range (0f, 2f));
            else
                QueueTransition (NPCState.Hidden, 0f);
            return;
        }

        if (entry.sceneName != currentSceneName)
        {
            // Entry is in a different scene — hide here
            QueueTransition (NPCState.Hidden, 0f);
            return;
        }

        if (entry.isInsideBuilding && entry.sceneName == "Athens")
        {
            // Walk to building entrance then disappear
            scheduleTarget = entry.entrancePosition;
            QueueTransition (NPCState.WalkingToEntrance, Random.Range (0f, 2f));
        }
        else
        {
            // Normal spot in current scene
            scheduleTarget = entry.position;
            if (state == NPCState.Hidden)
            {
                // Wake up — appear at home and walk to spot
                QueueTransition (NPCState.WalkingToSpot, Random.Range (0f, 4f));
            }
            else
            {
                QueueTransition (NPCState.WalkingToSpot, Random.Range (0f, 2f));
            }
        }
    }

    private void EnterState (NPCState newState)
    {
        switch (newState)
        {
            case NPCState.WalkingToSpot:
                if (state == NPCState.Hidden)
                    SnapTo (homePosition);
                ApplyScaleForScene (currentSceneName);
                SetVisibility (true);
                currentTarget = scheduleTarget;
                wanderOffset  = Vector2.zero;
                break;

            case NPCState.AtSpot:
                wanderTimer   = wanderInterval;
                currentTarget = scheduleTarget;
                ApplyScaleForScene (currentSceneName);
                SetVisibility (true);
                break;

            case NPCState.WalkingToEntrance:
                ApplyScaleForScene ("Athens");
                SetVisibility (true);
                currentTarget = activeEntry.entrancePosition;
                break;

            case NPCState.InsideBuilding:
                if (currentSceneName == "Athens")
                    SetVisibility (false);
                else
                {
                    ApplyScaleForScene (currentSceneName);
                    SetVisibility (true);
                }
                break;

            case NPCState.WalkingHome:
                ApplyScaleForScene ("Athens");
                SetVisibility (true);
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

    private void ApplyScaleForScene (string sceneName)
    {
        // Interiors use scale 1.5, Athens uses scale 1
        float scale = (sceneName == "Athens" || sceneName == "") ? 1f : 1.5f;
        transform.localScale = new Vector3 (scale, scale, 1f);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Schedule lookup
    // ══════════════════════════════════════════════════════════════════════

    private bool GetActiveEntry (float hour, out ScheduleEntry entry)
    {
        // Marriage override — spouse sleeps at HomeInterior
        if (GameState.Instance != null
            && GameState.Instance.romanceStage == GameState.RomanceStage.Married
            && GameState.Instance.romanceTarget == GetComponent<NPC> ()?.npcName)
        {
            // Check if this hour falls outside the normal schedule (sleep time)
            bool hasNormalEntry = false;
            if (schedule != null)
                foreach (ScheduleEntry e in schedule)
                    if (hour >= e.startHour && hour < e.endHour) { hasNormalEntry = true; break; }

            if (!hasNormalEntry)
            {
                // Sleep in HomeInterior — walk to player's home door in Athens first
                entry = new ScheduleEntry {
                    startHour        = 0f,
                    endHour          = 24f,
                    sceneName        = "HomeInterior",
                    position         = new Vector2 (0f, -1f),
                    isInsideBuilding = true,
                    entrancePosition = new Vector2 (1f, -4.5f)
                };
                return true;
            }
        }

        // Festival override takes priority
        if (hasFestivalOverride
            && FestivalManager.Instance != null
            && FestivalManager.Instance.IsFestivalDay
            && FestivalManager.Instance.CurrentFestival.type == festivalType
            && hour >= festivalStartHour && hour < festivalEndHour)
        {
            entry = new ScheduleEntry
            {
                startHour   = festivalStartHour,
                endHour     = festivalEndHour,
                sceneName   = festivalSceneName,
                position    = festivalPosition,
                isInsideBuilding = false,
                entrancePosition = Vector2.zero
            };
            return true;
        }

        if (schedule != null)
        {
            foreach (ScheduleEntry e in schedule)
            {
                if (hour >= e.startHour && hour < e.endHour)
                {
                    entry = e;
                    return true;
                }
            }
        }

        entry = default;
        return false;
    }

    // ══════════════════════════════════════════════════════════════════════
    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere (new Vector3 (homePosition.x, homePosition.y, 0), 0.3f);

        if (schedule == null) return;
        foreach (ScheduleEntry entry in schedule)
        {
            Gizmos.color = entry.isInsideBuilding ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere (new Vector3 (entry.position.x, entry.position.y, 0), 0.3f);
            if (entry.isInsideBuilding)
                Gizmos.DrawWireSphere (new Vector3 (entry.entrancePosition.x, entry.entrancePosition.y, 0), 0.2f);
        }
    }
}