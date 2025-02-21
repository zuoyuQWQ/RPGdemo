using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwordType
{
    Regular,//常规剑
    Bounce,//反弹剑
    Pierce,//穿刺剑
    Spin//旋转剑
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("反弹剑信息")]
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bouncePower;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("穿刺剑信息")]
    [SerializeField] private int pierceAmount;
    [SerializeField] private float piercePower;
    [SerializeField] private float pierceGravity;

    [Header("旋转剑信息")]
    [SerializeField] private float hitCooldown;
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private float spinDuration;
    [SerializeField] private float spinPower;
    [SerializeField] private float spinGravity;

    [Header("技能信息")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private float swordPower;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float swordReturnSpeed;

    protected override void Start()
    {
        base.Start();
       
    }

    protected override void Update()
    {
        base.Update();
        SetupSwordInfo();
    }


    private void SetupSwordInfo()
    {
        switch (swordType)
        {
            case SwordType.Bounce:
                swordPower = bouncePower;
                swordGravity = bounceGravity;
                break;
            case SwordType.Pierce:
                swordPower = piercePower;
                swordGravity = pierceGravity;
                break;
            case SwordType.Spin:
                swordPower = spinPower;
                swordGravity = spinGravity;
                break;
        }
    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        switch (swordType)
        {
            case SwordType.Bounce:
                newSwordScript.SetupBounce(true, bounceAmount,bounceSpeed);
                break;
            case SwordType.Pierce:
                newSwordScript.SetupPierce(true, pierceAmount);
                break;
            case SwordType.Spin:
                newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration,hitCooldown);
                break;
        }

        newSwordScript.SetupSword(swordPower, swordGravity, player,freezeTimeDuration,swordReturnSpeed); ;

        player.AssignNewSword(newSword);
    }

   
}