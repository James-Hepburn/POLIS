using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // ── Settings ───────────────────────────────────────────────────────────
    [Header ("Movement")]
    public float moveSpeed = 3f;

    [Header ("Time Cost")]
    public float minutesPerStep = 0.1f;

    [Header ("Bob Settings")]
    public float bobHeight = 0.04f;
    public float bobSpeed = 8f;

    // ── Internal ───────────────────────────────────────────────────────────
    private Rigidbody2D rb;
    private Vector2 movement;
    private Transform spriteParent;
    private Vector3 spriteBasePosition;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        PlayerController existing = FindFirstObjectByType<PlayerController> ();
        if (existing != null && existing != this)
        {
            Destroy (gameObject);
            return;
        }

        rb = GetComponent<Rigidbody2D> ();
        DontDestroyOnLoad (gameObject);
        spriteParent = transform.Find ("SpriteRoot");

        if (spriteParent != null)
            spriteBasePosition = spriteParent.localPosition;
    }

    private void Update ()
    {
        movement.x = 0f;
        movement.y = 0f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            movement.y =  1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            movement.y = -1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            movement.x = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            movement.x =  1f;

        movement = movement.normalized;

        // ── Walking bob ───────────────────────────────────────────────
        if (spriteParent != null)
        {
            if (movement.magnitude > 0)
            {
                if (Mathf.Abs (movement.x) >= Mathf.Abs (movement.y))
                {
                    // Moving horizontally — bob up and down
                    float bob = Mathf.Sin (Time.time * bobSpeed) * bobHeight;
                    spriteParent.localPosition = spriteBasePosition + new Vector3 (0, bob, 0);
                }
                else
                {
                    // Moving vertically — sway left and right
                    float sway = Mathf.Sin (Time.time * bobSpeed) * bobHeight;
                    spriteParent.localPosition = spriteBasePosition + new Vector3 (sway, 0, 0);
                }
            }
            else
            {
                spriteParent.localPosition = spriteBasePosition;
            }
        }

        // ── Advance time while moving ──────────────────────────────────
        if (movement.magnitude > 0 && TimeManager.Instance != null)
        {
            TimeManager.Instance.AdvanceTimeByMinutes (minutesPerStep * Time.deltaTime * 60f);
        }
    }

    private void FixedUpdate ()
    {
        if (TimeManager.Instance != null && !TimeManager.Instance.IsDayActive ()) return;
        rb.MovePosition (rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}