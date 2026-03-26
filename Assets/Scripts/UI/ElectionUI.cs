using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ElectionUI : MonoBehaviour
{
    public static ElectionUI Instance { get; private set; }

    [Header ("Election Prompt Panel")]
    public GameObject      promptPanel;
    public TextMeshProUGUI promptTitleText;
    public TextMeshProUGUI promptBodyText;
    public Button          runButton;
    public Button          declineButton;
    public TextMeshProUGUI promptHintText;

    [Header ("Election Result Panel")]
    public GameObject      resultPanel;
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI resultBodyText;
    public TextMeshProUGUI resultRoleText;
    public TextMeshProUGUI resultHintText;

    private bool promptOpen = false;
    private bool resultOpen = false;
    private bool canDismissResult = false;

    public bool IsOpen => promptOpen || resultOpen;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);

        if (promptPanel != null) promptPanel.SetActive (false);
        if (resultPanel != null) resultPanel.SetActive (false);
    }

    private void Start ()
    {
        if (runButton     != null) runButton.onClick.AddListener (OnRun);
        if (declineButton != null) declineButton.onClick.AddListener (OnDecline);
    }

    private void Update ()
    {
        if (resultOpen && canDismissResult && Keyboard.current.xKey.wasPressedThisFrame)
            CloseResult ();
    }

    // ══════════════════════════════════════════════════════════════════════
    // Election prompt — shown at season start
    // ══════════════════════════════════════════════════════════════════════

    public void ShowElectionPrompt ()
    {
        promptOpen = true;

        if (promptTitleText != null) promptTitleText.text = "The City Calls";

        string currentRole = GameState.Instance != null && GameState.Instance.civicRole > 0
            ? $"You currently hold the position of {PoliticsManager.GetRoleName (GameState.Instance.civicRole)}. "
            : "";

        if (promptBodyText != null)
            promptBodyText.text =
                $"{currentRole}A new season begins in Athens. The Assembly is calling for candidates to serve the city in civic office. Your reputation has reached the ears of those who matter.\n\nWill you put your name forward?";

        if (promptHintText != null) promptHintText.text = "";

        if (promptPanel != null) promptPanel.SetActive (true);
        AudioManager.Instance?.PlayFestivalNotification ();
    }

    private void OnRun ()
    {
        promptOpen = false;
        if (promptPanel != null) promptPanel.SetActive (false);
        PoliticsManager.Instance?.EnterElection ();
        NPCDialogueUI.Instance?.Open (null,
            "You add your name to the list of candidates. Athens will decide.");
    }

    private void OnDecline ()
    {
        promptOpen = false;
        if (promptPanel != null) promptPanel.SetActive (false);
        if (GameState.Instance != null)
            GameState.Instance.hasRunThisSeason = true; // prevent prompt repeating
        Debug.Log ("Player declined the election.");
    }

    // ══════════════════════════════════════════════════════════════════════
    // Election result — shown the next morning
    // ══════════════════════════════════════════════════════════════════════

    public void ShowElectionResult (bool won, int role, int score)
    {
        resultOpen     = true;
        canDismissResult = false;

        if (won)
        {
            if (resultTitleText != null) resultTitleText.text = "Athens Has Spoken";
            if (resultBodyText  != null) resultBodyText.text  = GetWinText (role);
            if (resultRoleText  != null) resultRoleText.text  = $"You are now {PoliticsManager.GetRoleName (role)} of Athens.";
        }
        else
        {
            if (resultTitleText != null) resultTitleText.text = "The Vote Is Cast";
            if (resultBodyText  != null) resultBodyText.text  =
                "The Assembly has considered your candidacy. This time, the votes did not fall in your favour. Athens is not yet ready to elevate you — but the fact that you stood is noted. Rebuild your standing and try again next season.";
            if (resultRoleText  != null) resultRoleText.text  = "(-3 honour)";
        }

        if (resultHintText != null) resultHintText.text = "[X] Continue";
        if (resultPanel    != null) resultPanel.SetActive (true);

        AudioManager.Instance?.PlayFestivalNotification ();
        StartCoroutine (EnableDismiss ());
    }

    private IEnumerator EnableDismiss ()
    {
        yield return null;
        yield return null;
        canDismissResult = true;
    }

    private void CloseResult ()
    {
        resultOpen       = false;
        canDismissResult = false;
        if (resultPanel != null) resultPanel.SetActive (false);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Win text per role
    // ══════════════════════════════════════════════════════════════════════

    private string GetWinText (int role)
    {
        switch (role)
        {
            case 1:
                return "The votes are counted and your name rises to the top of the list. Councillor. It is a beginning — a seat at the table where Athens decides its own fate. Men you have never met will now nod at you in the Agora. Your daily stipend arrives with the morning light.";
            case 2:
                return "Magistrate. The Assembly chose you above the others and the word travels fast. You will sit in judgement, mediate disputes, and carry the city's authority in your hands. It is heavier than you expected, and you find you do not mind the weight.";
            case 3:
                return "Archon. The highest civic office Athens can bestow. Your name will be used to mark this year in the city's records — \"in the year of your archonship.\" You stand in a line that stretches back to the founding of the city. You hope you are worthy of it.";
            default:
                return "Athens has elected you to serve.";
        }
    }
}