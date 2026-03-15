using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterCreationManager : MonoBehaviour
{
    [Header ("UI References")]
    public TMP_Dropdown professionDropdown;
    public TMP_Dropdown godDropdown;
    public Button       startButton;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        SetupProfessionDropdown ();
        SetupGodDropdown ();
        startButton.onClick.AddListener (OnStartClicked);
    }

    // ══════════════════════════════════════════════════════════════════════
    private void SetupProfessionDropdown ()
    {
        professionDropdown.ClearOptions ();
        professionDropdown.AddOptions (new System.Collections.Generic.List<string>
        {
            "Merchant",
            "Soldier",
            "Philosopher",
            "Craftsman",
            "Priest"
        });
    }

    private void SetupGodDropdown ()
    {
        godDropdown.ClearOptions ();
        godDropdown.AddOptions (new System.Collections.Generic.List<string>
        {
            "Hermes  — Trade & Luck",
            "Ares    — Combat & Honour",
            "Aphrodite — Love & Beauty",
            "Apollo  — Knowledge & Arts",
            "Hephaestus — Craft & Fire",
            "Athena  — Wisdom & the City"
        });
    }

    // ══════════════════════════════════════════════════════════════════════
    private void OnStartClicked ()
    {
        // Read dropdown selections
        GameState.Profession profession =
            (GameState.Profession) professionDropdown.value;

        GameState.PatronGod god =
            (GameState.PatronGod) godDropdown.value;

        // Make sure GameState exists
        if (GameState.Instance == null)
        {
            Debug.LogError ("GameState not found. Make sure it exists in the scene.");
            return;
        }

        // Initialise the game with chosen values
        // Appearance is all default for now — we'll add that later
        GameState.Instance.InitialiseNewGame (
            profession,
            god,
            skinTone:   0,
            hairColor:  0,
            hairStyle:  0,
            facialHair: 0,
            build:      0
        );

        // Load Athens
        SceneManager.LoadScene (1);
    }
}