using UnityEngine;

[RequireComponent(typeof(WindZone))]
public class WindController : MonoBehaviour
{
    [Header("基础设置")]
    public bool windEnabled = true;
    [Tooltip("风的基础强度，影响旗帜摆动幅度")]
    public float baseMainStrength = 1.2f;
    [Tooltip("湍流强度，数值越大风越乱")]
    public float baseTurbulence = 0.8f;

    [Header("脉冲风设置（风的强弱变化）")]
    public float pulseMagnitude = 0.3f;
    public float pulseFrequency = 0.2f;

    [Header("自然风呼吸效果")]
    public bool naturalWindMode = true;
    [Tooltip("风强度波动幅度（0-1）")]
    public float breathRange = 0.2f;
    [Tooltip("风强度波动速度")]
    public float breathSpeed = 0.5f;

    [Header("和升旗脚本联动（可选）")]
    public FlagRaise flagRaiseScript;
    public bool linkToRaiseProgress = true;
    public float raiseStartWind = 0.3f;
    public float raiseEndWind = 1.5f;

    [Header("调试按键")]
    public KeyCode toggleWindKey = KeyCode.F;
    public KeyCode increaseWindKey = KeyCode.Equals;
    public KeyCode decreaseWindKey = KeyCode.Minus;

    private WindZone _windZone;
    private float _breathOffset;
    private float _targetMainStrength;


    void Start()
    {
        // 自动获取风场组件
        _windZone = GetComponent<WindZone>();
        if (_windZone == null)
        {
            Debug.LogError("WindController 找不到 WindZone 组件！请挂载到带WindZone的物体上");
            enabled = false;
            return;
        }

        // 初始化风场模式（必须是定向模式，适合旗帜）
        _windZone.mode = WindZoneMode.Directional;

        // 初始设置参数（修正后的属性名）
        _windZone.windMain = baseMainStrength;
        _windZone.windTurbulence = baseTurbulence;
        _windZone.windPulseMagnitude = pulseMagnitude;
        _windZone.windPulseFrequency = pulseFrequency;

        _targetMainStrength = baseMainStrength;
        _breathOffset = Random.Range(0f, 100f); // 随机初始相位，避免所有风场同步
    }


    void Update()
    {
        // 调试按键控制
        HandleDebugInput();

        // 风场开关控制
        if (!windEnabled)
        {
            _windZone.windMain = 0f;
            return;
        }

        // 处理升旗联动逻辑
        if (linkToRaiseProgress && flagRaiseScript != null)
        {
            UpdateWindWithRaiseProgress();
        }
        else
        {
            _targetMainStrength = baseMainStrength;
        }

        // 处理自然风呼吸效果
        if (naturalWindMode)
        {
            ApplyNaturalBreathEffect();
        }
        else
        {
            // 无呼吸效果时直接设置目标强度
            _windZone.windMain = Mathf.Lerp(_windZone.windMain, _targetMainStrength, Time.deltaTime * 2f);
        }
    }


    // 自然风呼吸效果：强度在目标值附近平滑波动
    void ApplyNaturalBreathEffect()
    {
        float breathValue = Mathf.Sin((Time.time + _breathOffset) * breathSpeed) * breathRange;
        float finalStrength = _targetMainStrength * (1f + breathValue);
        _windZone.windMain = Mathf.Lerp(_windZone.windMain, finalStrength, Time.deltaTime * 3f);
    }


    // 和升旗进度联动，风随升旗过程逐渐增强
    void UpdateWindWithRaiseProgress()
    {
        if (flagRaiseScript.isRaising)
        {
            // 计算升旗进度（0-1）
            float progress = flagRaiseScript.GetProgress();
            // 风强度从 raiseStartWind 平滑过渡到 raiseEndWind
            _targetMainStrength = Mathf.Lerp(raiseStartWind, raiseEndWind, progress);
        }
        else
        {
            // 升旗结束后稳定在目标强度
            _targetMainStrength = baseMainStrength;
        }
    }


    // 调试按键处理
    void HandleDebugInput()
    {
        if (Input.GetKeyDown(toggleWindKey))
        {
            windEnabled = !windEnabled;
            Debug.Log("风场已" + (windEnabled ? "开启" : "关闭"));
        }

        if (Input.GetKeyDown(increaseWindKey))
        {
            baseMainStrength = Mathf.Clamp(baseMainStrength + 0.2f, 0f, 5f);
            Debug.Log("风强度提升到：" + baseMainStrength);
        }

        if (Input.GetKeyDown(decreaseWindKey))
        {
            baseMainStrength = Mathf.Clamp(baseMainStrength - 0.2f, 0f, 5f);
            Debug.Log("风强度降低到：" + baseMainStrength);
        }
    }


    // 外部调用：直接设置风强度
    public void SetWindStrength(float strength)
    {
        baseMainStrength = Mathf.Clamp(strength, 0f, 5f);
        _targetMainStrength = baseMainStrength;
    }


    // 外部调用：开启/关闭风场
    public void ToggleWind(bool enabled)
    {
        windEnabled = enabled;
    }
}