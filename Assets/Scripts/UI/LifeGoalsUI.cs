using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LifeGoalsUI : MonoBehaviour
{
    [Header ("UI")]
    public GameObject      goalsPanel;
    public TextMeshProUGUI goalsText;
    public TextMeshProUGUI hintText;

    private bool isOpen = false;

    private void Start ()
    {
        if (goalsPanel != null) goalsPanel.SetActive (false);
        if (hintText   != null) hintText.text = "[G] Goals";
    }

    private void Update ()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
            Toggle ();
    }

    private void Toggle ()
    {
        isOpen = !isOpen;
        if (goalsPanel != null)
            goalsPanel.SetActive (isOpen);

        if (isOpen)
            PopulateGoals ();
    }

    private void PopulateGoals ()
    {
        if (GameState.Instance == null || goalsText == null) return;

        System.Text.StringBuilder sb = new System.Text.StringBuilder ();
        sb.AppendLine ("Life Goals");
        sb.AppendLine ("");

        sb.AppendLine (FormatGoal (
            GameState.Instance.GetCareerGoalName (),
            GameState.Instance.goalCareerComplete));

        sb.AppendLine (FormatGoal (
            "Marry and build a life together",
            GameState.Instance.goalMarriageComplete));

        sb.AppendLine (FormatGoal (
            "Accumulate 500 drachma",
            GameState.Instance.goalWealthComplete,
            $"₯ {GameState.Instance.drachma:F0} / 500"));

        sb.AppendLine (FormatGoal (
            $"Reach +80 favour with {GameState.Instance.patronGod}",
            GameState.Instance.goalFavourComplete,
            $"{GameState.Instance.GetFavour (GameState.Instance.patronGod)} / 80"));

        sb.AppendLine (FormatGoal (
            "Achieve 80 Honour",
            GameState.Instance.goalHonourComplete,
            $"{GameState.Instance.honour} / 80"));

        sb.AppendLine (FormatGoal (
            "Have 5 close friends (60+ relationship)",
            GameState.Instance.goalFriendshipComplete,
            $"{CountCloseRelationships ()} / 5"));

        sb.AppendLine ("");
        sb.AppendLine ($"Completed: {GameState.Instance.GoalsCompleted} / 6");
        sb.AppendLine ("");
        sb.AppendLine ("[G] Close");

        goalsText.text = sb.ToString ();
    }

    private string FormatGoal (string name, bool complete, string progress = "")
    {
        string status = complete ? "✓" : "○";
        string prog   = (!complete && !string.IsNullOrEmpty (progress)) ? $"  ({progress})" : "";
        return $"  {status}  {name}{prog}";
    }

    private int CountCloseRelationships ()
    {
        if (GameState.Instance == null) return 0;
        int count = 0;
        int[] rels = {
            GameState.Instance.relationshipNikias,   GameState.Instance.relationshipDemetrios,
            GameState.Instance.relationshipTheron,   GameState.Instance.relationshipArgos,
            GameState.Instance.relationshipEudoros,  GameState.Instance.relationshipChloe,
            GameState.Instance.relationshipKallias,  GameState.Instance.relationshipLydia,
            GameState.Instance.relationshipMiriam,   GameState.Instance.relationshipPhaedra,
            GameState.Instance.relationshipStephanos,GameState.Instance.relationshipXanthos
        };
        foreach (int r in rels)
            if (r >= 60) count++;
        return count;
    }
}