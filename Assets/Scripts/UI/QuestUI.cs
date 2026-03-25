using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [Header ("UI")]
    public GameObject      questPanel;
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questBodyText;
    public TextMeshProUGUI hintText;

    private bool isOpen = false;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        if (questPanel != null) questPanel.SetActive (false);
        if (hintText   != null) hintText.text = "[Q] Close";
    }

    private void Update ()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
            Toggle ();
    }

    private void Toggle ()
    {
        isOpen = !isOpen;
        if (questPanel != null) questPanel.SetActive (isOpen);
        if (isOpen) Populate ();
    }

    // ══════════════════════════════════════════════════════════════════════
    private void Populate ()
    {
        if (GameState.Instance == null) return;

        if (questTitleText != null) questTitleText.text = "Active Quest";

        if (!GameState.Instance.hasActiveQuest)
        {
            if (questBodyText != null)
                questBodyText.text = "You have no active quest.\n\nVisit the Notice Board in Athens to pick one up.";
            return;
        }

        if (GameState.Instance.activeQuestComplete)
        {
            if (questBodyText != null)
                questBodyText.text = "Quest Complete!\n\nYour reward will be delivered at the end of the day.";
            return;
        }

        int currentDay = TimeManager.Instance != null ? TimeManager.Instance.GetCurrentDay () : 1;
        int daysLeft   = GameState.Instance.activeQuestDeadlineDays
                       - (currentDay - GameState.Instance.activeQuestStartDay);

        System.Text.StringBuilder sb = new System.Text.StringBuilder ();
        sb.AppendLine ($"From: {GameState.Instance.activeQuestGiver}");
        sb.AppendLine ();
        sb.AppendLine (GameState.Instance.activeQuestDescription);
        sb.AppendLine ();

        // Show progress for trackable quests
        QuestManager.QuestType type = (QuestManager.QuestType) GameState.Instance.activeQuestType;
        if (type == QuestManager.QuestType.WorkHard)
            sb.AppendLine ($"Progress: {GameState.Instance.activeQuestProgress} / {GameState.Instance.activeQuestTargetAmount} work sessions");
        else if (type == QuestManager.QuestType.AccumulateHonour)
            sb.AppendLine ($"Progress: {GameState.Instance.honour} / {GameState.Instance.activeQuestTargetAmount} honour");

        sb.AppendLine ($"Days remaining: {Mathf.Max (0, daysLeft)}");
        sb.AppendLine ();
        sb.AppendLine ($"Reward: {GameState.Instance.activeQuestReward}");

        if (questBodyText != null) questBodyText.text = sb.ToString ();
    }
}