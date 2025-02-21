using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill : Skill
{
    [Header("ºÚ¶´ÐÅÏ¢")]
    [SerializeField] private float blackholeDuration;
    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private float maxSzie;
    [SerializeField] private int attackAmounts;
    [SerializeField] private float cloneAttackCooldown;

    Blackhole_Skill_Controller currentBlackhole;

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }


    public override void UseSkill()
    {
        base.UseSkill();
        GameObject newBlackhole = Instantiate(blackholePrefab,player.transform.position,Quaternion.identity);

        currentBlackhole = newBlackhole.GetComponent<Blackhole_Skill_Controller>();
        
        currentBlackhole.SetupBlackhole(maxSzie,attackAmounts,cloneAttackCooldown,blackholeDuration);
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool SkillFinished()
    {
        if(!currentBlackhole)
            return false;

        if(currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
        }
        return false;
    }

    public float GetBlackholeRadius()
    {
        return maxSzie / 2;
    }
}
