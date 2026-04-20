using UnityEngine;

public class ObjectLiftMove : MonoBehaviour
{
    [Header("移动设置")]
    public Vector3 endPos;       // 最高点坐标
    public float moveSpeed = 3f; // 上升速度
    public KeyCode startKey = KeyCode.T;

    [Header("状态")]
    public bool isMoving = false;

    private Vector3 startPos;

    void Start()
    {
        // 自动记录初始底部位置
        startPos = transform.position;
    }

    void Update()
    {
        // 按键开始上升
        if (Input.GetKeyDown(startKey) && !isMoving)
        {
            isMoving = true;
        }

        // 直线平滑上升
        if (isMoving)
        {
            MoveUp();
        }
    }

    void MoveUp()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, 
            endPos, 
            moveSpeed * Time.deltaTime
        );

        // 到达终点 停止移动
        if (Vector3.Distance(transform.position, endPos) < 0.05f)
        {
            isMoving = false;
            OnArriveTop();
        }
    }

    // 到达最高点执行事件（用来放爆炸烟花）
    void OnArriveTop()
    {
        Debug.Log("到达最高点，准备爆炸");
        // 这里后面直接对接你的爆炸粒子
    }

    // 【可选】一键重置回底部
    public void ResetPosition()
    {
        transform.position = startPos;
        isMoving = false;
    }
}