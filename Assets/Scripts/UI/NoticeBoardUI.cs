using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class NoticeBoardUI : MonoBehaviour
{
    public static NoticeBoardUI Instance { get; private set; }

    [Header ("Panel")]
    public GameObject panel;

    [Header ("Quest Cards")]
    public GameObject questCard1;
    public GameObject questCard2;
    public GameObject questCard3;

    [Header ("Quest Card Texts — Card 1")]
    public TextMeshProUGUI card1GiverText;
    public TextMeshProUGUI card1DescText;
    public TextMeshProUGUI card1RewardText;
    public TextMeshProUGUI card1DeadlineText;
    public Button          card1AcceptButton;

    [Header ("Quest Card Texts — Card 2")]
    public TextMeshProUGUI card2GiverText;
    public TextMeshProUGUI card2DescText;
    public TextMeshProUGUI card2RewardText;
    public TextMeshProUGUI card2DeadlineText;
    public Button          card2AcceptButton;

    [Header ("Quest Card Texts — Card 3")]
    public TextMeshProUGUI card3GiverText;
    public TextMeshProUGUI card3DescText;
    public TextMeshProUGUI card3RewardText;
    public TextMeshProUGUI card3DeadlineText;
    public Button          card3AcceptButton;

    [Header ("Other")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI activeQuestText;
    public TextMeshProUGUI hintText;

    private bool isOpen = false;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);
        if (panel != null) panel.SetActive (false);
    }

    private void Start ()
    {
        if (card1AcceptButton != null) card1AcceptButton.onClick.AddListener (() => AcceptQuest (0));
        if (card2AcceptButton != null) card2AcceptButton.onClick.AddListener (() => AcceptQuest (1));
        if (card3AcceptButton != null) card3AcceptButton.onClick.AddListener (() => AcceptQuest (2));
    }

    private void Update ()
    {
        if (isOpen && Keyboard.current.xKey.wasPressedThisFrame)
            Close ();
    }

    // ══════════════════════════════════════════════════════════════════════
    public void Open ()
    {
        isOpen = true;
        if (panel != null) panel.SetActive (true);
        Populate ();
    }

    public void Close ()
    {
        isOpen = false;
        if (panel != null) panel.SetActive (false);
    }

    // ══════════════════════════════════════════════════════════════════════
    private void Populate ()
    {
        if (titleText != null) titleText.text = "Notices of Athens";
        if (hintText  != null) hintText.text  = "[X] Close";

        // If player already has an active quest, show that instead
        if (GameState.Instance != null && GameState.Instance.hasActiveQuest)
        {
            if (questCard1 != null) questCard1.SetActive (false);
            if (questCard2 != null) questCard2.SetActive (false);
            if (questCard3 != null) questCard3.SetActive (false);

            if (activeQuestText != null)
            {
                int daysLeft = GameState.Instance.activeQuestDeadlineDays
                             - (TimeManager.Instance != null
                                ? TimeManager.Instance.GetCurrentDay () - GameState.Instance.activeQuestStartDay
                                : 0);
                activeQuestText.text =
                    $"Active Quest\n\n{GameState.Instance.activeQuestDescription}\n\nDays remaining: {daysLeft}\nReward: {GameState.Instance.activeQuestReward}";
                activeQuestText.gameObject.SetActive (true);
            }
            return;
        }

        if (activeQuestText != null) activeQuestText.gameObject.SetActive (false);

        var quests = QuestManager.Instance?.DailyQuests;

        if (quests == null || quests.Count == 0)
        {
            if (questCard1 != null) questCard1.SetActive (false);
            if (questCard2 != null) questCard2.SetActive (false);
            if (questCard3 != null) questCard3.SetActive (false);
            if (activeQuestText != null)
            {
                activeQuestText.text = "No notices today. Check back tomorrow.";
                activeQuestText.gameObject.SetActive (true);
            }
            return;
        }

        PopulateCard (questCard1, card1GiverText, card1DescText, card1RewardText, card1DeadlineText, card1AcceptButton, quests.Count > 0 ? quests[0] : null);
        PopulateCard (questCard2, card2GiverText, card2DescText, card2RewardText, card2DeadlineText, card2AcceptButton, quests.Count > 1 ? quests[1] : null);
        PopulateCard (questCard3, card3GiverText, card3DescText, card3RewardText, card3DeadlineText, card3AcceptButton, quests.Count > 2 ? quests[2] : null);
    }

    private void PopulateCard (GameObject card, TextMeshProUGUI giverText,
        TextMeshProUGUI descText, TextMeshProUGUI rewardText,
        TextMeshProUGUI deadlineText, Button acceptButton,
        QuestManager.Quest quest)
    {
        if (card == null) return;
        if (quest == null) { card.SetActive (false); return; }

        card.SetActive (true);
        if (giverText    != null) giverText.text    = $"Posted by {quest.giverName}";
        if (descText     != null) descText.text     = quest.description;
        if (rewardText   != null) rewardText.text   = $"Reward: {quest.rewardDescription}";
        if (deadlineText != null) deadlineText.text = $"Deadline: {quest.deadlineDays} days";
        if (acceptButton != null) acceptButton.interactable = true;
    }

    private void AcceptQuest (int index)
    {
        var quests = QuestManager.Instance?.DailyQuests;
        if (quests == null || index >= quests.Count) return;

        QuestManager.Instance.AcceptQuest (quests[index]);
        Close ();
    }
}