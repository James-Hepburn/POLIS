using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class EndOfDaySummary : MonoBehaviour
{
    [Header ("UI References")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI seasonText;
    public TextMeshProUGUI drachmaText;
    public TextMeshProUGUI honourText;
    public TextMeshProUGUI careerText;
    public TextMeshProUGUI relationshipsText;
    public TextMeshProUGUI continuePrompt;
    public TextMeshProUGUI favourText;
    public TextMeshProUGUI eventsText;

    [Header ("Settings")]
    public int homeInteriorSceneIndex = 4;

    private bool ready = false;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        if (continuePrompt != null)
            continuePrompt.text = "";

        PopulateSummary ();
        Invoke ("ShowContinuePrompt", 1.5f);
    }

    private void Update ()
    {
        if (ready && Keyboard.current.eKey.wasPressedThisFrame)
            ContinueToNextDay ();
    }

    // ══════════════════════════════════════════════════════════════════════
    private void PopulateSummary ()
    {
        if (TimeManager.Instance != null)
        {
            dayText.text    = $"End of Day {GameState.Instance.lastCompletedDay}";
            seasonText.text = TimeManager.Instance.GetCurrentSeason ().ToString ();
        }

        if (GameState.Instance != null)
        {
            drachmaText.text = $"Drachma: ₯ {GameState.Instance.drachma:F0}";
            honourText.text  = $"Honour: {GameState.Instance.honour}";
            careerText.text  = $"Career: Level {GameState.Instance.careerLevel}  ({GameState.Instance.careerXP} / 100 XP)";
            
            if (favourText != null)
                favourText.text = BuildFavourSummary ();

            if (relationshipsText != null)
                relationshipsText.text = BuildRelationshipSummary ();

            if (eventsText != null)
                eventsText.text = BuildEventsSummary ();
        }
    }

    // Forces the value to always start at the same horizontal position
    // regardless of name length. Adjust posX to match your font size.
    private string Row (string name, object value, int posX = 160)
    {
        return $"  {name}<pos={posX}>{value}";
    }

    private string BuildFavourSummary ()
    {
        if (GameState.Instance == null) return "";
        System.Text.StringBuilder sb = new System.Text.StringBuilder ();
        sb.AppendLine ("Divine Favour");
        sb.AppendLine (Row ("Hermes",     GameState.Instance.favourHermes));
        sb.AppendLine (Row ("Ares",       GameState.Instance.favourAres));
        sb.AppendLine (Row ("Aphrodite",  GameState.Instance.favourAphrodite));
        sb.AppendLine (Row ("Apollo",     GameState.Instance.favourApollo));
        sb.AppendLine (Row ("Hephaestus", GameState.Instance.favourHephaestus));
        sb.AppendLine (Row ("Athena",     GameState.Instance.favourAthena));
        return sb.ToString ();
    }

    private string BuildRelationshipSummary ()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder ();
        sb.AppendLine ("Relationships");
        sb.AppendLine (Row ("Nikias",    FormatRelationship (GameState.Instance.relationshipNikias)));
        sb.AppendLine (Row ("Demetrios", FormatRelationship (GameState.Instance.relationshipDemetrios)));
        sb.AppendLine (Row ("Theron",    FormatRelationship (GameState.Instance.relationshipTheron)));
        sb.AppendLine (Row ("Argos",     FormatRelationship (GameState.Instance.relationshipArgos)));
        sb.AppendLine (Row ("Eudoros",   FormatRelationship (GameState.Instance.relationshipEudoros)));
        sb.AppendLine (Row ("Chloe",     FormatRelationship (GameState.Instance.relationshipChloe)));
        sb.AppendLine (Row ("Kallias",   FormatRelationship (GameState.Instance.relationshipKallias)));
        sb.AppendLine (Row ("Lydia",     FormatRelationship (GameState.Instance.relationshipLydia)));
        sb.AppendLine (Row ("Miriam",    FormatRelationship (GameState.Instance.relationshipMiriam)));
        sb.AppendLine (Row ("Phaedra",   FormatRelationship (GameState.Instance.relationshipPhaedra)));
        sb.AppendLine (Row ("Stephanos", FormatRelationship (GameState.Instance.relationshipStephanos)));
        sb.AppendLine (Row ("Xanthos",   FormatRelationship (GameState.Instance.relationshipXanthos)));
        return sb.ToString ();
    }

    private string BuildEventsSummary ()
    {
        if (GameState.Instance == null) return "";
        if (GameState.Instance.pendingEndOfDayEvents == null
            || GameState.Instance.pendingEndOfDayEvents.Count == 0)
            return "";

        System.Text.StringBuilder sb = new System.Text.StringBuilder ();
        sb.AppendLine ("The Gods Speak");
        foreach (string evt in GameState.Instance.pendingEndOfDayEvents)
            sb.AppendLine ($"  {evt}");
        return sb.ToString ();
    }

    private string FormatRelationship (int value)
    {
        if (value >= 80)  return $"{value}  [Close]";
        if (value >= 40)  return $"{value}  [Friendly]";
        if (value >= 1)   return $"{value}  [Neutral]";
        if (value == 0)   return $"{value}  ";
        return $"{value}  [Hostile]";
    }

    private void ShowContinuePrompt ()
    {
        if (continuePrompt != null)
            continuePrompt.text = "[E] Sleep and continue to the next day";
        ready = true;
    }

    private void ContinueToNextDay ()
    {
        SceneManager.LoadScene (homeInteriorSceneIndex);
    }
}