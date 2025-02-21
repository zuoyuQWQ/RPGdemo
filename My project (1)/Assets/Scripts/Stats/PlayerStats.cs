using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

    protected override void Start()
    {
        base.Start();
        player = PlayerManager.instance.player;
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();
        player.Die();
        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }

    //血量减少到一定程度时的效果
    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        ItemDataEquipment curretAmor = Inventory.Instance.GetEquipment(EquipmentType.Armor);

        if(curretAmor != null)
        {
            curretAmor.Effect(player.transform);
        }
    }
}
