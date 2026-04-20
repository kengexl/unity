using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 目标场景名称（填写 Build Settings 里的场景名）
    public string targetSceneName;
    
    // 触发物体的标签（默认玩家标签 Player）
    public string triggerTag = "Player";
    
    // 防止重复触发
    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // 打印：检测到碰撞的物体名称
        Debug.Log("【场景切换】碰撞到物体：" + other.gameObject.name);

        // 已经触发过
        if (isTriggered)
        {
            Debug.LogWarning("【场景切换】已触发过，忽略重复调用");
            return;
        }

        // 标签不匹配
        if (!other.CompareTag(triggerTag))
        {
            Debug.Log("【场景切换】标签不匹配，目标标签：" + triggerTag + "，碰撞物体标签：" + other.tag);
            return;
        }

        // 场景名为空
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("【场景切换】错误：未填写目标场景名称！");
            return;
        }

        // 所有条件满足，执行切换
        Debug.Log("【场景切换】条件满足，开始加载场景：" + targetSceneName);
        SceneManager.LoadScene(targetSceneName);
        isTriggered = true;
    }
}