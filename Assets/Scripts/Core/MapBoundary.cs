using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    [Header ("Map Bounds")]
    public float minX = -9f;
    public float maxX =  9f;
    public float minY = -5f;
    public float maxY =  5f;

    private Transform player;

    private void Start ()
    {
        PlayerController pc = FindFirstObjectByType<PlayerController> ();
        if (pc != null)
            player = pc.transform;
        else
            Debug.LogWarning ("MapBoundary: No player found.");
    }

    private void LateUpdate ()
    {
        Debug.Log ($"Clamping player to {minX} / {maxX}");
        if (player == null) return;

        Vector3 pos = player.position;
        pos.x = Mathf.Clamp (pos.x, minX, maxX);
        pos.y = Mathf.Clamp (pos.y, minY, maxY);
        player.position = pos;
    }
}