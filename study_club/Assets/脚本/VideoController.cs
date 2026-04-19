using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(BoxCollider))]
public class VideoController : MonoBehaviour
{
    [Header("关联播放视频的平面")]
    public VideoPlayer targetVideoPlayer;

    [Header("请设置你的地面层（例如：Ground）")]
    public LayerMask 地面层;

    private Renderer cubeRenderer;

    private void Awake()
    {
        cubeRenderer = GetComponent<Renderer>();

        if (targetVideoPlayer != null)
        {
            targetVideoPlayer.playOnAwake = false;
            targetVideoPlayer.Stop();
        }
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
        ToggleVideo();
    }

    // 触碰触发 —— 【关键：排除地面】
    private void OnTriggerEnter(Collider other)
    {
        // 如果碰撞对象是地面，直接跳过，不触发视频
        if (((1 << other.gameObject.layer) & 地面层) != 0)
        {
            return;
        }

        // 只有非地面物体才能触发播放
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