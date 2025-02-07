using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerStats : CharacterStats
{
    private Player player;
    [SerializeField] bool canDropItem;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();

        player.Die();

        GameManager.instance.lostCurrencyAmount = PlayerManager.instance.currency;
        PlayerManager.instance.currency = 0;

        if(canDropItem)
        { 
            GetComponent<PlayerItemDrop>()?.GenerateDrop();
        }
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if(_damage > GetMaxHealthValue() * .3f)
        {
            player.SetupKnockbackPower(new Vector2(10, 6)); // ����ܵ������˺�ʱ ����һ��ǿ����
            player.fx.ScreenShake(player.fx.shakeHighDamage);

            // int randomaSound = Random.Range();
            // AudioManager.instance.PlaySFX();    ���ڴ˼����ܻ���Ч
        }

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        if(currentArmor != null)
        {
            currentArmor.Effect(player.transform);
        }
    }

    public override void OnEvasion()
    {
        player.skill.dodge.CreateMirageOnDodge();
    }

    public void CloneDoDamage(CharacterStats _targetStats, float _multiplier)
    {
        if (TargetCanAvoidAttack(_targetStats))
        {
            return;
        }

        int totalDamage = damage.GetValue() + strength.GetValue();

        if(_multiplier > 0)
        {
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier); // ��ȡ��ҵ���ֵ�� �ټ����¡��Ĺ������� 
        }

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }



        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        DoMagicalDamage(_targetStats);
    }
}
