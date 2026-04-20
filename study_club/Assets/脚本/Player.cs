using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("基础设置")]
    public Vector2 _V2 = new Vector2(0, 0);
    public float 基础速度 = 10f; // 改名为基础速度，更准确
    public float 跳跃力度 = 7f;
    public float 转向平滑 = 8f;

    [Header("加速设置")]
    public KeyCode 加速键 = KeyCode.LeftShift; // 默认左Shift
    public float 加速倍率 = 2.0f; // 加速时的速度倍数
    private float 当前速度; // 内部私有变量，用于实时计算

    [Header("动画")]
    public Animator _aim;
    public bool 是否处于地面 = false;

    [Header("地面检测 (球形)")]
    public float 检测半径 = 0.3f;
    public float 检测偏移距离 = 0.1f;
    public LayerMask 地面层级;

    private Rigidbody 刚体;
    private Camera 主相机;
    private bool 上一帧是否在地面;

    void Start()
    {
        _aim = GetComponent<Animator>();
        刚体 = GetComponent<Rigidbody>();
        刚体.freezeRotation = true;
        刚体.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        主相机 = Camera.main;
        当前速度 = 基础速度; // 初始化速度
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        _V2 = new Vector2(h, v).normalized;

        检测地面状态();
        处理跳跃();
        更新动画();
    }

    void FixedUpdate()
    {
        // 【新增】每帧更新速度：如果按住加速键则提速
        if (Input.GetKey(加速键))
        {
            当前速度 = 基础速度 * 加速倍率;
        }
        else
        {
            当前速度 = 基础速度;
        }

        if (_V2.magnitude > 0.1f)
        {
            八方向移动();
            角色看向移动方向();
        }
        else
        {
            角色面朝相机();
        }
    }

    void 八方向移动()
    {
        Vector3 相机前 = Vector3.Scale(主相机.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 相机右 = Vector3.Scale(主相机.transform.right, new Vector3(1, 0, 1)).normalized;

        Vector3 移动方向 = (相机前 * _V2.y + 相机右 * _V2.x).normalized;
        // 【修改】这里使用动态的 当前速度 而不是固定的 速度
        Vector3 目标速度 = new Vector3(移动方向.x * 当前速度, 刚体.velocity.y, 移动方向.z * 当前速度);
        刚体.velocity = 目标速度;
    }

    void 角色看向移动方向()
    {
        Vector3 相机前 = Vector3.Scale(主相机.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 相机右 = Vector3.Scale(主相机.transform.right, new Vector3(1, 0, 1)).normalized;
        Vector3 移动方向 = (相机前 * _V2.y + 相机右 * _V2.x);

        Quaternion 目标角度 = Quaternion.LookRotation(移动方向);
        transform.rotation = Quaternion.Lerp(transform.rotation, 目标角度, 转向平滑 * Time.fixedDeltaTime);
    }

    void 角色面朝相机()
    {
        Vector3 相机视角前 = Vector3.Scale(主相机.transform.forward, new Vector3(1, 0, 1)).normalized;
        Quaternion 目标角度 = Quaternion.LookRotation(相机视角前);
        transform.rotation = Quaternion.Lerp(transform.rotation, 目标角度, 转向平滑 * Time.fixedDeltaTime);
    }

    void 处理跳跃()
    {
        if (Input.GetButtonDown("Jump") && 是否处于地面)
        {
            刚体.velocity = new Vector3(刚体.velocity.x, 0, 刚体.velocity.z);
            刚体.AddForce(Vector3.up * 跳跃力度, ForceMode.VelocityChange);
        }
    }

    void 更新动画()
    {
        _aim.SetBool("ISjump", !是否处于地面);
        _aim.SetBool("ISrun", _V2.magnitude > 0.1f);
        
        // 【可选】如果你想让动画也区分走和跑，可以添加下面这行
        // 假设你有一个 "Speed" 参数来控制动画混合树
        // _aim.SetFloat("Speed", 当前速度);
    }

    void 检测地面状态()
    {
        Vector3 检测起点 = transform.position + Vector3.down * (检测偏移距离 - 检测半径);
        是否处于地面 = Physics.CheckSphere(检测起点, 检测半径, 地面层级);
        上一帧是否在地面 = 是否处于地面;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 检测起点 = transform.position + Vector3.down * (检测偏移距离 - 检测半径);
        Gizmos.DrawWireSphere(检测起点, 检测半径);
    }
}