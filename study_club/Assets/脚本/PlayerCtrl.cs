using UnityEngine;

public class PlayerMoveShoot : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float upDownSpeed = 3f;

    [Header("子弹预制体 自身旋转调节")]
    [Tooltip("子弹生成时自身旋转 X")]
    public float bulletRotX;
    [Tooltip("子弹生成时自身旋转 Y")]
    public float bulletRotY;
    [Tooltip("子弹生成时自身旋转 Z")]
    public float bulletRotZ;

    [Header("射击设置")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 0.2f;
    public float bulletDestroyTime = 2f;

    private float fireCooldown;

    void Update()
    {
        MoveLogic();
        ShootLogic();
    }

    // WASD 平移 QE 上下 3D
    void MoveLogic()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        float upDir = 0;
        if (Input.GetKey(KeyCode.Q)) upDir = -1;
        if (Input.GetKey(KeyCode.E)) upDir = 1;

        Vector3 moveDir = new Vector3(h, upDir, v).normalized;
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
    }

    void ShootLogic()
    {
        fireCooldown -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && fireCooldown <= 0f)
        {
            fireCooldown = fireRate;
            SpawnBullet();
        }
    }

    void SpawnBullet()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // 1. 先在枪口生成子弹
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 2. 重点：单独修改【子弹预制体 自身旋转】
        bullet.transform.localEulerAngles = new Vector3(bulletRotX, bulletRotY, bulletRotZ);

        // 子弹飞行方向不变（依旧枪口向前）
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.velocity = firePoint.forward * bulletSpeed;
        }

        // 定时销毁
        Destroy(bullet, bulletDestroyTime);
    }
}