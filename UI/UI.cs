using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class UI : MonoBehaviour, ISaveManager
{
    [Header("End screen")]
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Space]

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;

    public UI_SkillToolTip skillToolTip;
    public UI_ItemTooltip itemTooltip;
    public UI_StatTooltip statTooltip;
    public UI_CraftWindow craftWindow;

    [SerializeField] private UI_VolumeSlider[] volumeSettings;
    private void Awake()
    {
        // 用 Awake 使之在 非游戏模式中关闭某 UI 后能调用该 UI 的 Awake 方法 来 在 skill 脚本中分配事件之前 在 skill tree 上分配事件
        // 避免 在非 游戏模式 关闭 UI 后 分配事件的顺序出问题
        SwitchTo(skillTreeUI);
        fadeScreen.gameObject.SetActive(true);
    }

    private void Start()
    {
        // 在 Unity 引擎中手动 获取组件 达到类似 Awake 的效果 就不用在这里 Start 里 GetComponent 了 

        SwitchTo(inGameUI);

        itemTooltip.gameObject.SetActive(false);
        statTooltip.gameObject.SetActive(false);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            SwitchWithKeyTo(characterUI);
        }    

        if(Input.GetKeyDown(KeyCode.B))
        {
            SwitchWithKeyTo(craftUI);
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            SwitchWithKeyTo(skillTreeUI);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SwitchWithKeyTo(optionsUI);
        }
    }

    public void SwitchTo(GameObject _menu)
    {


        for (int i = 0; i < transform.childCount; i++)
        {
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null; // 用它来让 fade screen 保持活跃状态

            // 如果 不存在 UI_FadeScreen 组件就 禁用 该子对象 (加这个判断的原因不必深究 就是单纯 让 fade screen 保持活跃状态)
            if (!fadeScreen) 
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        if (_menu != null)
        {
            AudioManager.instance.PlaySFX(8, null);
            _menu.SetActive(true);
        }

        if(GameManager.instance != null)
        {
            if(_menu == inGameUI)
            {
                GameManager.instance.PauseGame(false);
            }
            else
            {
                GameManager.instance.PauseGame(true);
            }
        }

    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if (_menu != null && _menu.activeSelf) // 可以通过设置 activeSelf 属性来获取游戏对象当前是否激活
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }

        SwitchTo(_menu);

    }

    private void CheckForInGameUI()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            // && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null
            // 它是用于限制 Unity 中的 DarkScreen 使它能在激活的前提下 调用 SwitchTo(inGameUI) 方法
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null) 
            {
                return; // 此处 return 是直接返回这整个方法
            }
        }

        SwitchTo(inGameUI);
    }

    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCorutine());
    }

    IEnumerator EndScreenCorutine()
    {
        yield return new WaitForSeconds(1);

        endText.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        restartButton.SetActive(true);
    }

    public void RestartGameButton() => GameManager.instance.RestarScene();

    public void LoadData(GameData _data)
    {
        foreach(KeyValuePair<string,float> pair in _data.volumeSettings)
        {
            foreach(UI_VolumeSlider item in volumeSettings)
            {
                if(item.parameter == pair.Key)
                {
                    item.LoadSlider(pair.Value);
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();

        foreach (UI_VolumeSlider item in volumeSettings)
        {
            _data.volumeSettings.Add(item.parameter, item.slider.value);
        }
    }
}
