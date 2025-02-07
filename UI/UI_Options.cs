using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Options : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainMenu";
    [SerializeField] private UI_FadeScreen fadeScreen;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void SwitchToMainMenu()
    {
        GameManager.instance.PauseGame(false);

        SaveManager.instance.SaveGame();

        StartCoroutine(LoadMainMenuWithFadeEffect(1.5f));
    }

    IEnumerator LoadMainMenuWithFadeEffect(float _delay)
    {
        fadeScreen.FadeOut();

        yield return new WaitForSeconds(_delay);

        // ʹ�� LoadScene ����ǰȷ�������� UnityEngine.SceneManagement �����ռ�
        // �÷��� λ�� UnityEngine.SceneManagement �����ռ��µ� SceneManager ����
        SceneManager.LoadScene(sceneName);
    }




}
