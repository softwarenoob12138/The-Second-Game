using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;

    public virtual void GenerateDrop()
    {
        for(int i = 0; i < possibleDrop.Length; i++)
        {
            if(Random.Range(0, 100) <= possibleDrop[i].dropChance)
            {
                dropList.Add(possibleDrop[i]);
            }
            
        }

        if(dropList.Count == 0)
        {
            return;
        }

        for(int i = 0; i < possibleItemDrop && dropList.Count > 0; i++)
        {
            
            //������������ʱ Random.Range(0, 1) �� dropList.Count = 2 ʱ ������ 0 �� 1 �����ɷ�����ʱ Random.Range(0, 1) ��ֻ������ 0
            ItemData randomItem = dropList[Random.Range(0, dropList.Count)];


            dropList.Remove(randomItem);
          

            DropItem(randomItem);
            
        }
    }


    protected void DropItem(ItemData _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));

        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelocity);
    }
}
