using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("��¡��Ϣ")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;

    [SerializeField] private bool createCloneOnDashStart;
    [SerializeField] private bool createCloneOnDashOver;
    [SerializeField] private bool canCreateCloneCounterAttack;

    [Header("���ط������")]
    [SerializeField] private float chanceToDuplicate;
    [SerializeField] private bool canDuplicateClone;

    [Header("ˮ���������")]
    public bool crystalInsteadOfClone;


    public void CreateClone(Transform _clonePosition,Vector3 _offset)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystalClone();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);
        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition,cloneDuration,canAttack,_offset,FindClosestEnemy(_clonePosition),canDuplicateClone,chanceToDuplicate,player);//FineClosetEnemy����Ҫ��_clonePosition������ȷ����
    }

    public void CreatCloneOnDashStart()
    {
        if (createCloneOnDashStart)
            CreateClone(player.transform, Vector3.zero);

    }

    public void CreateCloneOnDashOver()
    {
        if(createCloneOnDashOver)
            CreateClone(player.transform,Vector3.zero);
    }

    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (canCreateCloneCounterAttack)
            StartCoroutine(CreateCloneWithDelay(_enemyTransform, new Vector3(1 * player.facingDir,0,0)));
        
    }

    private IEnumerator CreateCloneWithDelay(Transform _transform,Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
        CreateClone(_transform, _offset);
    }
}
