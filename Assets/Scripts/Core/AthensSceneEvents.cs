using UnityEngine;

public class AthensSceneEvents : MonoBehaviour
{
    private void Start ()
    {
        // Evaluate life goals every time Athens loads
        if (GameState.Instance != null)
            GameState.Instance.EvaluateLifeGoals ();

        Invoke (nameof (FireFestivalNotifications), 0.5f);
    }

    private void FireFestivalNotifications ()
    {
        if (FestivalManager.Instance == null) return;

        FestivalManager.Instance.CheckFestivalForCurrentDay ();

        if (FestivalNotification.Instance == null) return;

        if (FestivalManager.Instance.IsFestivalDay)
        {
            FestivalNotification.Instance.ShowToday (
                FestivalManager.Instance.CurrentFestival);
            return;
        }

        var tomorrow = FestivalManager.Instance.GetTomorrowsFestival ();
        if (tomorrow.type != FestivalManager.FestivalType.None)
            FestivalNotification.Instance.ShowTomorrow (tomorrow);
    }
}