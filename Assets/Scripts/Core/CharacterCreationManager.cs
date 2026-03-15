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

    [Header ("Skin Tone")]
    public Button       skinTonePrev;
    public Button       skinToneNext;
    public TextMeshProUGUI skinToneLabel;

    [Header ("Hair Colour")]
    public Button       hairColorPrev;
    public Button       hairColorNext;
    public TextMeshProUGUI hairColorLabel;

    [Header ("Character Preview")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer hairRenderer;
    public SpriteRenderer clothingRenderer;

    [Header ("Skin Tone Sprites")]
    public Sprite[] skinToneSprites;   // fair, light, medium, olive, dark

    [Header ("Hair Colour Sprites")]
    public Sprite[] hairColorSprites;  // black, dark brown, brown, auburn, blonde, grey

    [Header ("Clothing Sprites")]
    public Sprite[] clothingSprites;   // merchant, soldier, philosopher, craftsman, priest

    // ── Internal ───────────────────────────────────────────────────────────
    private int skinToneIndex  = 2;    // Start on medium
    private int hairColorIndex = 0;    // Start on black

    private string[] skinToneNames  = { "Fair", "Light", "Medium", "Olive", "Dark" };
    private string[] hairColorNames = { "Black", "Dark Brown", "Brown", "Auburn", "Blonde", "Grey" };

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        SetupProfessionDropdown ();
        SetupGodDropdown ();

        startButton.onClick.AddListener (OnStartClicked);
        skinTonePrev.onClick.AddListener (() => CycleSkinTone (-1));
        skinToneNext.onClick.AddListener (() => CycleSkinTone (1));
        hairColorPrev.onClick.AddListener (() => CycleHairColor (-1));
        hairColorNext.onClick.AddListener (() => CycleHairColor (1));

        professionDropdown.onValueChanged.AddListener (OnProfessionChanged);

        UpdateCharacterPreview ();
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
    // Cycling
    // ══════════════════════════════════════════════════════════════════════

    private void CycleSkinTone (int direction)
    {
        skinToneIndex = (skinToneIndex + direction + skinToneSprites.Length) % skinToneSprites.Length;
        UpdateCharacterPreview ();
    }

    private void CycleHairColor (int direction)
    {
        hairColorIndex = (hairColorIndex + direction + hairColorSprites.Length) % hairColorSprites.Length;
        UpdateCharacterPreview ();
    }

    private void OnProfessionChanged (int index)
    {
        UpdateCharacterPreview ();
    }

    // ══════════════════════════════════════════════════════════════════════
    // Preview
    // ══════════════════════════════════════════════════════════════════════

    private void UpdateCharacterPreview ()
    {
        // Update body
        if (skinToneSprites != null && skinToneIndex < skinToneSprites.Length)
            bodyRenderer.sprite = skinToneSprites[skinToneIndex];

        // Update hair
        if (hairColorSprites != null && hairColorIndex < hairColorSprites.Length)
            hairRenderer.sprite = hairColorSprites[hairColorIndex];

        // Update clothing based on profession
        if (clothingSprites != null && professionDropdown.value < clothingSprites.Length)
            clothingRenderer.sprite = clothingSprites[professionDropdown.value];

        // Update labels
        if (skinToneLabel != null)
            skinToneLabel.text = skinToneNames[skinToneIndex];
        if (hairColorLabel != null)
            hairColorLabel.text = hairColorNames[hairColorIndex];
    }

    // ══════════════════════════════════════════════════════════════════════
    private void OnStartClicked ()
    {
        GameState.Profession profession =
            (GameState.Profession) professionDropdown.value;
        GameState.PatronGod god =
            (GameState.PatronGod) godDropdown.value;

        if (GameState.Instance == null)
        {
            Debug.LogError ("GameState not found.");
            return;
        }

        GameState.Instance.InitialiseNewGame (
            profession, god,
            skinTone:   skinToneIndex,
            hairColor:  hairColorIndex,
            hairStyle:  0,
            facialHair: 0,
            build:      0
        );

        SceneManager.LoadScene (1);
    }
}