using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    [Header ("Map Bounds")]
    public float minX = -9f;
    public float maxX =  9f;
    public float minY = -5f;
    public float maxY =  5f;

    private Transform player;
    private Camera mainCamera;
    private float camHalfHeight;
    private float camHalfWidth;

    private void Start ()
    {
        mainCamera = Camera.main;
        camHalfHeight = mainCamera.orthographicSize;
        camHalfWidth  = camHalfHeight * mainCamera.aspect;

        PlayerController pc = FindFirstObjectByType<PlayerController> ();
        if (pc != null)
            player = pc.transform;
        else
            Debug.LogWarning ("MapBoundary: No player found.");
    }

    private void LateUpdate ()
    {
        if (player == null) return;

        // Clamp player
        Vector3 pos = player.position;
        pos.x = Mathf.Clamp (pos.x, minX, maxX);
        pos.y = Mathf.Clamp (pos.y, minY, maxY);
        player.position = pos;

        // Only clamp camera if it has a Cinemachine Brain (i.e. Athens)
        if (mainCamera.GetComponent<Unity.Cinemachine.CinemachineBrain> () == null) return;

        Vector3 camPos = mainCamera.transform.position;
        camPos.x = Mathf.Clamp (camPos.x, minX + camHalfWidth,  maxX - camHalfWidth);
        camPos.y = Mathf.Clamp (camPos.y, minY + camHalfHeight, maxY - camHalfHeight);
        mainCamera.transform.position = camPos;
    }
}