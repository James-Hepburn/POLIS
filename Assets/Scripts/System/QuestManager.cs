using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    // ── Quest Types ────────────────────────────────────────────────────────
    public enum QuestType
    {
        EarnDrachma,
        TalkToSomeone,
        PrayToGod,
        WorkHard,
        MakeOffering,
        AccumulateHonour,
        AttendFestival
    }

    [System.Serializable]
    public class Quest
    {
        public QuestType type;
        public string    giverName;
        public string    targetName;
        public int       targetAmount;
        public int       deadlineDays;
        public string    description;
        public string    rewardDescription;
    }

    // ── Internal ───────────────────────────────────────────────────────────
    private List<Quest> _dailyQuests      = new List<Quest> ();
    private int         _lastGeneratedDay = -1;

    public List<Quest> DailyQuests => _dailyQuests;

    private static readonly string[] AllNPCs = {
        "Nikias", "Demetrios", "Theron", "Argos", "Eudoros", "Chloe",
        "Kallias", "Lydia", "Miriam", "Phaedra", "Stephanos", "Xanthos"
    };

    private static readonly string[] AllGods = {
        "Hermes", "Ares", "Aphrodite", "Apollo", "Hephaestus", "Athena"
    };

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Quest generation
    // ══════════════════════════════════════════════════════════════════════

    public void GenerateDailyQuests ()
    {
        if (TimeManager.Instance == null) return;
        int today = TimeManager.Instance.GetCurrentDay ();
        if (today == _lastGeneratedDay) return;

        _lastGeneratedDay = today;
        _dailyQuests.Clear ();

        List<QuestType> types = new List<QuestType> {
            QuestType.EarnDrachma, QuestType.TalkToSomeone, QuestType.PrayToGod,
            QuestType.WorkHard, QuestType.MakeOffering, QuestType.AccumulateHonour,
            QuestType.AttendFestival
        };

        for (int i = types.Count - 1; i > 0; i--)
        {
            int j = Random.Range (0, i + 1);
            QuestType tmp = types[i]; types[i] = types[j]; types[j] = tmp;
        }

        for (int i = 0; i < 3; i++)
            _dailyQuests.Add (BuildQuest (types[i]));
    }

    private Quest BuildQuest (QuestType type)
    {
        string giver  = AllNPCs[Random.Range (0, AllNPCs.Length)];
        string god    = AllGods[Random.Range (0, AllGods.Length)];
        string target = AllNPCs[Random.Range (0, AllNPCs.Length)];
        while (target == giver)
            target = AllNPCs[Random.Range (0, AllNPCs.Length)];

        switch (type)
        {
            case QuestType.EarnDrachma:
                int amount = Random.Range (2, 5) * 20;
                return new Quest {
                    type              = type,
                    giverName         = giver,
                    targetAmount      = amount,
                    deadlineDays      = 4,
                    description       = $"{giver} asks you to earn {amount} drachma through honest work within 4 days.",
                    rewardDescription = $"+15 relationship with {giver}, +10 drachma bonus"
                };

            case QuestType.TalkToSomeone:
                return new Quest {
                    type              = type,
                    giverName         = giver,
                    targetName        = target,
                    deadlineDays      = 3,
                    description       = $"{giver} needs you to speak with {target} on their behalf within 3 days.",
                    rewardDescription = $"+15 relationship with {giver}, +5 honour"
                };

            case QuestType.PrayToGod:
                return new Quest {
                    type              = type,
                    giverName         = giver,
                    targetName        = god,
                    deadlineDays      = 2,
                    description       = $"{giver} asks you to pray to {god} on their behalf within 2 days.",
                    rewardDescription = $"+15 relationship with {giver}, +10 favour with {god}"
                };

            case QuestType.WorkHard:
                int sessions = Random.Range (2, 4);
                return new Quest {
                    type              = type,
                    giverName         = giver,
                    targetAmount      = sessions,
                    deadlineDays      = 3,
                    description       = $"{giver} needs you to work {sessions} times within 3 days to prove your dedication.",
                    rewardDescription = $"+15 relationship with {giver}, +15 drachma"
                };

            case QuestType.MakeOffering:
                return new Quest {
                    type              = type,
                    giverName         = giver,
                    targetName        = god,
                    deadlineDays      = 2,
                    description       = $"{giver} asks you to make an offering to {god} within 2 days.",
                    rewardDescription = $"+15 relationship with {giver}, +15 favour with {god}"
                };

            case QuestType.AccumulateHonour:
                int honour = (GameState.Instance != null ? GameState.Instance.honour : 0) + Random.Range (10, 20);
                return new Quest {
                    type              = type,
                    giverName         = giver,
                    targetAmount      = honour,
                    deadlineDays      = 5,
                    description       = $"{giver} wants to vouch for you. Reach {honour} honour within 5 days.",
                    rewardDescription = $"+15 relationship with {giver}, +5 honour"
                };

            case QuestType.AttendFestival:
                return new Quest {
                    type              = type,
                    giverName         = giver,
                    deadlineDays      = 5,
                    description       = $"{giver} asks you to be present in Athens on the next festival day.",
                    rewardDescription = $"+15 relationship with {giver}, +5 honour"
                };

            default:
                return new Quest {
                    type = type, giverName = giver, deadlineDays = 3,
                    description = "A task awaits.", rewardDescription = "+15 relationship"
                };
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // Quest acceptance
    // ══════════════════════════════════════════════════════════════════════

    public void AcceptQuest (Quest quest)
    {
        if (GameState.Instance == null) return;

        GameState.Instance.activeQuestType         = (int) quest.type;
        GameState.Instance.activeQuestGiver        = quest.giverName;
        GameState.Instance.activeQuestTarget       = quest.targetName ?? "";
        GameState.Instance.activeQuestTargetAmount = quest.targetAmount;
        GameState.Instance.activeQuestDeadlineDays = quest.deadlineDays;
        GameState.Instance.activeQuestProgress     = 0;
        GameState.Instance.activeQuestComplete     = false;
        GameState.Instance.hasActiveQuest          = true;
        GameState.Instance.activeQuestDescription  = quest.description;
        GameState.Instance.activeQuestReward       = quest.rewardDescription;
        GameState.Instance.activeQuestStartDay     = TimeManager.Instance != null
            ? TimeManager.Instance.GetCurrentDay () : 1;

        _dailyQuests.Clear ();
        Debug.Log ($"Quest accepted: {quest.description}");
    }

    // ══════════════════════════════════════════════════════════════════════
    // Player action hooks
    // ══════════════════════════════════════════════════════════════════════

    // Called from WorkStation with the actual drachma earned that session
    public void OnPlayerWorked (float drachmaEarned)
    {
        if (GameState.Instance == null) return;

        if (IsQuestActive (QuestType.WorkHard))
        {
            GameState.Instance.activeQuestProgress++;
            if (GameState.Instance.activeQuestProgress >= GameState.Instance.activeQuestTargetAmount)
                CompleteQuest ();
            return;
        }

        if (IsQuestActive (QuestType.EarnDrachma))
        {
            GameState.Instance.activeQuestProgress += Mathf.RoundToInt (drachmaEarned);
            if (GameState.Instance.activeQuestProgress >= GameState.Instance.activeQuestTargetAmount)
                CompleteQuest ();
        }
    }

    public void OnPlayerPrayed (string godName)
    {
        if (!IsQuestActive (QuestType.PrayToGod)) return;
        if (GameState.Instance.activeQuestTarget == godName)
            CompleteQuest ();
    }

    public void OnPlayerMadeOffering (string godName)
    {
        if (!IsQuestActive (QuestType.MakeOffering)) return;
        if (GameState.Instance.activeQuestTarget == godName)
            CompleteQuest ();
    }

    public void OnPlayerTalkedToNPC (string npcName)
    {
        if (!IsQuestActive (QuestType.TalkToSomeone)) return;
        if (GameState.Instance.activeQuestTarget == npcName)
            CompleteQuest ();
    }

    // ══════════════════════════════════════════════════════════════════════
    // End of day
    // ══════════════════════════════════════════════════════════════════════

    public void ProcessEndOfDay ()
    {
        if (GameState.Instance == null) return;
        if (!GameState.Instance.hasActiveQuest) return;

        // Already completed mid-day — reward already applied, just clear
        if (GameState.Instance.activeQuestComplete)
        {
            GameState.Instance.hasActiveQuest = false;
            return;
        }

        int currentDay  = TimeManager.Instance != null ? TimeManager.Instance.GetCurrentDay () : 1;
        int daysElapsed = currentDay - GameState.Instance.activeQuestStartDay;

        if (daysElapsed >= GameState.Instance.activeQuestDeadlineDays)
        {
            GameState.Instance.pendingEndOfDayEvents.Add (
                $"Quest from {GameState.Instance.activeQuestGiver} has expired. You did not complete it in time.");
            GameState.Instance.ChangeRelationship (GameState.Instance.activeQuestGiver, -5);
            AbandonQuest ();
            return;
        }

        // Auto-complete checks at end of day
        QuestType type = (QuestType) GameState.Instance.activeQuestType;

        if (type == QuestType.AccumulateHonour
            && GameState.Instance.honour >= GameState.Instance.activeQuestTargetAmount)
        {
            CompleteQuest ();
            return;
        }

        if (type == QuestType.AttendFestival
            && FestivalManager.Instance != null
            && FestivalManager.Instance.IsFestivalDay)
        {
            CompleteQuest ();
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // Completion and abandonment
    // ══════════════════════════════════════════════════════════════════════

    public void CompleteQuest ()
    {
        if (GameState.Instance == null) return;

        string    giver  = GameState.Instance.activeQuestGiver;
        string    target = GameState.Instance.activeQuestTarget;
        QuestType type   = (QuestType) GameState.Instance.activeQuestType;

        GameState.Instance.ChangeRelationship (giver, 15);

        switch (type)
        {
            case QuestType.EarnDrachma:
                GameState.Instance.AddDrachma (10f);
                break;
            case QuestType.TalkToSomeone:
                GameState.Instance.AddHonour (5);
                break;
            case QuestType.PrayToGod:
                GameState.Instance.ChangeFavour (ParseGod (target), 10);
                break;
            case QuestType.WorkHard:
                GameState.Instance.AddDrachma (15f);
                break;
            case QuestType.MakeOffering:
                GameState.Instance.ChangeFavour (ParseGod (target), 15);
                break;
            case QuestType.AccumulateHonour:
                GameState.Instance.AddHonour (5);
                break;
            case QuestType.AttendFestival:
                GameState.Instance.AddHonour (5);
                break;
        }

        GameState.Instance.pendingEndOfDayEvents.Add (
            $"Quest complete! {giver} thanks you. ({GameState.Instance.activeQuestReward})");

        GameState.Instance.activeQuestComplete = true;
        GameState.Instance.hasActiveQuest      = false;
        AudioManager.Instance?.PlayHonourGained ();

        // Show completion popup immediately
        QuestCompleteUI.Instance?.Show (giver, GameState.Instance.activeQuestReward);

        Debug.Log ($"Quest completed: {giver}");
    }

    public void AbandonQuest ()
    {
        if (GameState.Instance == null) return;
        GameState.Instance.hasActiveQuest      = false;
        GameState.Instance.activeQuestComplete = false;
        GameState.Instance.activeQuestProgress = 0;
    }

    // ══════════════════════════════════════════════════════════════════════
    // Helpers
    // ══════════════════════════════════════════════════════════════════════

    public bool IsQuestActive (QuestType type)
    {
        return GameState.Instance != null
            && GameState.Instance.hasActiveQuest
            && !GameState.Instance.activeQuestComplete
            && (QuestType) GameState.Instance.activeQuestType == type;
    }

    public bool HasActiveQuest =>
        GameState.Instance != null && GameState.Instance.hasActiveQuest;

    private GameState.PatronGod ParseGod (string name)
    {
        switch (name)
        {
            case "Hermes":     return GameState.PatronGod.Hermes;
            case "Ares":       return GameState.PatronGod.Ares;
            case "Aphrodite":  return GameState.PatronGod.Aphrodite;
            case "Apollo":     return GameState.PatronGod.Apollo;
            case "Hephaestus": return GameState.PatronGod.Hephaestus;
            case "Athena":     return GameState.PatronGod.Athena;
            default:           return GameState.PatronGod.Hermes;
        }
    }
}