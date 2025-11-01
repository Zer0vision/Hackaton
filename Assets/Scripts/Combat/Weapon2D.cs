using UnityEngine;

public class Weapon2D : MonoBehaviour
{
    [Header("Refs")] public Bullet bulletPrefab; 
    public Transform muzzle;
    public ChaosMeter chaos;

    [Header("Tuning")] public float baseSpreadDeg = 0f;
    public float chaosSpreadMultiplier = 10f;

    public void ShootAt(Vector2 worldTarget)
    {
        if (!bulletPrefab || !muzzle) return;
        var dir = (worldTarget - (Vector2)muzzle.position).normalized;

        float spread = baseSpreadDeg + ((chaos != null ? chaos.Value : 0f) * chaosSpreadMultiplier);
        float offset = Random.Range(-spread, spread);
        dir = Quaternion.Euler(0, 0, offset) * dir;

        var bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);
        bullet.Fire(dir);
    }

    public void ShootTowardsMouse()
    {
        var mouseWorld = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ShootAt(mouseWorld);
    }
}