using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    public const int TotalCollectibles = 20;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Public API — delegates to GameState for persistence
    // ══════════════════════════════════════════════════════════════════════

    public bool IsCollected (int id)
    {
        if (GameState.Instance == null) return false;
        if (id < 0 || id >= TotalCollectibles) return false;
        return GameState.Instance.collectiblesFound[id];
    }

    public void MarkCollected (int id)
    {
        if (GameState.Instance == null) return;
        if (id < 0 || id >= TotalCollectibles) return;
        GameState.Instance.collectiblesFound[id] = true;
        Debug.Log ($"Collectible {id} collected. Total: {CountCollected ()} / {TotalCollectibles}");
    }

    public int CountCollected ()
    {
        if (GameState.Instance == null) return 0;
        int count = 0;
        foreach (bool b in GameState.Instance.collectiblesFound)
            if (b) count++;
        return count;
    }
}