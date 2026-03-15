using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // ── Settings ───────────────────────────────────────────────────────────
    [Header ("Movement")]
    public float moveSpeed = 3f;

    [Header ("Time Cost")]
    public float minutesPerStep = 0.1f;

    // ── Internal ───────────────────────────────────────────────────────────
    private Rigidbody2D rb;
    private Vector2 movement;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        rb = GetComponent<Rigidbody2D> ();
    }

    private void Update ()
    {
        // Read input using new Input System
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

        // Normalise so diagonal isn't faster
        movement = movement.normalized;

        // Advance time while moving
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