using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "冻结敌人效果", menuName = "Data/Item Effect/FreezeEnemy Effect")]

public class FreezeTime_Enemy_Effect : ItemEffect
{
    [SerializeField] private float duration;
    public override void ExecuteEffect(Transform _transform)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats.currentHealth > playerStats.GetMaxHealthValue() * .5f)
            return;

        if (!Inventory.Instance.CanUseAmor())
        {
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, 2);

        foreach (var hit in colliders)
        {
            hit.GetComponent<Enemy>()?.FreezeTimeFor(duration);
           
        }
    }
}
