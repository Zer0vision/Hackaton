using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Camera gameplayCamera;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gameplayCamera = gameplayCamera != null ? gameplayCamera : Camera.main;

        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void FixedUpdate()
    {
        Vector2 move = moveInput;
        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        Vector2 aimDirection = lookInput;

        if (aimDirection.sqrMagnitude <= 0.0001f && Mouse.current != null && gameplayCamera != null)
        {
            Vector3 mouseWorld = gameplayCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            aimDirection = (mouseWorld - transform.position);
        }

        if (aimDirection.sqrMagnitude > 0.0001f)
        {
            transform.up = aimDirection.normalized;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (context.canceled)
        {
            moveInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        if (context.canceled)
        {
            lookInput = Vector2.zero;
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        // Hook for rhythm validation before firing.
    }
}
