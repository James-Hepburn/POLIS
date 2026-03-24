using UnityEngine;

public class CollectibleBob : MonoBehaviour
{
    public float bobSpeed    = 2f;
    public float bobHeight   = 0.05f;
    public float glowSpeed   = 3f;
    public float glowMin     = 0.6f;
    public float glowMax     = 1.0f;

    private Vector3        startPos;
    private SpriteRenderer sr;

    private void Start ()
    {
        startPos = transform.position;
        sr       = GetComponent<SpriteRenderer> ();
    }

    private void Update ()
    {
        // Bob up and down
        float newY = startPos.y + Mathf.Sin (Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3 (startPos.x, newY, startPos.z);

        // Pulse opacity
        if (sr != null)
        {
            float alpha = Mathf.Lerp (glowMin, glowMax, (Mathf.Sin (Time.time * glowSpeed) + 1f) / 2f);
            Color c = sr.color;
            c.a      = alpha;
            sr.color = c;
        }
    }
}