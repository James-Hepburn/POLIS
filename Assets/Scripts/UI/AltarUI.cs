using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AltarUI : MonoBehaviour
{
    [Header ("God Buttons")]
    public Button hermesButton;
    public Button aresButton;
    public Button aphroditeButton;
    public Button apolloButton;
    public Button hephaestusButton;
    public Button athenaButton;

    [Header ("Action Panel")]
    public GameObject actionPanel;
    public TextMeshProUGUI selectedGodText;
    public TextMeshProUGUI currentFavourText;
    public Button prayButton;
    public Button offerButton;
    public Button backButton;
    public TextMeshProUGUI feedbackText;

    [Header ("Close")]
    public Button closeButton;

    [Header ("Other")]
    public TextMeshProUGUI titleText;
    public Image altarBackground;

    [Header ("Settings")]
    public float prayTimeCost    = 60f;
    public float offeringCost    = 10f;
    public int   prayFavourGain  = 5;
    public int   offerFavourGain = 15;

    private GameState.PatronGod selectedGod;
    private PrayerAltar         altar;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        altar = FindFirstObjectByType<PrayerAltar> ();

        hermesButton.onClick.AddListener     (() => SelectGod (GameState.PatronGod.Hermes));
        aresButton.onClick.AddListener       (() => SelectGod (GameState.PatronGod.Ares));
        aphroditeButton.onClick.AddListener  (() => SelectGod (GameState.PatronGod.Aphrodite));
        apolloButton.onClick.AddListener     (() => SelectGod (GameState.PatronGod.Apollo));
        hephaestusButton.onClick.AddListener (() => SelectGod (GameState.PatronGod.Hephaestus));
        athenaButton.onClick.AddListener     (() => SelectGod (GameState.PatronGod.Athena));

        prayButton.onClick.AddListener  (OnPray);
        offerButton.onClick.AddListener (OnOffer);
        backButton.onClick.AddListener  (OnBack);
        closeButton.onClick.AddListener (OnClose);

        actionPanel.SetActive (false);
        feedbackText.text = "";
    }

    // ══════════════════════════════════════════════════════════════════════
    private void SelectGod (GameState.PatronGod god)
    {
        selectedGod = god;
        feedbackText.text = "";

        selectedGodText.text   = $"{god}";
        currentFavourText.text = $"Current Favour: {GameState.Instance.GetFavour (god)}";

        offerButton.GetComponentInChildren<TextMeshProUGUI> ().text
            = $"Make Offering (₯ {offeringCost:F0})";

        SetGodSelectionVisible (false);
        actionPanel.SetActive (true);
    }

    private void SetGodSelectionVisible (bool visible)
    {
        if (titleText != null)             titleText.gameObject.SetActive (visible);

        hermesButton.gameObject.SetActive (visible);
        aresButton.gameObject.SetActive (visible);
        aphroditeButton.gameObject.SetActive (visible);
        apolloButton.gameObject.SetActive (visible);
        hephaestusButton.gameObject.SetActive (visible);
        athenaButton.gameObject.SetActive (visible);
        closeButton.gameObject.SetActive (visible);

        // Toggle background alpha instead of disabling
        if (altarBackground != null)
        {
            Color c = altarBackground.color;
            c.a = visible ? 220f / 255f : 0f;
            altarBackground.color = c;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    private void OnPray ()
    {
        if (GameState.Instance == null || TimeManager.Instance == null) return;

        TimeManager.Instance.AdvanceTimeByMinutes (prayTimeCost);

        bool doubleFavour = FestivalManager.Instance != null
                         && FestivalManager.Instance.IsDoubleFavourDay (selectedGod);
        int gain = doubleFavour ? prayFavourGain * 2 : prayFavourGain;

        GameState.Instance.ChangeFavour (selectedGod, gain);

        if (selectedGod == GameState.Instance.patronGod)
            GameState.Instance.prayedToPatronToday = true;

        currentFavourText.text = $"Current Favour: {GameState.Instance.GetFavour (selectedGod)}";
        feedbackText.text = doubleFavour
            ? $"You pray to {selectedGod} on this sacred day. The god is greatly pleased!"
            : $"You pray to {selectedGod}. The god is pleased.";

        Debug.Log ($"Prayed to {selectedGod}. Favour: {GameState.Instance.GetFavour (selectedGod)}");
    }

    private void OnOffer ()
    {
        if (GameState.Instance == null || TimeManager.Instance == null) return;

        if (!GameState.Instance.SpendDrachma (offeringCost))
        {
            feedbackText.text = "You do not have enough drachma for this offering.";
            return;
        }

        TimeManager.Instance.AdvanceTimeByMinutes (prayTimeCost);

        bool doubleFavour = FestivalManager.Instance != null
                         && FestivalManager.Instance.IsDoubleFavourDay (selectedGod);
        int gain = doubleFavour ? offerFavourGain * 2 : offerFavourGain;

        GameState.Instance.ChangeFavour (selectedGod, gain);

        if (selectedGod == GameState.Instance.patronGod)
            GameState.Instance.prayedToPatronToday = true;

        currentFavourText.text = $"Current Favour: {GameState.Instance.GetFavour (selectedGod)}";
        feedbackText.text = doubleFavour
            ? $"You make an offering to {selectedGod} on this sacred day. The god is greatly moved!"
            : $"You make an offering to {selectedGod}. The god smiles upon you.";

        Debug.Log ($"Offered to {selectedGod}. Favour: {GameState.Instance.GetFavour (selectedGod)}");
    }

    private void OnBack ()
    {
        actionPanel.SetActive (false);
        feedbackText.text = "";
        SetGodSelectionVisible (true);
    }

    private void OnClose ()
    {
        actionPanel.SetActive (false);
        feedbackText.text = "";
        SetGodSelectionVisible (true);
        if (altar != null) altar.CloseAltar ();
    }
}