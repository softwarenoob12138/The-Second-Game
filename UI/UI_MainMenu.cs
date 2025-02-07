using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] UI_FadeScreen fadeScreen;

    private void Start()
    {
        // 注册一个回调函数，当场景加载完成时触发
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (SaveManager.instance.HasSavedData() == false)
        {
            continueButton.SetActive(false);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // 回调函数，这是回调函数在 Scene 里的一个用法
    {
        fadeScreen.FadeIn();
    }

    private void OnDestroy()
    {   
        // 清除事件监听器，防止内存泄漏
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ContinuGame()
    {
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void NewGame()
    {
        SaveManager.instance.DeleteSaveData();
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void ExitGame()
    {
#if UNITY_EDITOR  // 在编辑器模式退出
        UnityEditor.EditorApplication.isPlaying = false;
#else  // 发布后退出
        Application.Quit();
#endif
    }

    IEnumerator LoadSceneWithFadeEffect(float _delay)
    {
        fadeScreen.FadeOut();

        yield return new WaitForSeconds(_delay);

        // 使用 LoadScene 方法前确定导入了 UnityEngine.SceneManagement 命名空间
        // 该方法 位于 UnityEngine.SceneManagement 命名空间下的 SceneManager 类中
        SceneManager.LoadScene(sceneName);
    }
}
