using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum StatType
{

    strength,//力量，增加伤害和暴击伤害
    agility,//敏捷。闪避和暴击几率
    intelligence,//智力,法伤和法抗
    vitality,//活力,生命值


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

    [Header("基础属性")]
    public Stat strength;//力量，增加伤害和暴击伤害
    public Stat agility;//敏捷。闪避和暴击几率
    public Stat intelligence;//智力,法伤和法抗
    public Stat vitality;//活力,生命值

    [Header("进攻属性")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;


    [Header("防御属性")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("魔法属性")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;


    [Header("状态")]
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
    private int beLightingDamage;//状态的伤害

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
    #region buff方法
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

    #region 物伤物抗模块
    //最后的物理伤害计算方法
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
        DoMagicDamage(_targetStats);//普通攻击也能造成法伤，不需要删除
    }

    //护甲减伤计算方法
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


    #region 法伤法抗模块
    //最后的魔法伤害计算方法
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

    //法抗计算方法
    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    //状态计算方法
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
                    //判断周围是否存在其他敌人，若是存在则优先雷击其他敌人
                    if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position,hit.transform.position) > 1)
                    {
                        float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                        if (distanceToEnemy < closestDistance)
                        {
                            closestDistance = distanceToEnemy;
                            closestEnemy = hit.transform;
                        }
                    }
                    //若是周围没有其他敌人，则选择其本身
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
    //光伤状态方法
    public void ApplyLighting(bool _lighting)
    {
        if (isLighting)
            return;

        lightingTimer = lightingTimeDuration;
        isLighting = _lighting;

        fx.LightingFxFor(defaultLightingTimeDuration, defaultLightingDamageCooldown);
    }

    //火伤状态方法
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


    #region 暴击，闪避，生命值计算方法
    //暴击几率计算方法
    private bool CanCrit()
    {
        int totalCritChance = agility.GetValue() + critChance.GetValue();
        if(UnityEngine.Random.Range(0, 100) < totalCritChance)
        {
            return true;
        }
        return false;
    }

    //暴击伤害计算方法
    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritDamage = (critPower.GetValue()+strength.GetValue())*0.01f;
        float critDamage = _damage * totalCritDamage;

        return Mathf.RoundToInt(critDamage);
    }

    //闪避计算方法
    private bool CanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + agility.GetValue();

        if (isLighting)
            totalEvasion += 20;

        if (UnityEngine.Random.Range(0, 100) < totalEvasion)
        {
            Debug.Log("闪避了");
            return true;
        }
        return false;
    }

    //buff后的最大生命值
    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
    #endregion

    //造成伤害方法
    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    //血量增加方法
    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;

        if(currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();

        if(onHealthChanged != null)
            onHealthChanged();
    }

    //血条与当前血量相等的方法,血量减少方法
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

    //属性变化方法
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
