using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("基础设置")]
    public Vector2 _V2 = new Vector2(0, 0);
    public float 速度 = 5f;
    public float 跳跃力度 = 5f;
    public float 转向平滑 = 8f;

    [Header("动画")]
    public Animator _aim;
    public bool 是否处于地面 = false;

    [Header("地面检测")]
    public Vector3 地面检测偏移 = new Vector3(0, -0.4f, 0);
    public Vector3 地面检测盒大小 = new Vector3(0.4f, 0.15f, 0.4f);
    public LayerMask 地面层级;

    private Rigidbody 刚体;
    private Camera 主相机;

    void Start()
    {
        _aim = GetComponent<Animator>();
        刚体 = GetComponent<Rigidbody>();
        刚体.freezeRotation = true;
        主相机 = Camera.main;
    }

    void Update()
    {
        // 八方向输入 存入 _V2
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        _V2 = new Vector2(h, v).normalized;

        检测地面状态();
        处理跳跃();
        更新动画();
    }

    void FixedUpdate()
    {
        if (_V2.magnitude > 0.1f)
        {
            八方向移动();
            角色看向移动方向();
        }
        else
        {
            // 静止：角色面朝相机正前方
            角色面朝相机();
        }
    }

    // 八方向移动 配合相机
    void 八方向移动()
    {
        Vector3 相机前 = Vector3.Scale(主相机.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 相机右 = Vector3.Scale(主相机.transform.right, new Vector3(1, 0, 1)).normalized;

        Vector3 移动方向 = (相机前 * _V2.y + 相机右 * _V2.x).normalized;
        Vector3 目标速度 = new Vector3(移动方向.x * 速度, 刚体.velocity.y, 移动方向.z * 速度);
        刚体.velocity = 目标速度;
    }

    // 移动时看向移动方向
    void 角色看向移动方向()
    {
        Vector3 相机前 = Vector3.Scale(主相机.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 相机右 = Vector3.Scale(主相机.transform.right, new Vector3(1, 0, 1)).normalized;
        Vector3 移动方向 = (相机前 * _V2.y + 相机右 * _V2.x);

        Quaternion 目标角度 = Quaternion.LookRotation(移动方向);
        transform.rotation = Quaternion.Lerp(transform.rotation, 目标角度, 转向平滑 * Time.fixedDeltaTime);
    }

    // 静止时：面朝相机
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
            刚体.velocity = new Vector3(刚体.velocity.x, 跳跃力度, 刚体.velocity.z);
        }
    }

    void 更新动画()
    {
        _aim.SetBool("ISjump", !是否处于地面);
        _aim.SetBool("ISrun", _V2.magnitude > 0.1f);
    }

    void 检测地面状态()
    {
        Vector3 检测点 = transform.TransformPoint(地面检测偏移);
        是否处于地面 = Physics.CheckBox(检测点, 地面检测盒大小 * 0.5f, transform.rotation, 地面层级);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 检测点 = transform.TransformPoint(地面检测偏移);
        Gizmos.DrawWireCube(检测点, 地面检测盒大小);
    }
}