using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSetup : MonoBehaviour
{
    [Header ("Player Setup")]
    public Vector3 playerSpawnPosition = Vector3.zero;
    public Vector3 playerScale = new Vector3 (1.5f, 1.5f, 1f);

    [Header ("Player Visibility")]
    public bool hidePlayer = false;

    private void Start ()
    {
        // Find the persistent player
        PlayerController player = FindFirstObjectByType<PlayerController> ();
        if (player == null) return;

        // Set position and scale for this scene
        player.transform.position = playerSpawnPosition;
        player.transform.localScale = playerScale;

        // Reapply appearance in case it needs refreshing
        PlayerAppearance appearance = player.GetComponent<PlayerAppearance> ();
        if (appearance != null)
            appearance.ApplyAppearance ();

        // Hide or show player sprites
        SpriteRenderer[] renderers = player.GetComponentsInChildren<SpriteRenderer> ();
        foreach (SpriteRenderer sr in renderers)
            sr.enabled = !hidePlayer;
    }
}