using UnityEngine;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    [Header ("UI References")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI seasonText;

    private void Update ()
    {
        if (TimeManager.Instance == null) return;
        if (!TimeManager.Instance.IsDayActive ()) return;

        timeText.text   = TimeManager.Instance.GetTimeString ();
        dayText.text    = $"Day {TimeManager.Instance.GetCurrentDay ()}";
        seasonText.text = TimeManager.Instance.GetCurrentSeason ().ToString ();
    }
}