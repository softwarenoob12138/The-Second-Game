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
        // ע��һ���ص��������������������ʱ����
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (SaveManager.instance.HasSavedData() == false)
        {
            continueButton.SetActive(false);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // �ص����������ǻص������� Scene ���һ���÷�
    {
        fadeScreen.FadeIn();
    }

    private void OnDestroy()
    {   
        // ����¼�����������ֹ�ڴ�й©
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
#if UNITY_EDITOR  // �ڱ༭��ģʽ�˳�
        UnityEditor.EditorApplication.isPlaying = false;
#else  // �������˳�
        Application.Quit();
#endif
    }

    IEnumerator LoadSceneWithFadeEffect(float _delay)
    {
        fadeScreen.FadeOut();

        yield return new WaitForSeconds(_delay);

        // ʹ�� LoadScene ����ǰȷ�������� UnityEngine.SceneManagement �����ռ�
        // �÷��� λ�� UnityEngine.SceneManagement �����ռ��µ� SceneManager ����
        SceneManager.LoadScene(sceneName);
    }
}
