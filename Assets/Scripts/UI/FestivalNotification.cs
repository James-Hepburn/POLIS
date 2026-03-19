using UnityEngine;
using TMPro;
using System.Collections;

public class FestivalNotification : MonoBehaviour
{
    public static FestivalNotification Instance { get; private set; }

    [Header ("UI")]
    public GameObject      notificationPanel;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI bodyText;

    [Header ("Settings")]
    public float displayDuration = 5f;

    private void Awake ()
    {
        Instance = this;
        if (notificationPanel != null)
            notificationPanel.SetActive (false);
    }

    public void ShowTomorrow (FestivalManager.FestivalData festival)
    {
        Show (
            "Festival Tomorrow",
            $"Tomorrow is the {festival.displayName}.\n{festival.description}"
        );
    }

    public void ShowTomorrowDelayed (FestivalManager.FestivalData festival, float delay)
    {
        StartCoroutine (DelayedTomorrow (festival, delay));
    }

    private IEnumerator DelayedTomorrow (FestivalManager.FestivalData festival, float delay)
    {
        yield return new WaitForSeconds (delay);
        ShowTomorrow (festival);
    }

    public void ShowToday (FestivalManager.FestivalData festival)
    {
        Show (
            festival.displayName,
            festival.description
        );
    }

    private void Show (string header, string body)
    {
        if (notificationPanel == null) return;
        StopAllCoroutines ();
        headerText.text = header;
        bodyText.text   = body;
        notificationPanel.SetActive (true);
        StartCoroutine (HideAfterDelay ());
    }

    private IEnumerator HideAfterDelay ()
    {
        yield return new WaitForSeconds (displayDuration);
        if (notificationPanel != null)
            notificationPanel.SetActive (false);
    }
}