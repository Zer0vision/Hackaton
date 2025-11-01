using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Camera cam;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void FixedUpdate()
    {
        if (PauseManager.IsPaused) return; // ← стопим движения при паузе
        Vector2 move = ReadMove();
        if (move.sqrMagnitude > 1f) move.Normalize();
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    void Update()
    {
        if (PauseManager.IsPaused) return; // ← стопим движения при паузе
        if (cam == null || Mouse.current == null) return;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = (mouseWorld - transform.position);
        if (dir.sqrMagnitude > 0.0001f) transform.up = dir.normalized;
    }

    private Vector2 ReadMove()
    {
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        // Клавиатура WASD
        float x = 0f, y = 0f;
        if (kb != null)
        {
            if (kb.aKey.isPressed) x -= 1f;
            if (kb.dKey.isPressed) x += 1f;
            if (kb.sKey.isPressed) y -= 1f;
            if (kb.wKey.isPressed) y += 1f;
        }

        // Геймпад (если подключён)
        if (gp != null)
        {
            Vector2 stick = gp.leftStick.ReadValue();
            if (stick.sqrMagnitude > 0.0001f)
            {
                x = stick.x;
                y = stick.y;
            }
        }

        return new Vector2(x, y);
    }
}
