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
            
            //生成数组索引时 Random.Range(0, 1) 当 dropList.Count = 2 时 会生成 0 或 1 ，生成非索引时 Random.Range(0, 1) 就只会生成 0
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
