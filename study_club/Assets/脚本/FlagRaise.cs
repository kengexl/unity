using UnityEngine;
using UnityEngine.SceneManagement;

public class FlagRaise : MonoBehaviour
{
    [Header("升旗 起点 & 终点")]
    public Vector3 startPos;
    public Vector3 endPos;

    [Header("音频设置")]
    public AudioClip raiseAudio;
    private AudioSource _audioSource;

    [Header("状态锁定")]
    public bool isRaising = false;
    private float _runTime;
    private float _audioLength;

    // 全局其他音源
    private System.Collections.Generic.List<AudioSource> _otherAudio = new System.Collections.Generic.List<AudioSource>();


    void Start()
    {
        // 自动把当前坐标设为初始位置
        if (startPos == Vector3.zero)
        {
            startPos = transform.position;
        }

        // 添加音源组件
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        // 收集场景所有其他声音
        CollectAllAudioSource();
    }


    void Update()
    {
        // 按E开始升旗
        if (Input.GetKeyDown(KeyCode.E) && !isRaising)
        {
            StartRaise();
        }

        // 升旗移动逻辑
        if (isRaising)
        {
            RaiseUpdate();
        }
    }


    void StartRaise()
    {
        isRaising = true;
        _runTime = 0f;

        // 关闭所有其他音效/音乐
        MuteAllOtherSound();

        // 播放升旗音频
        if (raiseAudio != null)
        {
            _audioLength = raiseAudio.length;
            _audioSource.clip = raiseAudio;
            _audioSource.Play();
        }

        Debug.Log("开始升旗");
    }


    void RaiseUpdate()
    {
        if (_audioLength <= 0) return;

        _runTime += Time.deltaTime;
        float progress = Mathf.Clamp01(_runTime / _audioLength);

        // 从【初始位置】平滑移动到【结束位置】
        transform.position = Vector3.Lerp(startPos, endPos, progress);

        // 音频播放完毕 → 升旗结束 定格在最高点
        if (progress >= 1f)
        {
            isRaising = false;
            Debug.Log("升旗完成，已到达终点");
        }
    }


    // 收集全场音源
    void CollectAllAudioSource()
    {
        AudioSource[] all = FindObjectsOfType<AudioSource>();
        foreach (var a in all)
        {
            if (a != _audioSource)
                _otherAudio.Add(a);
        }
    }

    // 静音所有其他声音
    void MuteAllOtherSound()
    {
        foreach (var a in _otherAudio)
        {
            a.Pause();
            a.mute = true;
        }
    }

    // 给风场脚本调用的进度获取方法
    public float GetProgress()
    {
        if (_audioLength <= 0) return 0f;
        return Mathf.Clamp01(_runTime / _audioLength);
    }
}