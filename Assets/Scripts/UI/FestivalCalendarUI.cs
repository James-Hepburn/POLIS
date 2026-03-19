using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class FestivalCalendarUI : MonoBehaviour
{
    [Header ("UI")]
    public GameObject      calendarPanel;
    public TextMeshProUGUI calendarText;
    public TextMeshProUGUI hintText;

    private bool isOpen = false;

    private void Start ()
    {
        if (calendarPanel != null) calendarPanel.SetActive (false);
        if (hintText != null)      hintText.text = "[C] Calendar";
    }

    private void Update ()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
            Toggle ();
    }

    private void Toggle ()
    {
        isOpen = !isOpen;
        if (calendarPanel != null)
            calendarPanel.SetActive (isOpen);

        if (isOpen)
            PopulateCalendar ();
    }

    private void PopulateCalendar ()
    {
        if (FestivalManager.Instance == null || calendarText == null) return;

        System.Text.StringBuilder sb = new System.Text.StringBuilder ();
        sb.AppendLine ("Festival Calendar");
        sb.AppendLine ("");

        TimeManager.Season lastSeason = (TimeManager.Season)(-1);

        foreach (FestivalManager.FestivalData f in FestivalManager.Instance.GetAllFestivals ())
        {
            if (f.season != lastSeason)
            {
                sb.AppendLine ($"── {f.season} ──");
                lastSeason = f.season;
            }

            bool isToday = FestivalManager.Instance.IsFestivalDay
                        && FestivalManager.Instance.CurrentFestival.type == f.type;

            string marker = isToday ? "  ★ " : "    ";
            sb.AppendLine ($"{marker}Day {f.dayOfSeason}  {f.displayName}");
        }

        sb.AppendLine ("");
        sb.AppendLine ("[C] Close");

        calendarText.text = sb.ToString ();
    }
}