using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill : Skill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    [SerializeField] private bool canExplode;

    [Header("�ƶ�ˮ��")]
    [SerializeField] private bool canMove;
    [SerializeField] private float moveSpeed;
    private GameObject currentCrystal;

    [Header("ˮ������")]
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("����ˮ��")]
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

        // ��� currentCrystal �� null����ʾû�����ɹ�ˮ����ִ������ˮ�����߼�
        if (currentCrystal == null)
        {
 //           Debug.Log("��Ϊ0");
            // ȷ�� crystalPrefab �Ѿ�����ֵ�����Ϊ null ����ᱨ��
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
 //           Debug.Log("����");
            // ȷ�� currentCrystal ����ʼ�������ٷ������Ա
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
                    currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();  // ���� currentCrystal ����
                    currentCrystal = null;  // ���� currentCrystal���Ա��´�����
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
