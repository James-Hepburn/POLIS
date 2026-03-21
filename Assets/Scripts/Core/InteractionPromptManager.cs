using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionPromptManager : MonoBehaviour
{
    public static InteractionPromptManager Instance { get; private set; }

    private readonly List<IInteractable> _registered = new List<IInteractable> ();
    private Transform _player;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);
    }

    private void OnEnable ()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable ()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        RefreshPlayer ();
    }

    private void Update ()
    {
        if (_player == null || _registered.Count == 0) return;

        IInteractable nearest  = null;
        float         bestDist = float.MaxValue;

        foreach (var interactable in _registered)
        {
            if (!interactable.IsEligible) continue;

            float dist = Vector2.Distance (_player.position, interactable.WorldPosition);
            if (dist < bestDist)
            {
                bestDist = dist;
                nearest  = interactable;
            }
        }

        foreach (var interactable in _registered)
        {
            interactable.ShowPrompt (interactable == nearest && interactable.IsEligible);
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    public void Register (IInteractable i)
    {
        if (!_registered.Contains (i)) _registered.Add (i);
    }

    public void Unregister (IInteractable i)
    {
        _registered.Remove (i);
        i.ShowPrompt (false);
    }

    public void RefreshPlayer ()
    {
        GameObject p = GameObject.FindWithTag ("Player");
        if (p != null) _player = p.transform;
    }
}