using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockStrike_Controller : MonoBehaviour
{
    [SerializeField] private CharacterStats targetStats;
    [SerializeField] private float speed;
    private int damage;

    private Animator anim;
    private bool triggered;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Setup(int _damage, CharacterStats _targetStats)
    {
        damage = _damage;
        targetStats = _targetStats;
    }

    private void Update()
    {
        if(!targetStats)
        {
            return;
        }

        if(triggered)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - targetStats.transform.position; //������� ��Ŀ��λ��ָ��ǰ����λ�õķ�������    
        
        if(Vector2.Distance(transform.position, targetStats.transform.position) < .1f)
        {
            anim.transform.localRotation = Quaternion.identity; //���׻��Ӵ�����һ˲�� ���� ��תֵ ��Ϊ 0������̶�Ϊ��ֱ������׻�
            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);
            anim.transform.localPosition = new Vector3(0, .5f);


            Invoke("DamageAndSelDestroy", .2f);

            triggered = true;
            anim.SetTrigger("Hit");
        }
    }

    private void DamageAndSelDestroy()
    {
        targetStats.ApplyShock(true);  //�ñ�������еĶ���Ŀ�� Ҳ�������Ч��
        targetStats.TakeDamage(damage);
        Destroy(gameObject, .4f);
    }

}
