using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float 速度 = 1f;
    public float 跳跃力度 = 5f;
    public bool 是否处于地面 = false;
    public float 左右方向 = 0f;
    public float 斜着走的角度 = 30;
    public Animator _aim;

    // 地面检测参数
    public Vector3 地面检测偏移 = new Vector3(0, -0.4f, 0);
    public Vector3 地面检测盒大小 = new Vector3(0.4f, 0.15f, 0.4f);
    public LayerMask 地面层级;

    private Rigidbody 刚体;

    void Start()
    {
        _aim = GetComponent<Animator>();
        刚体 = GetComponent<Rigidbody>();
        // 锁定刚体旋转 防止歪倒
        刚体.freezeRotation = true;
    }

    void Update()
    {
        // Update 做输入+地面检测 最丝滑
        检测地面状态();
        
        // 【关键】跳跃放Update 用GetButtonDown 只触发一次
        if (Input.GetButtonDown("Jump") && 是否处于地面)
        {
            _aim.SetBool("ISjump", true);
            刚体.velocity = new Vector3(刚体.velocity.x, 跳跃力度, 刚体.velocity.z);
            
        }

        // 落地立刻关闭跳跃动画
        if (是否处于地面)
        {
            _aim.SetBool("ISjump", false);
        }
        else
        {
            _aim.SetBool("ISjump", true);
        }
    }

    void FixedUpdate()
    {
        左右方向 = Input.GetAxis("Horizontal");
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 跑步动画
        if (v == 1 || v == -1)
        {
            _aim.SetBool("ISrun", true);
        }
        else
        {
            _aim.SetBool("ISrun", false);
        }

        // 左右动画
        if (左右方向 > 0.1)
        {
            _aim.SetBool("ISright", true);
            _aim.SetBool("ISleft", false);
        }
        else if (左右方向 < -0.1)
        {
            _aim.SetBool("ISleft", true);
            _aim.SetBool("ISright", false);
        }
        else
        {
            _aim.SetBool("ISleft", false);
            _aim.SetBool("ISright", false);
        }

        // 原有移动 完全保留
        transform.Translate(h * 速度 * 0.01f, 0, v * 速度 * 0.01f);
        transform.eulerAngles = new Vector3(0, 左右方向 * 斜着走的角度, 0);
    }

    // 极速Box地面检测 性能高 不卡
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