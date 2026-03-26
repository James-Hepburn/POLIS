using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PropertyBrokerUI : MonoBehaviour
{
    public static PropertyBrokerUI Instance { get; private set; }

    [Header ("UI References")]
    public GameObject      panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI shopNameText;
    public TextMeshProUGUI shopDescText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI feedbackText;
    public Button          buyButton;
    public TextMeshProUGUI hintText;

    private bool isOpen = false;
    public bool IsOpen => isOpen;

    private const float ShopCost = 200f;

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
        if (buyButton != null) buyButton.onClick.AddListener (OnBuy);
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
        if (GameState.Instance == null) return;

        if (titleText != null) titleText.text = "Property Broker";
        if (hintText  != null) hintText.text  = "[X] Close";
        if (feedbackText != null) feedbackText.text = "";

        // Already owns a shop
        if (GameState.Instance.hasShop)
        {
            if (shopNameText != null) shopNameText.text = GetShopName ();
            if (shopDescText != null) shopDescText.text = "Your establishment is running well. Passive income arrives each morning.";
            if (costText     != null) costText.text     = "Owned";
            if (incomeText   != null) incomeText.text   = $"Daily income: {GetIncomeDescription ()}";
            if (buyButton    != null) buyButton.gameObject.SetActive (false);
            return;
        }

        // Career level too low
        if (GameState.Instance.careerLevel < 2)
        {
            if (shopNameText != null) shopNameText.text = "Not Yet Available";
            if (shopDescText != null) shopDescText.text = "You must reach career level 2 before the broker will deal with you. Prove yourself first.";
            if (costText     != null) costText.text     = "";
            if (incomeText   != null) incomeText.text   = "";
            if (buyButton    != null) buyButton.gameObject.SetActive (false);
            return;
        }

        // Available to purchase
        if (shopNameText != null) shopNameText.text = GetShopName ();
        if (shopDescText != null) shopDescText.text = GetShopDescription ();
        if (costText     != null) costText.text     = $"Cost: ₯{ShopCost:F0}   (You have: ₯{GameState.Instance.drachma:F0})";
        if (incomeText   != null) incomeText.text   = $"Daily income: {GetIncomeDescription ()}";
        if (buyButton    != null)
        {
            buyButton.gameObject.SetActive (true);
            buyButton.interactable = GameState.Instance.drachma >= ShopCost;
        }
    }

    private void OnBuy ()
    {
        if (GameState.Instance == null) return;
        if (GameState.Instance.hasShop) return;
        if (GameState.Instance.careerLevel < 2) return;

        if (!GameState.Instance.SpendDrachma (ShopCost))
        {
            if (feedbackText != null)
                feedbackText.text = "You do not have enough drachma.";
            return;
        }

        GameState.Instance.hasShop      = true;
        GameState.Instance.shopPurchaseDay = TimeManager.Instance != null
            ? TimeManager.Instance.GetCurrentDay () : 1;

        AudioManager.Instance?.PlayHonourGained ();
        Populate (); // Refresh the panel
    }

    // ══════════════════════════════════════════════════════════════════════
    // Shop data helpers
    // ══════════════════════════════════════════════════════════════════════

    public static string GetShopName ()
    {
        if (GameState.Instance == null) return "";
        switch (GameState.Instance.currentProfession)
        {
            case GameState.Profession.Merchant:    return "Trading Post";
            case GameState.Profession.Soldier:     return "Weapons Supplier";
            case GameState.Profession.Philosopher: return "School of Thought";
            case GameState.Profession.Craftsman:   return "Master's Workshop";
            case GameState.Profession.Priest:      return "Votive Stall";
            default:                               return "Establishment";
        }
    }

    private string GetShopDescription ()
    {
        if (GameState.Instance == null) return "";
        switch (GameState.Instance.currentProfession)
        {
            case GameState.Profession.Merchant:
                return "A trading post at the heart of the Agora. Goods move through here day and night, and your cut arrives every morning.";
            case GameState.Profession.Soldier:
                return "A supplier of arms and armour to the city's fighting men. Athens always needs weapons, and you will provide them.";
            case GameState.Profession.Philosopher:
                return "A school where students pay to hear arguments sharpened by your mind. Knowledge, it turns out, is profitable.";
            case GameState.Profession.Craftsman:
                return "A workshop in the Kerameikos bearing your mark. Commissions flow in without you needing to be present for every one.";
            case GameState.Profession.Priest:
                return "A stall selling votive offerings near the temples. The devout are generous, and the gods are always hungry.";
            default:
                return "A modest establishment that generates steady income.";
        }
    }

    private string GetIncomeDescription ()
    {
        if (GameState.Instance == null) return "";
        switch (GameState.Instance.currentProfession)
        {
            case GameState.Profession.Merchant:    return "₯15 per day";
            case GameState.Profession.Soldier:     return "₯10 per day + 2 honour";
            case GameState.Profession.Philosopher: return "₯12 per day + 5 career XP";
            case GameState.Profession.Craftsman:   return "₯15 per day";
            case GameState.Profession.Priest:      return "₯10 per day + 5 patron favour";
            default:                               return "₯10 per day";
        }
    }
}