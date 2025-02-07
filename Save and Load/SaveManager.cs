using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 用于引入 LINQ 相关的命名空间，类似于 SQL 查询语言，可更方便地对数组、列表和其他集合进行过滤、排序、分组等操作 
using System.Linq;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;

    private GameData gameData;
    private List<ISaveManager> saveManagers; // 用列表来收集多个接口，用来应对这种需要在多个脚本中保存数据的情况
    private FileDataHandler dataHandler;

    // 它允许开发者在 检视面板 中为脚本添加一个右键菜单选项
    // 使用户在右键点击此脚本时 会显示 Delete save file
    // 而在这里 它点击此选项后会执行 它的下方第一个方法 即 DeleteSaveData
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
        // Application.persistentDataPath 是 Unity 提供的一个属性，返回一个字符串，表示应用程序的持久数据存储路径
        // 这个路径在不同平台上会有不同的值如 Windows 系统， Android 系统等等......
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
        foreach (ISaveManager saveManager in saveManagers) // 对 saveManagers 里 所有 MonoBehaviour 对象里 对 ISaveManager 接口的实现
        {
            saveManager.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);

    }

    //Unity中的一个回调方法
    //当应用程序即将退出时或者Scene进行切换时会被调用，通常用于保存数据
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
        // FindObjectsOfType<MonoBehaviour>() 这个方法返回当前场景中所有活动的 MonoBehaviour 对象的数组
        // .OfType 这个 LINQ 方法用于过滤集合中的对象，只保留那些实现了 ISaveManager 接口的对象
        // IEnumerable 是 LINQ 方法查询的基础 此处是将 过滤后的 IEnumerable<ISaveManager> 转换为 List<ISaveManager> 并返回
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
