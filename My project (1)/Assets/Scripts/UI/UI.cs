using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionUI;


    public UI_ItemTooltip itemToolTip;
    public UI_StatTooltip statTooltip;
    // Start is called before the first frame update
    void Start()
    {
        SwitchTo(null);
        itemToolTip.gameObject.SetActive(false);
        statTooltip.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Tab))
        {
            SwitchTo(characterUI);
        }
        if(Input.GetKey (KeyCode.Escape))
        {
            SwitchTo(null);
        }
        if(Input.GetKey (KeyCode.Z))
        {
            SwitchTo(skillTreeUI);
        }
        if(Input.GetKey (KeyCode.X))
        {
            SwitchTo(craftUI);
        }
        if(Input.GetKey(KeyCode.C))
        {
            SwitchTo(optionUI);
        }
    }

    public void SwitchTo(GameObject _menu)
    {
        for(int i = 0;i<transform.childCount;i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_menu != null)
        {
            _menu.SetActive(true);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if(_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false) ;
            return;
        }
        SwitchTo(_menu);
    }
}
