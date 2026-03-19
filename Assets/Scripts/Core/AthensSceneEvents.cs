using UnityEngine;

public class AthensSceneEvents : MonoBehaviour
{
    private void Start ()
    {
        Invoke (nameof (FireFestivalNotifications), 0.5f);
    }

    private void FireFestivalNotifications ()
    {
        if (FestivalManager.Instance == null) return;

        // Check festival state now that Athens has loaded
        FestivalManager.Instance.CheckFestivalForCurrentDay ();

        if (FestivalNotification.Instance == null) return;

        // Today is a festival
        if (FestivalManager.Instance.IsFestivalDay)
        {
            FestivalNotification.Instance.ShowToday (
                FestivalManager.Instance.CurrentFestival);
            return;
        }

        // Tomorrow is a festival
        var tomorrow = FestivalManager.Instance.GetTomorrowsFestival ();
        if (tomorrow.type != FestivalManager.FestivalType.None)
            FestivalNotification.Instance.ShowTomorrow (tomorrow);
    }
}