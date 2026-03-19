using UnityEngine;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    [Header ("UI References")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI seasonText;
    public TextMeshProUGUI drachmaText;

    private void Update ()
    {
        if (TimeManager.Instance == null) return;
        if (!TimeManager.Instance.IsDayActive ()) return;

        timeText.text   = TimeManager.Instance.GetTimeString ();
        dayText.text    = $"Day {TimeManager.Instance.GetCurrentDay ()}";

        if (FestivalManager.Instance != null && FestivalManager.Instance.IsFestivalDay)
            seasonText.text = $"{FestivalManager.Instance.CurrentFestival.displayName}";
        else
            seasonText.text = TimeManager.Instance.GetCurrentSeason ().ToString ();

        if (drachmaText != null && GameState.Instance != null)
            drachmaText.text = $"₯ {GameState.Instance.drachma:F0}";
    }
}