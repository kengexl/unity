using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(BoxCollider))]
public class VideoController : MonoBehaviour
{
    [Header("关联播放视频的平面")]
    public VideoPlayer targetVideoPlayer;

    [Header("请设置你的地面层（例如：Ground）")]
    public LayerMask 地面层;

    [Header("防误触设置")]
    [Tooltip("每次操作后需要等待多少秒才能再次操作")]
    public float 冷却时间 = 1.0f; // 默认1秒，可在Inspector调整
    
    private Renderer cubeRenderer;
    private float 上次操作时间; // 内部记录上次点击/触发的时间

    private void Awake()
    {
        cubeRenderer = GetComponent<Renderer>();

        if (targetVideoPlayer != null)
        {
            targetVideoPlayer.playOnAwake = false;
            targetVideoPlayer.Stop();
        }
        // 初始化时间，确保游戏开始时能马上操作一次
        上次操作时间 = -冷却时间; 
    }

    void Start()
    {
        if (targetVideoPlayer != null)
        {
            targetVideoPlayer.Stop();
        }
        UpdateColorByVideoState();
    }

    // 鼠标点击
    private void OnMouseDown()
    {
        TryToggleVideo();
    }

    // 触碰触发
    private void OnTriggerEnter(Collider other)
    {
        // 排除地面
        if (((1 << other.gameObject.layer) & 地面层) != 0)
        {
            return;
        }

        TryToggleVideo();
    }

    // 【新增】尝试触发切换，这里会做冷却时间检查
    void TryToggleVideo()
    {
        // 如果当前时间 - 上次操作时间 < 冷却时间，说明还在冷却中，直接返回
        if (Time.time - 上次操作时间 < 冷却时间)
        {
            return;
        }

        // 冷却结束，执行操作，并更新上次操作时间
        上次操作时间 = Time.time;
        ToggleVideo();
    }

    // 播放/暂停切换
    void ToggleVideo()
    {
        if (targetVideoPlayer == null || cubeRenderer == null)
            return;

        if (targetVideoPlayer.isPlaying)
        {
            targetVideoPlayer.Pause();
        }
        else
        {
            targetVideoPlayer.Play();
        }

        UpdateColorByVideoState();
    }

    // 更新颜色
    void UpdateColorByVideoState()
    {
        if (targetVideoPlayer.isPlaying)
            cubeRenderer.material.color = Color.green;
        else
            cubeRenderer.material.color = Color.red;
    }
}