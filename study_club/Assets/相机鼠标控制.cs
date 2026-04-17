using UnityEngine;

public class 相机鼠标控制 : MonoBehaviour
{
    [Header("鼠标灵敏度")]
    public float 灵敏度 = 100f;
    [Header("上下视角最大角度")]
    public float 最高角度 = 60f;
    public float 最低角度 = -60f;

    float 上下旋转角度;

    void Start()
    {
        // 锁定鼠标到窗口中心
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 获取鼠标移动
        float 鼠标左右 = Input.GetAxis("Mouse X") * 灵敏度 * Time.deltaTime;
        float 鼠标上下 = Input.GetAxis("Mouse Y") * 灵敏度 * Time.deltaTime;

        // 左右旋转：控制当前空物体Y轴（水平转头）
        transform.Rotate(0, 鼠标左右, 0);

        // 上下旋转：单独限制角度，防止看天看地穿模
        上下旋转角度 -= 鼠标上下;
        上下旋转角度 = Mathf.Clamp(上下旋转角度, 最低角度, 最高角度);
        
        // 相机子物体上下抬头低头
        transform.GetChild(0).localEulerAngles = new Vector3(上下旋转角度, 0, 0);
    }
}