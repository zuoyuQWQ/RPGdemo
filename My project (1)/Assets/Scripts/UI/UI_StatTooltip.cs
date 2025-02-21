using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StatTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemDescription;


    public void ShowStatToolTip(string _text)
    {
        if(_text == null)
            return;
        itemDescription.text = _text;
        gameObject.SetActive(true);
    }

    public void HideStatToolTip()
    {
        gameObject.SetActive(false);
    }
}
