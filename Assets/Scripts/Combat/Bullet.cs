using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [Header("Tuning")] public float speed = 16f; 
    public float lifeTime = 2f; 
    public int damage = 1;

    private Rigidbody2D _rb;

    void Awake() { _rb = GetComponent<Rigidbody2D>(); }

    public void Fire(Vector2 direction)
    {
        _rb.velocity = direction.normalized * speed;
        if (lifeTime > 0f) Invoke(nameof(SelfDestruct), lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // TODO: apply damage to enemy / walls as per your project
        Destroy(gameObject);
    }

    void SelfDestruct() => Destroy(gameObject);
}