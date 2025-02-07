using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;

    private Transform player;

    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private string closestCheckpointId;
    
    [Header("Lost currency")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);  //ɾ��instance,������ж��ʵ���Ļ�����һ��������ֵ������Ľ������٣������C#�е� ����
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        checkpoints = FindObjectsOfType<Checkpoint>();

        player = PlayerManager.instance.player.transform; 
    }

    public void RestarScene()
    {
        SaveManager.instance.SaveGame();

        //  GetActiveScene() ���� ��ȡ��ǰ����ĳ�����������洢��һ����Ϊ scene �ı�����
        //  LoadScene() ���¼��س�������ζ�����½���һ�� ��Ϸģʽ
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadData(GameData _data) => StartCoroutine(LoadWithDelay(_data));

    private void LoadCheckpoints(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (Checkpoint checkpoint in checkpoints)   // ���� foreach ѭ��Ƕ�� ʵ�� ��֤�Ƿ� �������
            {
                if (checkpoint.id == pair.Key && pair.Value == true)
                {
                    checkpoint.ActivateCheckpoint();
                }
            }
        }
    }

    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if(lostCurrencyAmount > 0)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY + .4f), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;  // ��û�񵽻���û��û�֮ǰ ���ڶ��� ����Ļ�ͱ�� 0 ��

    }

    private IEnumerator LoadWithDelay(GameData _data)  // ����һ����ʱ ���ô˷��������� ��Ϊ һЩ����ԭ���µĽű�˳���������
    {
        yield return new WaitForSeconds(.00000001f);

        LoadLostCurrency(_data);
        LoadClosestCheckpoint(_data);
        LoadCheckpoints(_data);
    }

    public void SaveData(ref GameData _data)
    {
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;


        if(FindClosestCheckpoint() != null)
        {
            _data.closestCheckpointId = FindClosestCheckpoint().id; // ���˳� ��Ϸģʽ ʱ���¼�˳�ʱ�������� ����
        }
        _data.checkpoints.Clear();

        foreach (Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.id, checkpoint.activationStatus);
        }
    }
    private void LoadClosestCheckpoint(GameData _data)
    {
        if(_data.closestCheckpointId == null)
        {
            return;
        }

        closestCheckpointId = _data.closestCheckpointId;

        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (closestCheckpointId == checkpoint.id)
            {
                player.position = new Vector3(checkpoint.transform.position.x, checkpoint.transform.position.y - 1);
            }
        }
    }

    private Checkpoint FindClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach (var checkpoint in checkpoints)
        {
            float distanceToCheckpoint = Vector2.Distance(player.position, checkpoint.transform.position);

            if (distanceToCheckpoint < closestDistance && checkpoint.activationStatus == true)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }
        }

        return closestCheckpoint;
    }

    public void PauseGame(bool _pause)
    {
        if(_pause)
        {
            Time.timeScale = 0;  // ���ڿ���ʱ��߶ȣ�Ϊ 0 ����ʱ��ֹͣ��Ϊ 1 ������ʱ�����
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
