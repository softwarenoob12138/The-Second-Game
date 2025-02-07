using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    public string id;
    public bool activationStatus;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    [ContextMenu("Generate checkpoint id")]
    private void GenerateId()
    {
        // System.Guid.NewGuid() ����������һ���������Ա�֤ȫ��Ψһ�� GUID ���� GUID ��һ��128λ������
        id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() != null)
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        if(activationStatus == false)
        {
            AudioManager.instance.PlaySFX(5, transform);
        }
        activationStatus = true;
        anim.SetBool("active", true);
    }
}
