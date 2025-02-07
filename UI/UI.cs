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
        // �� Awake ʹ֮�� ����Ϸģʽ�йر�ĳ UI ���ܵ��ø� UI �� Awake ���� �� �� skill �ű��з����¼�֮ǰ �� skill tree �Ϸ����¼�
        // ���� �ڷ� ��Ϸģʽ �ر� UI �� �����¼���˳�������
        SwitchTo(skillTreeUI);
        fadeScreen.gameObject.SetActive(true);
    }

    private void Start()
    {
        // �� Unity �������ֶ� ��ȡ��� �ﵽ���� Awake ��Ч�� �Ͳ��������� Start �� GetComponent �� 

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
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null; // �������� fade screen ���ֻ�Ծ״̬

            // ��� ������ UI_FadeScreen ����� ���� ���Ӷ��� (������жϵ�ԭ�򲻱�� ���ǵ��� �� fade screen ���ֻ�Ծ״̬)
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
        if (_menu != null && _menu.activeSelf) // ����ͨ������ activeSelf ��������ȡ��Ϸ����ǰ�Ƿ񼤻�
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
            // ������������ Unity �е� DarkScreen ʹ�����ڼ����ǰ���� ���� SwitchTo(inGameUI) ����
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null) 
            {
                return; // �˴� return ��ֱ�ӷ�������������
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
