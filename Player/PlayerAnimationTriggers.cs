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

        //Phisics2D ��OverlapCircleAll���� ��ȡ���������ֱ���һ�����һ���뾶��Ȼ�󹹳ɵ�һ��Բ�������Բ�Ĵ�С��Ϊ��ײ���Ĵ�С
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius); 

        //��������var�������������ײ��֮�У��͵���Damage����
        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>() != null)  //�����ײ������Ƿ������˴��� Enemy �������Ϸ����
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();  //����ײ������л�ȡĿ��������ϵ� EnemyStats ����������丳�� _target ���� 

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
