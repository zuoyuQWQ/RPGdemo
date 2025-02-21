using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealtBar : MonoBehaviour
{
    private Entity entity;
    private RectTransform myTransform;
    private Slider slider;
    private CharacterStats myStat;

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        myStat = GetComponentInParent<CharacterStats>();
        entity = GetComponentInParent<Entity>();
        slider =GetComponentInChildren<Slider>();

        entity.onFlip += FlipUI;
        myStat.onHealthChanged += UpdateHealthUI;

        UpdateHealthUI();
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = myStat.GetMaxHealthValue();
        slider.value = myStat.currentHealth;
    }

    

    private void FlipUI() => myTransform.Rotate(0, 180, 0);

    private void Disable()
    {
        entity.onFlip -= FlipUI;
        myStat.onHealthChanged -= UpdateHealthUI;
    }
}
