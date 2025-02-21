using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFx : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("受击特效")]
    [SerializeField] private float hitFxTime;
    [SerializeField] private Material hitMat;
    private Material originalMat;

    [Header("状态颜色")]
    [SerializeField] private Color[] iceColor;
    [SerializeField] private Color[] fireColor;
    [SerializeField] private Color[] lightingColor;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
        
    }

    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currenColor = sr.color;

        sr.color = Color.white;
        yield return new WaitForSeconds(hitFxTime);
        sr.color = currenColor;

        sr.material = originalMat;
    }

    private void RedColorBlink()
    {
        if (sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;
    }
    #region 燃烧效果
    public void FireFxFor(float _seconds,float _duration)
    {
        InvokeRepeating("FireColorFx",0,_duration);
        Invoke("CancelColorChange", _seconds);
    }

    private void FireColorFx()
    {
        if(sr.color != fireColor[0])
        {
            sr.color = fireColor[0];
        }
        else
        {
            sr.color = fireColor[1];
        } 
    }
    #endregion

    #region 冰冻效果
    public void IceFxFor(float _seconds,float _dutation)
    {
        InvokeRepeating("IceColorFx",0,_dutation);
        Invoke("CancelColorChange", _seconds);
    }

    private void IceColorFx()
    {
        if (sr.color != iceColor[0])
            sr.color = iceColor[0];
        else
            sr.color = iceColor[1];
    }
    #endregion

    #region 雷电效果
    public void LightingFxFor(float _seconds, float _duration)
    {
        InvokeRepeating("LightingColorFx", 0, _duration);
        Invoke("CancelColorChange", _seconds);
    }
    private void LightingColorFx()
    {
        if (sr.color != lightingColor[0])
            sr.color = lightingColor[0];
        else
            sr.color = lightingColor[1];
    }
    #endregion

    //隐身视觉效果
    public void MakeTransprent(bool _transprent)
    {
        if (_transprent)
        {
            sr.color = Color.clear;

        }
        else
        {
            sr.color = Color.white;
        }
    }


}
