using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// �������� LINQ ��ص������ռ䣬������ SQL ��ѯ���ԣ��ɸ�����ض����顢�б���������Ͻ��й��ˡ����򡢷���Ȳ��� 
using System.Linq;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;

    private GameData gameData;
    private List<ISaveManager> saveManagers; // ���б����ռ�����ӿڣ�����Ӧ��������Ҫ�ڶ���ű��б������ݵ����
    private FileDataHandler dataHandler;

    // ������������ ������� ��Ϊ�ű����һ���Ҽ��˵�ѡ��
    // ʹ�û����Ҽ�����˽ű�ʱ ����ʾ Delete save file
    // �������� �������ѡ����ִ�� �����·���һ������ �� DeleteSaveData
    [ContextMenu("Delete save file")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject); 
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        // Application.persistentDataPath �� Unity �ṩ��һ�����ԣ�����һ���ַ�������ʾӦ�ó���ĳ־����ݴ洢·��
        // ���·���ڲ�ͬƽ̨�ϻ��в�ͬ��ֵ�� Windows ϵͳ�� Android ϵͳ�ȵ�......
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        saveManagers = FindAllSaveManagers();
        LoadGame();
    }




    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if(this.gameData == null)
        {
            NewGame();
        }

        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    public void SaveGame()
    { 
        foreach (ISaveManager saveManager in saveManagers) // �� saveManagers �� ���� MonoBehaviour ������ �� ISaveManager �ӿڵ�ʵ��
        {
            saveManager.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);

    }

    //Unity�е�һ���ص�����
    //��Ӧ�ó��򼴽��˳�ʱ����Scene�����л�ʱ�ᱻ���ã�ͨ�����ڱ�������
    private void OnApplicationQuit()  
    {
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name == "MainScene")
        {
            SaveGame();
        }
    }

    private List<ISaveManager> FindAllSaveManagers()
    {
        // FindObjectsOfType<MonoBehaviour>() ����������ص�ǰ���������л�� MonoBehaviour ���������
        // .OfType ��� LINQ �������ڹ��˼����еĶ���ֻ������Щʵ���� ISaveManager �ӿڵĶ���
        // IEnumerable �� LINQ ������ѯ�Ļ��� �˴��ǽ� ���˺�� IEnumerable<ISaveManager> ת��Ϊ List<ISaveManager> ������
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers);
    }

    public bool HasSavedData()
    {
        if(dataHandler.Load() != null)
        {
            return true;
        }

        return false;
    }
}
