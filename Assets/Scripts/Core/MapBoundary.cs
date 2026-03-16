using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    // ── Boundary Settings ──────────────────────────────────────────────────
    [Header ("Map Bounds")]
    public float minX = -9f;
    public float maxX =  9f;
    public float minY = -5f;
    public float maxY =  5f;

    [Header ("References")]
    public Transform player;
    public Camera    mainCamera;

    // ── Camera half sizes ──────────────────────────────────────────────────
    private float camHalfHeight;
    private float camHalfWidth;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        camHalfHeight = mainCamera.orthographicSize;
        camHalfWidth  = camHalfHeight * mainCamera.aspect;
    }

    private void LateUpdate ()
    {
        if (player == null) return;

        // Clamp player position
        Vector3 playerPos = player.position;
        playerPos.x = Mathf.Clamp (playerPos.x, minX, maxX);
        playerPos.y = Mathf.Clamp (playerPos.y, minY, maxY);
        player.position = playerPos;

        // Clamp camera position
        Vector3 camPos = mainCamera.transform.position;
        camPos.x = Mathf.Clamp (camPos.x, minX + camHalfWidth,  maxX - camHalfWidth);
        camPos.y = Mathf.Clamp (camPos.y, minY + camHalfHeight, maxY - camHalfHeight);
        mainCamera.transform.position = camPos;
    }
}