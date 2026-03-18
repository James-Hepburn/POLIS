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
    public int homeInteriorSceneIndex = 8;

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

    private string BuildFavourSummary ()
    {
        if (GameState.Instance == null) return "";
        System.Text.StringBuilder sb = new System.Text.StringBuilder ();
        sb.AppendLine ("Divine Favour");
        sb.AppendLine ($"  Hermes      {GameState.Instance.favourHermes}");
        sb.AppendLine ($"  Ares        {GameState.Instance.favourAres}");
        sb.AppendLine ($"  Aphrodite   {GameState.Instance.favourAphrodite}");
        sb.AppendLine ($"  Apollo      {GameState.Instance.favourApollo}");
        sb.AppendLine ($"  Hephaestus  {GameState.Instance.favourHephaestus}");
        sb.AppendLine ($"  Athena      {GameState.Instance.favourAthena}");
        return sb.ToString ();
    }

    private string BuildRelationshipSummary ()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder ();
        sb.AppendLine ("Relationships");
        sb.AppendLine ($"  Nikias      {FormatRelationship (GameState.Instance.relationshipNikias)}");
        sb.AppendLine ($"  Demetrios   {FormatRelationship (GameState.Instance.relationshipDemetrios)}");
        sb.AppendLine ($"  Theron      {FormatRelationship (GameState.Instance.relationshipTheron)}");
        sb.AppendLine ($"  Argos       {FormatRelationship (GameState.Instance.relationshipArgos)}");
        sb.AppendLine ($"  Eudoros     {FormatRelationship (GameState.Instance.relationshipEudoros)}");
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