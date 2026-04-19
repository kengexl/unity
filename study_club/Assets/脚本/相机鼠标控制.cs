using UnityEngine;

public class 相机鼠标控制 : MonoBehaviour
{
    [Header("鼠标灵敏度")]
    public float 灵敏度 = 100f;
    [Header("上下视角最大角度")]
    public float 最高角度 = 70f;
    public float 最低角度 = -20f;

    [Header("第三人称相机设置")]
    public float 相机高度 = 1.2f;     // 相机高度
    public float 默认距离 = 3f;        // 默认拉远距离
    public float 最小距离 = 1f;
    public float 最大距离 = 8f;
    public float 滚轮速度 = 5f;

    // 独立保存相机的旋转角度，不依赖父物体
    float 上下旋转角度;
    float 水平旋转角度;
    float 当前距离;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        当前距离 = 默认距离;
    }

    void Update()
    {
        // ====================== 独立鼠标控制 ======================
        // 水平旋转（左右）：完全独立，不影响父物体
        float 鼠标左右 = Input.GetAxis("Mouse X") * 灵敏度 * Time.deltaTime;
        水平旋转角度 += 鼠标左右;

        // 垂直旋转（上下）
        float 鼠标上下 = Input.GetAxis("Mouse Y") * 灵敏度 * Time.deltaTime;
        上下旋转角度 -= 鼠标上下;
        上下旋转角度 = Mathf.Clamp(上下旋转角度, 最低角度, 最高角度);

        // 滚轮缩放
        float 滚轮 = Input.GetAxis("Mouse ScrollWheel");
        当前距离 -= 滚轮 * 滚轮速度;
        当前距离 = Mathf.Clamp(当前距离, 最小距离, 最大距离);
    }

    void LateUpdate()
    {
        Transform 相机 = transform.GetChild(0);

        // 相机独立旋转（关键：不跟随父物体）
        相机.rotation = Quaternion.Euler(上下旋转角度, 水平旋转角度, 0);
        
        // 相机位置：保持在父物体上方 + 后方
        相机.position = transform.position - 相机.forward * 当前距离 + Vector3.up * 相机高度;
    }
}