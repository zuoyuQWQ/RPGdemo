using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum StatType
{

    strength,//�����������˺��ͱ����˺�
    agility,//���ݡ����ܺͱ�������
    intelligence,//����,���˺ͷ���
    vitality,//����,����ֵ


    damage,
    critChance,
    critPower,


    maxHealth,
    armor,
    evasion,
    magicResistance,

    fireDamage,
    iceDamage,
    lightingDamage
}

public class CharacterStats : MonoBehaviour
{
    private EntityFx fx;

    [Header("��������")]
    public Stat strength;//�����������˺��ͱ����˺�
    public Stat agility;//���ݡ����ܺͱ�������
    public Stat intelligence;//����,���˺ͷ���
    public Stat vitality;//����,����ֵ

    [Header("��������")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;


    [Header("��������")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("ħ������")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;


    [Header("״̬")]
    public bool isFire;
    [SerializeField] private float defaultFireTimeDuration;
    private float fireTimeDuration;

    public bool isIce;
    [SerializeField] private float defaultIceTimeDuration;
    private float iceTimeDuration;

    public bool isLighting;
    [SerializeField] private float defaultLightingTimeDuration;
    private float lightingTimeDuration;


    private float fireTimer;
    private float iceTimer;
    private float lightingTimer;


    private float defaultFireDamageCooldown = 0.5f;
    private float fireDamageCooldown;
    private float fireDamageTimer;
    private int fireStateDamage;

    [SerializeField] private GameObject lightingStrikePrefab;
    private int beLightingDamage;//״̬���˺�

    private float defaultLightingDamageCooldown = 0.5f;
    private float lightingDamageCooldown;

    [Space]
    public int currentHealth;


    public System.Action onHealthChanged;
    public bool isDead {  get; private set; }

    protected virtual void Start()
    {
        fireTimeDuration = defaultFireTimeDuration;
        fireDamageCooldown = defaultFireDamageCooldown;

        iceTimeDuration = defaultIceTimeDuration;

        lightingTimeDuration = defaultLightingTimeDuration;
        lightingDamageCooldown = defaultLightingDamageCooldown;

        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();

        fx = GetComponent<EntityFx>();

    }

    protected virtual void Update()
    {
        fireTimer -= Time.deltaTime;
        iceTimer -= Time.deltaTime;
        lightingTimer -= Time.deltaTime;

        fireDamageTimer -= Time.deltaTime;

        if (fireTimer < 0)
            isFire = false;
        if (iceTimer < 0)
            isIce = false;
        if (lightingTimer < 0)
            isLighting = false;

        ApplyFireDamage();
    }
    #region buff����
    public virtual void IncreaseStatBy(int _modifier,float _duration,Stat _statToModify)
    {
        StartCoroutine(StatModCorotuien(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCorotuien(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }
    #endregion

    #region �����￹ģ��
    //���������˺����㷽��
    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (CanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
           totalDamage = CalculateCriticalDamage(totalDamage);
        }
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);
        DoMagicDamage(_targetStats);//��ͨ����Ҳ����ɷ��ˣ�����Ҫɾ��
    }

    //���׼��˼��㷽��
    private static int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isIce)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * 0.8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }
    #endregion


    #region ���˷���ģ��
    //����ħ���˺����㷽��
    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();
        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);

        _targetStats.TakeDamage(totalMagicDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
        {
            return;
        }

        
        AttemptToApplyAilements(_targetStats, _fireDamage, _iceDamage, _lightingDamage);

    }

    private void AttemptToApplyAilements(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        bool canApplyFire = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyIce = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyLighting = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;


        while (!canApplyFire && !canApplyIce && !canApplyLighting)
        {
            if (UnityEngine.Random.value < .5d && _fireDamage > 0)
            {
                canApplyFire = true;
                _targetStats.ApplyAilment(canApplyFire, canApplyIce, canApplyLighting);
                return;
            }
            if (UnityEngine.Random.value < .5d && _iceDamage > 0)
            {
                canApplyIce = true;
                _targetStats.ApplyAilment(canApplyFire, canApplyIce, canApplyLighting);
                return;
            }
            if (UnityEngine.Random.value < .5d && _lightingDamage > 0)
            {
                canApplyLighting = true;
                _targetStats.ApplyAilment(canApplyFire, canApplyIce, canApplyLighting);
                return;
            }

        }

        if (canApplyFire)
            _targetStats.SetupFireDamage(Mathf.RoundToInt(_fireDamage * 0.1f));
        if (canApplyLighting)
            _targetStats.SetupLightingStrikeDamage(Mathf.RoundToInt(_lightingDamage * 0.2f));

        _targetStats.ApplyAilment(canApplyFire, canApplyIce, canApplyLighting);
    }

    //�������㷽��
    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    //״̬���㷽��
    public void ApplyAilment(bool _fire, bool _ice, bool _lighting)
    {
        bool canApplyFire = !isFire && !_ice && !isLighting;
        bool canApplyIce = !isFire  && !isLighting;
        bool canApplyLighting = !isFire && !_ice; 

        if (_fire && canApplyFire)
        {
            isFire =_fire;
            fireTimer = fireTimeDuration;
            fx.FireFxFor(defaultFireTimeDuration, defaultFireDamageCooldown);
        }
        if (_ice && canApplyIce)
        {
            
            isIce = _ice;
            iceTimer = iceTimeDuration;

            float slowPercentage = .5f;
            GetComponent<Entity>().SlowEntityBY(slowPercentage,defaultIceTimeDuration);
            fx.IceFxFor(defaultIceTimeDuration,0.5f);
        }
        if(_lighting && canApplyLighting)
        {
            if(!isLighting)
            {
                ApplyLighting(_lighting);

            }
            else
            {
                if(GetComponent<Player>() != null)
                    return;

                Collider2D[] colliers = Physics2D.OverlapCircleAll(transform.position, 10);
                float closestDistance = Mathf.Infinity;

                Transform closestEnemy = null;

                foreach (var hit in colliers)
                {
                    //�ж���Χ�Ƿ�����������ˣ����Ǵ����������׻���������
                    if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position,hit.transform.position) > 1)
                    {
                        float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                        if (distanceToEnemy < closestDistance)
                        {
                            closestDistance = distanceToEnemy;
                            closestEnemy = hit.transform;
                        }
                    }
                    //������Χû���������ˣ���ѡ���䱾��
                    if(closestEnemy == null)
                        closestEnemy = transform;

                }
                if (closestEnemy != null)
                {
                    GameObject newLightingStrike = Instantiate(lightingStrikePrefab, transform.position,Quaternion.identity);

                    newLightingStrike.GetComponent<LightingStrike_Controller>().Setup(beLightingDamage, closestEnemy.GetComponent<CharacterStats>());
                }
            }

            
        }

        isFire = _fire;
        isIce = _ice;
        isLighting = _lighting;
    }
    //����״̬����
    public void ApplyLighting(bool _lighting)
    {
        if (isLighting)
            return;

        lightingTimer = lightingTimeDuration;
        isLighting = _lighting;

        fx.LightingFxFor(defaultLightingTimeDuration, defaultLightingDamageCooldown);
    }

    //����״̬����
    private void ApplyFireDamage()
    {
        if (fireDamageTimer < 0 && isFire)
        {

            DecreaseHealthBy(fireStateDamage);

            if (currentHealth <= 0 && !isDead)
                Die();

            fireDamageTimer = fireDamageCooldown;
        }
    }

    public void SetupFireDamage(int _damage) => fireStateDamage = _damage;
    public void SetupLightingStrikeDamage(int _damage) => beLightingDamage = _damage;
    #endregion


    #region ���������ܣ�����ֵ���㷽��
    //�������ʼ��㷽��
    private bool CanCrit()
    {
        int totalCritChance = agility.GetValue() + critChance.GetValue();
        if(UnityEngine.Random.Range(0, 100) < totalCritChance)
        {
            return true;
        }
        return false;
    }

    //�����˺����㷽��
    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritDamage = (critPower.GetValue()+strength.GetValue())*0.01f;
        float critDamage = _damage * totalCritDamage;

        return Mathf.RoundToInt(critDamage);
    }

    //���ܼ��㷽��
    private bool CanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + agility.GetValue();

        if (isLighting)
            totalEvasion += 20;

        if (UnityEngine.Random.Range(0, 100) < totalEvasion)
        {
            Debug.Log("������");
            return true;
        }
        return false;
    }

    //buff����������ֵ
    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
    #endregion

    //����˺�����
    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    //Ѫ�����ӷ���
    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;

        if(currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();

        if(onHealthChanged != null)
            onHealthChanged();
    }

    //Ѫ���뵱ǰѪ����ȵķ���,Ѫ�����ٷ���
    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;

        GetComponent<Entity>().DamageEffect();
        fx.StartCoroutine("FlashFX");

        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    //���Ա仯����
    public Stat GetStat(StatType _statType)
    {
        if (_statType == StatType.strength)
            return strength;
        else if (_statType == StatType.agility)
            return agility;
        else if (_statType == StatType.intelligence)
            return intelligence;
        else if (_statType == StatType.vitality)
            return vitality;
        else if (_statType == StatType.damage)
            return damage;
        else if (_statType == StatType.critChance)
            return critChance;
        else if (_statType == StatType.critPower)
            return critPower;
        else if (_statType == StatType.maxHealth)
            return maxHealth;
        else if (_statType == StatType.armor)
            return armor;
        else if (_statType == StatType.evasion)
            return evasion;
        else if (_statType == StatType.magicResistance)
            return magicResistance;
        else if (_statType == StatType.fireDamage)
            return fireDamage;
        else if (_statType == StatType.iceDamage)
            return iceDamage;
        else if (_statType == StatType.lightingDamage)
            return lightingDamage;

        return null;
    }
}
