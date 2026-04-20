using UnityEngine;

public class FireworkNodeControl : MonoBehaviour
{
    [Header("拖拽子节点粒子")]
    public ParticleSystem upParticle;
    public ParticleSystem explodeParticle;

    [Header("上升参数")]
    public float maxHeight = 15f;
    public float riseSpeed = 4f;
    public KeyCode fireKey = KeyCode.T;

    private Vector3 startPos;
    private bool isRising;
    private bool isWaitExplode;

    void Start()
    {
        // 记录初始位置
        startPos = transform.position;
        
        // 初始关闭所有特效
        upParticle.Stop();
        explodeParticle.Stop();
    }

    void Update()
    {
        // 按键触发发射（冷却锁定）
        if (Input.GetKeyDown(fireKey) && !isRising && !isWaitExplode)
        {
            LaunchFirework();
        }

        // 上升移动
        if (isRising)
        {
            MoveRise();
        }

        // 爆炸结束 自动重置
        if (isWaitExplode && !explodeParticle.isPlaying)
        {
            ResetFirework();
        }
    }

    // 开始发射
    void LaunchFirework()
    {
        isRising = true;
        upParticle.Play();
        explodeParticle.Stop();
    }

    // 垂直上升
    void MoveRise()
    {
        Vector3 targetPos = new Vector3(transform.position.x, maxHeight, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, riseSpeed * Time.deltaTime);

        // 到达最高点
        if (Mathf.Abs(transform.position.y - maxHeight) < 0.1f)
        {
            ReachTop();
        }
    }

    // 抵达顶端 → 切换爆炸
    void ReachTop()
    {
        isRising = false;
        isWaitExplode = true;

        upParticle.Stop();
        explodeParticle.Play();
    }

    // 完全重置，可二次播放
    void ResetFirework()
    {
        isWaitExplode = false;
        transform.position = startPos;

        upParticle.Stop();
        upParticle.Clear();
        explodeParticle.Stop();
        explodeParticle.Clear();
    }
}