using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("地面移动")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 2f;

    [Header("鼠标视角")]
    public float mouseSensitivity = 200f;
    public float minYAngle = -80f;
    public float maxYAngle = 80f;

    [Header("浮空设置")]
    public float floatBaseSpeed = 6f;     // 浮空基础速度
    public float floatUpDownSpeed = 3f;   // 上升下降速度
    public float speedMultiplier = 2f;    // Shift加速倍数
    public float slowMultiplier = 0.3f;   // Ctrl减速倍数

    [Header("重力")]
    public float gravity = -9.81f;

    private CharacterController controller;
    private Transform cameraTransform;

    private Vector3 velocity;
    private bool isGrounded;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private float lastSpaceTime = 0f;
    private float doubleClickTime = 0.3f;
    private bool isFloating = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        // 视角
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minYAngle, maxYAngle);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // 移动方向
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = transform.right * x + transform.forward * z;
        moveDir.Normalize();

        if (isFloating)
        {
            // 浮空水平速度：Shift加速 / Ctrl减速
            float currentFloatSpeed = floatBaseSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
                currentFloatSpeed = floatBaseSpeed * speedMultiplier;
            else if (Input.GetKey(KeyCode.LeftControl))
                currentFloatSpeed = floatBaseSpeed * slowMultiplier;

            controller.Move(moveDir * currentFloatSpeed * Time.deltaTime);

            // 浮空垂直：Space上升 / Ctrl下降
            float vertical = 0f;
            if (Input.GetKey(KeyCode.Space))
                vertical = floatUpDownSpeed;

            velocity.y = vertical;
        }
        else
        {
            // 地面移动
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            controller.Move(moveDir * currentSpeed * Time.deltaTime);

            // 地面跳跃与重力
            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            velocity.y += gravity * Time.deltaTime;
        }

        // 应用垂直运动
        controller.Move(velocity * Time.deltaTime);

        // 双击空格切换浮空
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time - lastSpaceTime < doubleClickTime)
            {
                isFloating = !isFloating;
                velocity.y = 0;
            }
            lastSpaceTime = Time.time;
        }
    }
}