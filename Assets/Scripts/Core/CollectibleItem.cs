using UnityEngine;
using UnityEngine.InputSystem;

public class CollectibleItem : MonoBehaviour
{
    [Header ("Identity")]
    public int    collectibleID  = 0;
    public string itemName       = "Hermes' Coin";
    [TextArea]
    public string description    = "A coin stamped with the caduceus.";

    [Header ("Effect")]
    public CollectibleEffect effect     = CollectibleEffect.FavourHermes;
    public int               effectAmount = 10;

    [Header ("Settings")]
    public float interactionRadius = 1.2f;

    [Header ("UI")]
    public GameObject promptUI;

    // ── Internal ───────────────────────────────────────────────────────────
    private Transform player;
    private bool      playerInRange = false;

    // ══════════════════════════════════════════════════════════════════════
    public enum CollectibleEffect
    {
        FavourHermes,
        FavourAres,
        FavourAphrodite,
        FavourApollo,
        FavourHephaestus,
        FavourAthena,
        FavourPatron,
        Drachma,
        Honour,
        CareerXP,
    }

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        player = GameObject.FindWithTag ("Player")?.transform;

        // If already collected, deactivate immediately
        if (CollectibleManager.Instance != null
            && CollectibleManager.Instance.IsCollected (collectibleID))
        {
            gameObject.SetActive (false);
            return;
        }

        if (promptUI != null) promptUI.SetActive (false);
    }

    private void Update ()
    {
        if (player == null) return;

        float distance = Vector2.Distance (transform.position, player.position);
        playerInRange  = distance <= interactionRadius;

        if (promptUI != null)
            promptUI.SetActive (playerInRange);

        if (playerInRange && Keyboard.current.fKey.wasPressedThisFrame)
            Collect ();
    }

    // ══════════════════════════════════════════════════════════════════════
    private void Collect ()
    {
        if (GameState.Instance == null) return;

        // Mark as collected
        if (CollectibleManager.Instance != null)
            CollectibleManager.Instance.MarkCollected (collectibleID);

        // Apply effect
        ApplyEffect ();

        // Show pickup UI
        if (CollectibleUI.Instance != null)
            CollectibleUI.Instance.Show (itemName, description, GetEffectString ());

        // Deactivate
        gameObject.SetActive (false);
    }

    private void ApplyEffect ()
    {
        if (GameState.Instance == null) return;

        switch (effect)
        {
            case CollectibleEffect.FavourHermes:
                GameState.Instance.ChangeFavour (GameState.PatronGod.Hermes, effectAmount);
                break;
            case CollectibleEffect.FavourAres:
                GameState.Instance.ChangeFavour (GameState.PatronGod.Ares, effectAmount);
                break;
            case CollectibleEffect.FavourAphrodite:
                GameState.Instance.ChangeFavour (GameState.PatronGod.Aphrodite, effectAmount);
                break;
            case CollectibleEffect.FavourApollo:
                GameState.Instance.ChangeFavour (GameState.PatronGod.Apollo, effectAmount);
                break;
            case CollectibleEffect.FavourHephaestus:
                GameState.Instance.ChangeFavour (GameState.PatronGod.Hephaestus, effectAmount);
                break;
            case CollectibleEffect.FavourAthena:
                GameState.Instance.ChangeFavour (GameState.PatronGod.Athena, effectAmount);
                break;
            case CollectibleEffect.FavourPatron:
                GameState.Instance.ChangeFavour (GameState.Instance.patronGod, effectAmount);
                break;
            case CollectibleEffect.Drachma:
                GameState.Instance.AddDrachma (effectAmount);
                break;
            case CollectibleEffect.Honour:
                GameState.Instance.AddHonour (effectAmount);
                break;
            case CollectibleEffect.CareerXP:
                GameState.Instance.AddCareerXP (effectAmount);
                break;
        }
    }

    private string GetEffectString ()
    {
        switch (effect)
        {
            case CollectibleEffect.FavourHermes:     return $"+{effectAmount} Hermes Favour";
            case CollectibleEffect.FavourAres:       return $"+{effectAmount} Ares Favour";
            case CollectibleEffect.FavourAphrodite:  return $"+{effectAmount} Aphrodite Favour";
            case CollectibleEffect.FavourApollo:     return $"+{effectAmount} Apollo Favour";
            case CollectibleEffect.FavourHephaestus: return $"+{effectAmount} Hephaestus Favour";
            case CollectibleEffect.FavourAthena:     return $"+{effectAmount} Athena Favour";
            case CollectibleEffect.FavourPatron:     return $"+{effectAmount} Patron Favour";
            case CollectibleEffect.Drachma:          return $"+{effectAmount} Drachma";
            case CollectibleEffect.Honour:           return $"+{effectAmount} Honour";
            case CollectibleEffect.CareerXP:         return $"+{effectAmount} Career XP";
            default:                                 return "";
        }
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, interactionRadius);
    }
}