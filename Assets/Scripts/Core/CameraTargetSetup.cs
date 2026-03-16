using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraTargetSetup : MonoBehaviour
{
    private void Start ()
    {
        StartCoroutine (AssignTarget ());
    }

    private IEnumerator AssignTarget ()
    {
        // Wait until the player exists (arrives from DontDestroyOnLoad)
        PlayerController player = null;
        while (player == null)
        {
            player = FindFirstObjectByType<PlayerController> ();
            yield return null;
        }

        CinemachineCamera vcam = GetComponent<CinemachineCamera> ();
        if (vcam != null)
        {
            vcam.Target.TrackingTarget = player.transform;
            Debug.Log ("CameraTargetSetup: target assigned.");
        }
    }
}