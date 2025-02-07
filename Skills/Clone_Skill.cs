using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{

    [Header("Clone info")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]

    [Header("Clone attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private float cloneAttakcMultiplier;
    [SerializeField] private bool canAttack;

    [Header("Aggresive clone")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttakcMultiplier;
    public bool canApplyOnHitEffect {  get; private set; }

    [Header("Mutiple clone")]
    [SerializeField] private UI_SkillTreeSlot multipleUnlockButton;
    [SerializeField] private float multiCloneAttakcMutiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;
    [Header("Crystal instead of clone")]
    [SerializeField] private UI_SkillTreeSlot crystalInsteadUnlockButton;
    public bool crystalInsteadOfClone;


    protected override void Start()
    {
        base.Start();

        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultiClone);
        crystalInsteadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);
    }

    #region Unlock region

    protected override void CheckUnlock()
    {
        UnlockCloneAttack();
        UnlockAggresiveClone();
        UnlockMultiClone();
        UnlockCrystalInstead();
    }


    private void UnlockCloneAttack()
    {
        if(cloneAttackUnlockButton.unlocked)
        {
            canAttack = true;
            attackMultiplier = cloneAttakcMultiplier;
        }
    }

    private void UnlockAggresiveClone()
    {
        if(aggresiveCloneUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;  // 可以给克隆体 一些玩家能打出的攻击效果
            attackMultiplier = aggresiveCloneAttakcMultiplier; 
        }
    }

    private void UnlockMultiClone()
    {
        if(multipleUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMultiplier = multiCloneAttakcMutiplier;
        }
    }

    private void UnlockCrystalInstead()
    {
        if (crystalInsteadUnlockButton.unlocked)
        {
            crystalInsteadOfClone = true;
        }
    }


    #endregion

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        if(crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Skill_Controller>().
            SetupClone(_clonePosition, cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate, player, attackMultiplier);  //创建一个新克隆体之后立马给他 Setup 到新的位置上去
    }

    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        StartCoroutine(CloneDelayCorotine(_enemyTransform, new Vector3(2 * player.facingDir, .4f)));
    }

    private IEnumerator CloneDelayCorotine(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);

        CreateClone(_transform,_offset);
    }
}
