using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Skill : MonoBehaviour    
{
    public float cooldown;
    public float cooldownTimer;

    protected Player player;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;

        CheckUnlock();  // 在 开始时检查 存档中的技能解锁程度
    }
    protected virtual void Update()     // 这个脚本统一给技能冷却
    {
        cooldownTimer -= Time.deltaTime;
    }

    protected virtual void CheckUnlock()
    {

    }

    public virtual bool CanUseSkill()
    {
        if (cooldownTimer < 0)
        {
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }

        player.fx.CreatePopUpText("Cooldown");
        return false;
    }

    
    public virtual void UseSkill()
    {

    }

    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 25);

        float closestDistance = Mathf.Infinity;  //Mathf.Infinity 针对单精度浮点数 返回一个float类型的无穷正值
        Transform closestEnemy = null;  //设置为 null 让它在每次调用该方法的开头都被剔除

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distenceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position); //对于每一个碰撞器，得到预制体和录入的物体的距离

                if (distenceToEnemy < closestDistance)  //反复检查和录入直到录入最近的那一个 (遍历最小值)
                {
                    closestDistance = distenceToEnemy;
                    closestEnemy = hit.transform;  //最新的距离是最后检查的那个最近距离，把这个最近的赋给closestEnemy
                }
            }
        }

        return closestEnemy;
    }
}
