using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public class WorkStation : MonoBehaviour
{
    [Header ("Allowed Professions")]
    public GameState.Profession[] allowedProfessions;

    [Header ("Settings")]
    public string locationLabel        = "here";
    public float  interactionRadius    = 1.5f;
    public int    maxWorksPerDay       = 4;

    [Header ("UI")]
    public GameObject promptUI;

    // ── Profession Data ────────────────────────────────────────────────────
    private struct ProfessionWork
    {
        public string label;
        public float  timeCostMinutes;
        public float  drachmaReward;
        public int    xpReward;
    }

    private Dictionary<GameState.Profession, ProfessionWork> workData =
        new Dictionary<GameState.Profession, ProfessionWork>
    {
        { GameState.Profession.Merchant,    new ProfessionWork { label = "Trade",         timeCostMinutes = 90f,  drachmaReward = 8f, xpReward = 10 } },
        { GameState.Profession.Soldier,     new ProfessionWork { label = "Train",         timeCostMinutes = 120f, drachmaReward = 5f, xpReward = 15 } },
        { GameState.Profession.Philosopher, new ProfessionWork { label = "Debate",        timeCostMinutes = 90f,  drachmaReward = 6f, xpReward = 12 } },
        { GameState.Profession.Craftsman,   new ProfessionWork { label = "Work",          timeCostMinutes = 120f, drachmaReward = 9f, xpReward = 10 } },
        { GameState.Profession.Priest,      new ProfessionWork { label = "Perform Rites", timeCostMinutes = 60f,  drachmaReward = 4f, xpReward = 8  } },
    };

    // ── Internal ───────────────────────────────────────────────────────────
    private Transform      player;
    private bool           playerInRange = false;
    private int            worksToday    = 0;
    private int            lastWorkedDay = -1;
    private TextMeshProUGUI promptText;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        player = GameObject.FindWithTag ("Player")?.transform;

        if (promptUI != null)
        {
            promptUI.SetActive (false);
            promptText = promptUI.GetComponentInChildren<TextMeshProUGUI> ();
        }
    }

    private void Update ()
    {
        if (player == null) return;
        if (GameState.Instance == null) return;

        // Reset daily counter on new day
        if (TimeManager.Instance != null)
        {
            int today = TimeManager.Instance.GetCurrentDay ();
            if (today != lastWorkedDay)
            {
                worksToday    = 0;
                lastWorkedDay = today;
            }
        }

        float distance = Vector2.Distance (transform.position, player.position);
        playerInRange  = distance <= interactionRadius;

        bool professionAllowed = System.Array.IndexOf (allowedProfessions, GameState.Instance.currentProfession) >= 0;

        bool canWork = professionAllowed
                    && worksToday < maxWorksPerDay
                    && TimeManager.Instance != null
                    && TimeManager.Instance.IsDayActive ();

        if (promptUI != null && playerInRange && canWork)
        {
            if (promptText != null)
            {
                ProfessionWork work = workData[GameState.Instance.currentProfession];
                promptText.text = $"[E] {work.label} at the {locationLabel}";
            }
            promptUI.SetActive (true);
        }
        else if (promptUI != null)
        {
            promptUI.SetActive (false);
        }

        if (playerInRange && canWork && Keyboard.current.eKey.wasPressedThisFrame)
            DoWork ();
    }

    private void DoWork ()
    {
        ProfessionWork work = workData[GameState.Instance.currentProfession];

        TimeManager.Instance.AdvanceTimeByMinutes (work.timeCostMinutes);
        GameState.Instance.AddDrachma (work.drachmaReward);
        GameState.Instance.AddCareerXP (work.xpReward);
        worksToday++;

        Debug.Log ($"{work.label} — earned {work.drachmaReward} drachma. Total: {GameState.Instance.drachma}");
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere (transform.position, interactionRadius);
    }
}