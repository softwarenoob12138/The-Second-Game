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
        transform.right = transform.position - targetStats.transform.position; //计算的是 从目标位置指向当前物体位置的方向向量    
        
        if(Vector2.Distance(transform.position, targetStats.transform.position) < .1f)
        {
            anim.transform.localRotation = Quaternion.identity; //在雷击接触到的一瞬间 将其 旋转值 变为 0，让其固定为垂直方向的雷击
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
        targetStats.ApplyShock(true);  //让被闪电击中的额外目标 也进入麻痹效果
        targetStats.TakeDamage(damage);
        Destroy(gameObject, .4f);
    }

}
