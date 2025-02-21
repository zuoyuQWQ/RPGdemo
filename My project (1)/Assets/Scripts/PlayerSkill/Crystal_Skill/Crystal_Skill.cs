using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill : Skill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    [SerializeField] private bool canExplode;

    [Header("移动水晶")]
    [SerializeField] private bool canMove;
    [SerializeField] private float moveSpeed;
    private GameObject currentCrystal;

    [Header("水晶幻象")]
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("多重水晶")]
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();


    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiCrystal())
            return;

        // 如果 currentCrystal 是 null，表示没有生成过水晶，执行生成水晶的逻辑
        if (currentCrystal == null)
        {
 //           Debug.Log("不为0");
            // 确保 crystalPrefab 已经被赋值，如果为 null 这里会报错
            if (crystalPrefab != null)
            {
                CreateCrystalClone();
            }
            else
            {
                Debug.LogError("Crystal prefab is not assigned!");
            }
        }
        else
        {
 //           Debug.Log("传送");
            // 确保 currentCrystal 被初始化过后再访问其成员
            if (currentCrystal != null)
            {
                if (canMove)
                    return;

                Vector2 playerPos = player.transform.position;
                player.transform.position = currentCrystal.transform.position;
                currentCrystal.transform.position = playerPos;

                if (cloneInsteadOfCrystal)
                {
                    SkillManager.instance.clone.CreateClone(currentCrystal.transform,Vector3.zero);
                    Destroy(currentCrystal);
                }
                else
                {
                    currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();  // 销毁 currentCrystal 对象
                    currentCrystal = null;  // 重置 currentCrystal，以便下次生成
                }

            }
            else
            {
                Debug.LogError("currentCrystal is null, cannot teleport.");
            }
        }
    }

    public void CreateCrystalClone()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMove, moveSpeed, FindClosestEnemy(currentCrystal.transform), player);
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            if(crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == amountOfStacks)
                    Invoke("RestAbility",useTimeWindow);
                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn,player.transform.position,Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().SetupCrystal(crystalDuration, canExplode, canMove, moveSpeed, FindClosestEnemy(newCrystal.transform), player);

                if(crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                }
            return true;
            }
        }

        return false;

    }

    private void RefilCrystal()
    {
        int amountAdd =amountOfStacks - crystalLeft.Count;
        for (int i = 0;i < amountAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void RestAbility()
    {
        if (cooldownTimer > 0)
            return;
        cooldownTimer = multiStackCooldown;
        RefilCrystal();
    }
}
