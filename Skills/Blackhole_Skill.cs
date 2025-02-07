using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackhole_Skill : Skill
{

    [SerializeField] private UI_SkillTreeSlot blackHoleUnlockButton;
    public bool blackHoleUnlocked { get; private set; }
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float cloneCooldown; //攻击间隔时间
    [SerializeField] private float blackholeDuration;
    [Space]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    Blackhole_Skill_Controller currentBlackhole;

    private void UnlockBlackhole()
    {
        if(blackHoleUnlockButton.unlocked)
        {
            blackHoleUnlocked = true;
        }
    }

    public override bool CanUseSkill()
    {
        if (cooldownTimer < 0)
        {
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }

        return false;
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity); 

        currentBlackhole = newBlackHole.GetComponent<Blackhole_Skill_Controller>();

        currentBlackhole.SetupBlackhole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneCooldown, blackholeDuration);

        AudioManager.instance.PlaySFX(7, player.transform);
    }

    protected override void Start()
    {
        base.Start();

        blackHoleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBlackhole);
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool SkillCompleted()
    {
        if(!currentBlackhole)
        {
            return false;
        }

        if(currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
            
        }

        return false;
    }

    public float GetBlackholeRadius()
    {
        return maxSize / 2;  // 半径是比例的一半，所以在控制半径检查时要给半径的值设置为比例的一半
    }

    protected override void CheckUnlock()
    {
        UnlockBlackhole();
    }

}
