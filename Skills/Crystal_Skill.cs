using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Crystal mirage")]
    [SerializeField] private UI_SkillTreeSlot unlockCloneInsteadButton;
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("Crystal simple")] 
    [SerializeField] private UI_SkillTreeSlot unlockCrystalButton;
    public bool crystalUnlocked { get; private set; }

    [Header("Explosive crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosiveButton;
    [SerializeField] private float explosiveCooldown;
    [SerializeField] private bool canExplode;

    [Header("Moving crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMovingCrystalButton;
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;


    [Header("Multi stacking crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMultiStackButton;
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWinndow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    protected override void Start()
    {
        base.Start();

        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCloneInsteadButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        unlockExplosiveButton.GetComponent<Button>().onClick.AddListener(UnlockExplosiveCrystal);
        unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        unlockMultiStackButton.GetComponent<Button>().onClick.AddListener(UnlockMultiStack);
    }

    // 这里解锁 水晶系列技能
    #region Unlock skill region

    protected override void CheckUnlock()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrystal();
        UnlockMultiStack();
    }

    private void UnlockCrystal()
    {
        if(unlockCrystalButton.unlocked)
        {
            crystalUnlocked = true;
        }
    }

    private void UnlockCrystalMirage()
    {
        if(unlockCloneInsteadButton.unlocked)
        {
            cloneInsteadOfCrystal = true;
        }
    }

    private void UnlockExplosiveCrystal()
    {
        if(unlockExplosiveButton.unlocked)
        {
            canExplode = true;
            cooldown = explosiveCooldown; 
        }
    }

    private void UnlockMovingCrystal()
    {
        if(unlockMovingCrystalButton.unlocked)
        {
            canMoveToEnemy = true;
        }
    }

    private void UnlockMultiStack()
    {
        if(unlockMultiStackButton.unlocked)
        {
            canUseMultiStacks = true;
        }
    }
    #endregion

    public override void UseSkill()
    {
        base.UseSkill();

        if(CanUseMultiCrystal()) //点按 F 之后,进入 CanUseMultiCrystal() 方法进行 bool 判断时会触发相应的 crystal 的方法
        {
            return;
        }

        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if(canMoveToEnemy)
            {
                return;
            }

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;


            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, new Vector3(0, .4f));
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal(); // ?. 运算符:空条件运算符，只有在其左边的操作数不是 null 的情况下，才会执行右边的表达式
            }

        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform), player);
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();


    private bool CanUseMultiCrystal()
    {
        if(canUseMultiStacks)
        {
            if(crystalLeft.Count > 0)
            {

                if(crystalLeft.Count == amountOfStacks) // 只要使用了第一次，就开始计时补充新的 crystal
                {
                    Invoke("ReResetAbility", useTimeWinndow);
                }

                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1]; 
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform), player);

                if(crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown; // 三个变量赋值问题，在不更改 cooldownTimer 的情况下，要在 crystalLeft.Count > 0 时不断给 cooldown 赋值为 0
                    RefilCrystal();
                }

                return true;

            }
        }
        return false;
    }

    private void RefilCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;

        for(int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if(cooldownTimer > 0)
        {
            return;
        }
        cooldownTimer = multiStackCooldown;
        RefilCrystal();
    }
}
