using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class DivineInterventionUI : MonoBehaviour
{
    public static DivineInterventionUI Instance { get; private set; }

    [Header ("UI References")]
    public GameObject      panel;
    public TextMeshProUGUI godNameText;
    public TextMeshProUGUI narrativeText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI dismissText;

    private bool isOpen      = false;
    private bool canDismiss  = false;
    private GameState.PatronGod pendingGod;

    public bool IsOpen => isOpen;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);
        if (panel != null) panel.SetActive (false);
    }

    private void Update ()
    {
        if (isOpen && canDismiss && Keyboard.current.eKey.wasPressedThisFrame)
            Dismiss ();
    }

    // ══════════════════════════════════════════════════════════════════════
    public void Show (GameState.PatronGod god)
    {
        if (panel == null) return;

        pendingGod = god;
        isOpen     = true;
        canDismiss = false;

        var data = GetEventData (god);

        if (godNameText   != null) godNameText.text   = data.godName;
        if (narrativeText != null) narrativeText.text = data.narrative;
        if (rewardText    != null) rewardText.text    = data.reward;
        if (dismissText   != null) dismissText.text   = "[E] Accept";

        panel.SetActive (true);
        AudioManager.Instance?.PlayFestivalNotification ();
        StartCoroutine (EnableDismiss ());
    }

    private IEnumerator EnableDismiss ()
    {
        yield return null;
        yield return null;
        canDismiss = true;
    }

    private void Dismiss ()
    {
        isOpen     = false;
        canDismiss = false;

        // Apply reward
        ApplyReward (pendingGod);

        // Mark as fired
        if (GameState.Instance != null)
            GameState.Instance.SetIntervention (pendingGod);

        if (panel != null) panel.SetActive (false);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Event data
    // ══════════════════════════════════════════════════════════════════════

    private struct EventData
    {
        public string godName;
        public string narrative;
        public string reward;
    }

    private EventData GetEventData (GameState.PatronGod god)
    {
        switch (god)
        {
            case GameState.PatronGod.Hermes:
                return new EventData {
                    godName   = "Hermes Speaks",
                    narrative = "A stranger intercepts you near the Agora. His sandals are worn but his eyes are quick. He offers you a trade — a sealed amphora for a handful of coins. You hesitate. He smiles. \"Trust is the oldest currency,\" he says. You open the amphora that evening. It is full of silver.",
                    reward    = "Hermes has blessed you. You receive 80 drachma."
                };

            case GameState.PatronGod.Ares:
                return new EventData {
                    godName   = "Ares Watches",
                    narrative = "A soldier you have never met steps into your path. He says nothing — only looks at you, assessing. Then he nods once, slowly, as a general nods at a man worth fighting beside. Word spreads through the Gymnasium before nightfall. Ares has marked you.",
                    reward    = "Ares has honoured you. You gain 20 honour."
                };

            case GameState.PatronGod.Aphrodite:
                return new EventData {
                    godName   = "Aphrodite Smiles",
                    narrative = "You pass the Theatre at dusk and hear laughter from within. Through the open door you see her — Lydia, or perhaps Chloe, you cannot tell in the dying light — and for a moment she looks directly at you. Not through you. At you. The goddess has arranged this moment with care.",
                    reward    = "Aphrodite has intervened. Lydia and Chloe both warm to you."
                };

            case GameState.PatronGod.Apollo:
                return new EventData {
                    godName   = "Apollo Illuminates",
                    narrative = "Mid-argument in the Agora, a clarity descends on you like cold water. The words arrange themselves perfectly. Your opponent falls silent. The crowd leans in. You are not speaking — something is speaking through you. When it passes you are exhausted and electrified in equal measure.",
                    reward    = "Apollo has gifted you clarity. You gain a surge of career experience."
                };

            case GameState.PatronGod.Hephaestus:
                return new EventData {
                    godName   = "Hephaestus Guides",
                    narrative = "Your hands move without thought. The work that emerges is unlike anything you have produced before — proportions perfect, finish immaculate, every element exactly as it should be. You stare at it for a long time. A craftsman passing by stops, stares, and offers you twice the asking price without haggling.",
                    reward    = "Hephaestus has guided your hands. You receive 60 drachma and 10 honour."
                };

            case GameState.PatronGod.Athena:
                return new EventData {
                    godName   = "Athena Takes Notice",
                    narrative = "You are summoned — not by any official, but by rumour. People say your name in the Agora as an example. Of what, exactly, varies by who you ask: wisdom, integrity, industry. A city councillor stops you in the street to say he has heard good things. Athens has noticed you.",
                    reward    = "Athena has elevated you. You gain 15 honour and your relationships improve across the city."
                };

            default:
                return new EventData { godName = "The Gods Speak", narrative = "", reward = "" };
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // Rewards
    // ══════════════════════════════════════════════════════════════════════

    private void ApplyReward (GameState.PatronGod god)
    {
        if (GameState.Instance == null) return;

        switch (god)
        {
            case GameState.PatronGod.Hermes:
                GameState.Instance.AddDrachma (80f);
                break;

            case GameState.PatronGod.Ares:
                GameState.Instance.AddHonour (20);
                break;

            case GameState.PatronGod.Aphrodite:
                GameState.Instance.ChangeRelationship ("Lydia", 15);
                GameState.Instance.ChangeRelationship ("Chloe", 15);
                break;

            case GameState.PatronGod.Apollo:
                GameState.Instance.AddCareerXP (50);
                break;

            case GameState.PatronGod.Hephaestus:
                GameState.Instance.AddDrachma (60f);
                GameState.Instance.AddHonour (10);
                break;

            case GameState.PatronGod.Athena:
                GameState.Instance.AddHonour (15);
                GameState.Instance.ChangeRelationship ("Nikias",    5);
                GameState.Instance.ChangeRelationship ("Eudoros",   5);
                GameState.Instance.ChangeRelationship ("Kallias",   5);
                GameState.Instance.ChangeRelationship ("Demetrios", 5);
                GameState.Instance.ChangeRelationship ("Theron",    5);
                break;
        }
    }
}