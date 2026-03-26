using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class LoanUI : MonoBehaviour
{
    public static LoanUI Instance { get; private set; }

    [Header ("UI References")]
    public GameObject      panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI hintText;

    [Header ("Loan Buttons")]
    public Button loan50Button;
    public Button loan100Button;
    public Button loan200Button;
    public Button repayButton;

    [Header ("Button Labels")]
    public TextMeshProUGUI loan50Text;
    public TextMeshProUGUI loan100Text;
    public TextMeshProUGUI loan200Text;
    public TextMeshProUGUI repayText;

    private bool isOpen = false;
    public bool IsOpen => isOpen;

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
        if (loan50Button  != null) loan50Button.onClick.AddListener  (() => TakeLoan (50f,  65f,  5));
        if (loan100Button != null) loan100Button.onClick.AddListener (() => TakeLoan (100f, 130f, 8));
        if (loan200Button != null) loan200Button.onClick.AddListener (() => TakeLoan (200f, 260f, 12));
        if (repayButton   != null) repayButton.onClick.AddListener   (RepayLoan);
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
        if (feedbackText != null) feedbackText.text = "";
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
        if (GameState.Instance == null) return;

        if (titleText != null) titleText.text = "Kallias, the Banker";
        if (hintText  != null) hintText.text  = "[X] Close";

        // Has defaulted — Kallias won't lend
        if (GameState.Instance.loanDefaulted)
        {
            if (bodyText != null)
                bodyText.text = "Kallias looks at you coldly. \"You have already cost me enough,\" he says. \"Find another banker.\" He turns away. You will not get a loan from Kallias again.";
            SetLoanButtonsActive (false);
            if (repayButton != null) repayButton.gameObject.SetActive (false);
            return;
        }

        // Has active loan
        if (GameState.Instance.hasActiveLoan)
        {
            int currentDay = TimeManager.Instance != null ? TimeManager.Instance.GetCurrentDay () : 1;
            int daysLeft   = GameState.Instance.loanDeadlineDay - currentDay;

            if (bodyText != null)
                bodyText.text =
                    $"You owe Kallias ₯{GameState.Instance.loanRepayAmount:F0}.\n\n" +
                    $"Deadline: {Mathf.Max (0, daysLeft)} days remaining.\n\n" +
                    $"You currently have ₯{GameState.Instance.drachma:F0}.";

            SetLoanButtonsActive (false);

            if (repayButton != null)
            {
                repayButton.gameObject.SetActive (true);
                repayButton.interactable = GameState.Instance.drachma >= GameState.Instance.loanRepayAmount;
                if (repayText != null)
                    repayText.text = $"Repay ₯{GameState.Instance.loanRepayAmount:F0}";
            }
            return;
        }

        // No active loan — show options
        if (bodyText != null)
            bodyText.text = "\"Ah, you need capital,\" Kallias says, settling back in his chair. \"I can arrange that. What do you need?\"";

        SetLoanButtonsActive (true);
        if (repayButton != null) repayButton.gameObject.SetActive (false);

        if (loan50Text  != null) loan50Text.text  = "Borrow ₯50\n(Repay ₯65 in 5 days)";
        if (loan100Text != null) loan100Text.text = "Borrow ₯100\n(Repay ₯130 in 8 days)";
        if (loan200Text != null) loan200Text.text = "Borrow ₯200\n(Repay ₯260 in 12 days)";
    }

    private void SetLoanButtonsActive (bool active)
    {
        if (loan50Button  != null) loan50Button.gameObject.SetActive (active);
        if (loan100Button != null) loan100Button.gameObject.SetActive (active);
        if (loan200Button != null) loan200Button.gameObject.SetActive (active);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Loan actions
    // ══════════════════════════════════════════════════════════════════════

    private void TakeLoan (float amount, float repayAmount, int deadlineDays)
    {
        if (GameState.Instance == null) return;
        if (GameState.Instance.hasActiveLoan) return;

        GameState.Instance.hasActiveLoan    = true;
        GameState.Instance.loanAmount       = amount;
        GameState.Instance.loanRepayAmount  = repayAmount;
        GameState.Instance.loanExtended     = false;
        GameState.Instance.loanDeadlineDay  = (TimeManager.Instance != null
            ? TimeManager.Instance.GetCurrentDay () : 1) + deadlineDays;

        GameState.Instance.AddDrachma (amount);

        if (feedbackText != null)
            feedbackText.text = $"Kallias counts out ₯{amount:F0} and slides it across the table. \"Don't make me regret this.\"";

        Populate ();
        Debug.Log ($"Loan taken: ₯{amount}, repay ₯{repayAmount} by day {GameState.Instance.loanDeadlineDay}");
    }

    private void RepayLoan ()
    {
        if (GameState.Instance == null) return;
        if (!GameState.Instance.hasActiveLoan) return;

        if (!GameState.Instance.SpendDrachma (GameState.Instance.loanRepayAmount))
        {
            if (feedbackText != null)
                feedbackText.text = "You do not have enough drachma to repay.";
            return;
        }

        float repaid = GameState.Instance.loanRepayAmount;

        GameState.Instance.hasActiveLoan   = false;
        GameState.Instance.loanAmount      = 0f;
        GameState.Instance.loanRepayAmount = 0f;
        GameState.Instance.loanDeadlineDay = 0;
        GameState.Instance.loanExtended    = false;

        GameState.Instance.ChangeRelationship ("Kallias", 5);

        if (feedbackText != null)
            feedbackText.text = $"You hand over ₯{repaid:F0}. Kallias nods. \"Prompt payment. I respect that.\" (+5 Kallias relationship)";

        AudioManager.Instance?.PlayHonourGained ();
        Populate ();
        Debug.Log ("Loan repaid.");
    }
}