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

        CheckUnlock();  // �� ��ʼʱ��� �浵�еļ��ܽ����̶�
    }
    protected virtual void Update()     // ����ű�ͳһ��������ȴ
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

        float closestDistance = Mathf.Infinity;  //Mathf.Infinity ��Ե����ȸ����� ����һ��float���͵�������ֵ
        Transform closestEnemy = null;  //����Ϊ null ������ÿ�ε��ø÷����Ŀ�ͷ�����޳�

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distenceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position); //����ÿһ����ײ�����õ�Ԥ�����¼�������ľ���

                if (distenceToEnemy < closestDistance)  //��������¼��ֱ��¼���������һ�� (������Сֵ)
                {
                    closestDistance = distenceToEnemy;
                    closestEnemy = hit.transform;  //���µľ������������Ǹ�������룬���������ĸ���closestEnemy
                }
            }
        }

        return closestEnemy;
    }
}
