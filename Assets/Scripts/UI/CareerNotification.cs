using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class CareerNotification : MonoBehaviour
{
    public static CareerNotification Instance { get; private set; }

    [Header ("UI")]
    public GameObject notificationUI;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public TextMeshProUGUI dismissText;

    private bool isShowing  = false;
    private bool canDismiss = false;
    public bool IsShowing => isShowing;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        Instance = this;
    }

    private void Start ()
    {
        if (notificationUI != null)
            notificationUI.SetActive (false);
    }

    private void Update ()
    {
        if (isShowing && canDismiss && Keyboard.current.xKey.wasPressedThisFrame)
            Dismiss ();
    }

    // ══════════════════════════════════════════════════════════════════════
    public void ShowLevelUp (int newLevel, GameState.Profession profession)
    {
        if (notificationUI == null) return;

        isShowing  = true;
        canDismiss = false;
        notificationUI.SetActive (true);

        titleText.text   = "Career Advancement";
        bodyText.text    = $"You have reached {GetCareerTitle (profession, newLevel)}.\n\n{GetFlavourText (profession, newLevel)}";
        dismissText.text = "[X] Continue";

        // Wait one frame before allowing dismissal
        StartCoroutine (EnableDismissNextFrame ());
    }

    private IEnumerator EnableDismissNextFrame ()
    {
        yield return null;
        yield return null; // two frames to be safe
        canDismiss = true;
    }

    private void Dismiss ()
    {
        isShowing  = false;
        canDismiss = false;
        if (notificationUI != null)
            notificationUI.SetActive (false);
    }

    // ══════════════════════════════════════════════════════════════════════
    private string GetCareerTitle (GameState.Profession profession, int level)
    {
        switch (profession)
        {
            case GameState.Profession.Merchant:
                return level == 2 ? "Established Merchant" : "Ship Owner";
            case GameState.Profession.Soldier:
                return level == 2 ? "Hoplite" : "Strategos";
            case GameState.Profession.Philosopher:
                return level == 2 ? "Teacher" : "Renowned Sophist";
            case GameState.Profession.Craftsman:
                return level == 2 ? "Journeyman" : "Master Craftsman";
            case GameState.Profession.Priest:
                return level == 2 ? "Priest" : "High Priest";
            default:
                return $"Level {level}";
        }
    }

    private string GetFlavourText (GameState.Profession profession, int level)
    {
        switch (profession)
        {
            case GameState.Profession.Merchant:
                return level == 2
                    ? "Your name is known in the Agora. Merchants nod as you pass."
                    : "You own a ship. The sea routes of Athens are yours to command.";
            case GameState.Profession.Soldier:
                return level == 2
                    ? "You carry a hoplite's shield with honour. Athens is safer for it."
                    : "Strategos. Men follow your orders without question.";
            case GameState.Profession.Philosopher:
                return level == 2
                    ? "Students seek you out. Your arguments are sharp as any blade."
                    : "Athens speaks your name alongside Socrates.";
            case GameState.Profession.Craftsman:
                return level == 2
                    ? "Your work is sought after. Argos himself approves."
                    : "Master. Your workshop is the finest in the Kerameikos.";
            case GameState.Profession.Priest:
                return level == 2
                    ? "The gods hear your prayers more clearly now."
                    : "High Priest. The city turns to you in times of divine need.";
            default:
                return "Your dedication has been rewarded.";
        }
    }
}