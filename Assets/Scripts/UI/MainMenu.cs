using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header ("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI taglineText;
    public Button          newGameButton;
    public Button          continueButton;
    public TextMeshProUGUI continueButtonText;

    [Header ("Confirmation Panel")]
    public GameObject      confirmPanel;
    public TextMeshProUGUI confirmText;
    public Button          confirmYesButton;
    public Button          confirmNoButton;

    [Header ("Scene Indices")]
    public int characterCreationSceneIndex = 2;
    public int homeInteriorSceneIndex      = 4;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        if (titleText   != null) titleText.text   = "POLIS";
        if (taglineText != null) taglineText.text = "Live a life. Build a legacy. Appease the gods.";

        newGameButton.onClick.AddListener  (OnNewGameClicked);
        continueButton.onClick.AddListener (OnContinueClicked);
        confirmYesButton.onClick.AddListener (OnConfirmNewGame);
        confirmNoButton.onClick.AddListener  (OnCancelNewGame);

        if (confirmPanel != null) confirmPanel.SetActive (false);

        bool saveExists = GameState.SaveExists ();
        continueButton.interactable = saveExists;
        if (continueButtonText != null)
            continueButtonText.color = saveExists ? Color.white : new Color (1f, 1f, 1f, 0.4f);
    }

    // ══════════════════════════════════════════════════════════════════════
    private void OnNewGameClicked ()
    {
        if (GameState.SaveExists ())
        {
            if (confirmPanel != null)
            {
                confirmPanel.SetActive (true);
                if (confirmText != null)
                    confirmText.text = "A save already exists.\nStarting a new game will overwrite it.\n\nAre you sure?";
            }
        }
        else
        {
            StartNewGame ();
        }
    }

    private void OnContinueClicked ()
    {
        if (GameState.Instance == null) return;

        if (GameState.Instance.Load ())
            SceneTransition.Instance.TransitionToScene (homeInteriorSceneIndex);
        else
            Debug.LogError ("Failed to load save.");
    }

    private void OnConfirmNewGame ()
    {
        if (confirmPanel != null) confirmPanel.SetActive (false);
        GameState.DeleteSave ();
        StartNewGame ();
    }

    private void OnCancelNewGame ()
    {
        if (confirmPanel != null) confirmPanel.SetActive (false);
    }

    private void StartNewGame ()
    {
        SceneTransition.Instance.TransitionToScene (characterCreationSceneIndex);
    }
}