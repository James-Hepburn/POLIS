using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class BuildingEntrance : MonoBehaviour
{
    [Header ("Building Settings")]
    public string buildingName = "Home";
    public int    interiorSceneIndex = 2;
    public float  interactionRadius  = 1.5f;

    [Header ("UI")]
    public GameObject promptUI;

    private Transform player;
    private bool      playerInRange = false;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        player = GameObject.FindWithTag ("Player")?.transform;

        if (promptUI != null)
            promptUI.SetActive (false);
    }

    private void Update ()
    {
        if (player == null)
        {
            Debug.LogError ("Player reference is null!");
            return;
        }

        float distance = Vector2.Distance (transform.position, player.position);
        playerInRange = distance <= interactionRadius;

        if (promptUI != null)
            promptUI.SetActive (playerInRange);

        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log ("E pressed — entering building");
            EnterBuilding ();
        }
    }

    private void EnterBuilding ()
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.TransitionToScene (interiorSceneIndex);
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, interactionRadius);
    }
}