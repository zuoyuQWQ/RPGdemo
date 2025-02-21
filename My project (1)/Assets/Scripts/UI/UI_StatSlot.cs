using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private string statName;

    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    public UI ui;

    [TextArea]
    [SerializeField] private string statDescription;

    
    private void OnValidate()
    {
        gameObject.name = "Stat - "+ statName;

        if(statNameText != null)
        {
            statNameText.text = statName;
        }
    }

    void Start()
    {
        ui = GetComponentInParent<UI>();
        UpdateStatValueUI();
    }

    public void UpdateStatValueUI()
    {
        PlayerStats playerstats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if(playerstats != null)
        {
            statValueText.text = playerstats.GetStat(statType).GetValue().ToString();

            if(statType == StatType.maxHealth)
                statValueText.text = playerstats.GetMaxHealthValue().ToString();
            if(statType == StatType.damage)
                statValueText.text = (playerstats.damage.GetValue() + playerstats.strength.GetValue()).ToString();
            if(statType == StatType.critPower)
                statValueText.text = (playerstats.critPower.GetValue() + playerstats.strength.GetValue()).ToString();
            if(statType == StatType.critChance)
                statValueText.text = (playerstats.critChance.GetValue() + playerstats.agility.GetValue()).ToString();
            if(statType == StatType.magicResistance)
                statValueText.text = (playerstats.magicResistance.GetValue() + playerstats.intelligence.GetValue() * 3).ToString();

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (statDescription == null)
            return;
        ui.statTooltip.ShowStatToolTip(statDescription);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (statDescription == null)
            return;
        ui.statTooltip.HideStatToolTip();
    }
}
