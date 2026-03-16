using UnityEngine;
using UnityEngine.InputSystem;

public class ExitTrigger : MonoBehaviour
{
    [Header ("Settings")]
    public int athensSceneIndex = 1;
    public float interactionRadius = 1.5f;

    [Header ("UI")]
    public GameObject promptUI;

    private Transform player;
    private bool playerInRange = false;

    private void Start ()
    {
        player = GameObject.FindWithTag ("Player")?.transform;

        if (promptUI != null)
            promptUI.SetActive (false);
    }

    private void Update ()
    {
        if (player == null) return;

        float distance = Vector2.Distance (transform.position, player.position);
        playerInRange = distance <= interactionRadius;

        if (promptUI != null)
            promptUI.SetActive (playerInRange);

        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // No position saving here — just go back
            if (SceneTransition.Instance != null)
                SceneTransition.Instance.TransitionToScene (athensSceneIndex);
        }
    }
}