using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        //AudioManager.instance.PlaySFX(0);

        //Phisics2D 的OverlapCircleAll函数 获取两个变量分别是一个点和一个半径，然后构成的一个圆，让这个圆的大小作为碰撞器的大小
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius); 

        //如果进入的var变量落在这个碰撞器之中，就调用Damage函数
        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null)  //检查碰撞检测结果是否命中了带有 Enemy 组件的游戏对象
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();  //从碰撞检测结果中获取目标对象身上的 EnemyStats 组件，并将其赋给 _target 变量 

                if(_target != null)  
                {
                    player.stats.DoDamage(_target);
                }

                ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);  

                if (weaponData != null)
                {
                    weaponData.Effect(_target.transform);
                }

            }
        }
    }


    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }
}
