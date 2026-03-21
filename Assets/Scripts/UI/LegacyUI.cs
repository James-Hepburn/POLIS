using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LegacyUI : MonoBehaviour
{
    [Header ("Trigger")]
    public KeyCode toggleKey = KeyCode.L;

    [Header ("Confirmation Panel")]
    public GameObject      confirmPanel;
    public TextMeshProUGUI confirmText;
    public Button          confirmYesButton;
    public Button          confirmNoButton;

    [Header ("Legacy Panel")]
    public GameObject      legacyPanel;
    public TextMeshProUGUI prosperityText;
    public TextMeshProUGUI bondsText;
    public TextMeshProUGUI pietyText;
    public TextMeshProUGUI renownText;
    public TextMeshProUGUI totalText;
    public TextMeshProUGUI epitaphText;
    public Button          continueButton;

    [Header ("Hint")]
    public TextMeshProUGUI hintText;

    [Header ("Scene Indices")]
    public int mainMenuSceneIndex = 1;

    private bool confirmOpen = false;
    private bool legacyOpen  = false;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        if (confirmPanel != null) confirmPanel.SetActive (false);
        if (legacyPanel  != null) legacyPanel.SetActive (false);
        if (hintText     != null) hintText.text = "[L] End Story";

        if (confirmYesButton != null) confirmYesButton.onClick.AddListener (OnConfirmYes);
        if (confirmNoButton  != null) confirmNoButton.onClick.AddListener  (OnConfirmNo);
        if (continueButton   != null) continueButton.onClick.AddListener   (OnContinueToMenu);
    }

    private void Update ()
    {
        if (legacyOpen) return;

        if (Keyboard.current.lKey.wasPressedThisFrame)
            ToggleConfirm ();
    }

    // ══════════════════════════════════════════════════════════════════════
    private void ToggleConfirm ()
    {
        confirmOpen = !confirmOpen;
        if (confirmPanel != null)
        {
            confirmPanel.SetActive (confirmOpen);
            if (confirmOpen && confirmText != null)
                confirmText.text =
                    "You have chosen to end your story.\n\n" +
                    "Your life will be evaluated and committed to memory.\n\n" +
                    "This cannot be undone.\n\nAre you sure?";
        }
    }

    private void OnConfirmYes ()
    {
        confirmOpen = false;
        legacyOpen  = true;

        if (confirmPanel != null) confirmPanel.SetActive (false);

        // Delete save so Continue is greyed out on main menu
        GameState.DeleteSave ();

        ShowLegacy ();
    }

    private void OnConfirmNo ()
    {
        confirmOpen = false;
        if (confirmPanel != null) confirmPanel.SetActive (false);
    }

    // ══════════════════════════════════════════════════════════════════════
    private void ShowLegacy ()
    {
        if (GameState.Instance == null || legacyPanel == null) return;

        int prosperity = GameState.Instance.ScoreProsperity ();
        int bonds      = GameState.Instance.ScoreBonds ();
        int piety      = GameState.Instance.ScorePiety ();
        int renown     = GameState.Instance.ScoreRenown ();
        int total      = GameState.Instance.ScoreTotal ();
        string epitaph = GameState.Instance.GetEpitaph ();

        if (prosperityText != null)
            prosperityText.text = $"Prosperity\n{prosperity} / 100\n{DescribeProsperity (prosperity)}";

        if (bondsText != null)
            bondsText.text = $"Bonds\n{bonds} / 100\n{DescribeBonds (bonds)}";

        if (pietyText != null)
            pietyText.text = $"Piety\n{piety} / 100\n{DescribePiety (piety)}";

        if (renownText != null)
            renownText.text = $"Renown\n{renown} / 100\n{DescribeRenown (renown)}";

        if (totalText != null)
            totalText.text = $"Total Score: {total} / 400";

        if (epitaphText != null)
            epitaphText.text = epitaph;

        legacyPanel.SetActive (true);
        AudioManager.Instance?.PlayLegacyPanelOpens ();
    }

    private void OnContinueToMenu ()
    {
        SceneTransition.Instance.TransitionToScene (mainMenuSceneIndex);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Descriptors
    // ══════════════════════════════════════════════════════════════════════

    private string DescribeProsperity (int score)
    {
        if (score >= 80) return "A life of remarkable wealth and achievement.";
        if (score >= 50) return "A comfortable and productive life.";
        if (score >= 25) return "Modest means, honestly earned.";
        return "The world offered little, and took much.";
    }

    private string DescribeBonds (int score)
    {
        if (score >= 80) return "Loved by many. Athens mourns your passing.";
        if (score >= 50) return "True friends and a warm household.";
        if (score >= 25) return "A few who will remember you fondly.";
        return "You walked through Athens largely alone.";
    }

    private string DescribePiety (int score)
    {
        if (score >= 80) return "The gods knew your name. That is rare.";
        if (score >= 50) return "Your devotion did not go unnoticed.";
        if (score >= 25) return "You honoured the gods when you remembered to.";
        return "The gods looked elsewhere.";
    }

    private string DescribeRenown (int score)
    {
        if (score >= 80) return "Your name will outlast your body.";
        if (score >= 50) return "Athens knew who you were.";
        if (score >= 25) return "Some knew you. Fewer will remember.";
        return "You passed through without leaving a mark.";
    }
}