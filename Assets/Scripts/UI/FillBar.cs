using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class FillBar : MonoBehaviour
{
    [SerializeField] protected Image m_barImage;
    [SerializeField] private float m_initialFill = 100f;
    [SerializeField] private float m_maxFillValue = 100f;

    protected float m_currentFillValue;

    public float ReserveAmount => m_currentFillValue;

    private  void Start()
    {
        m_barImage.fillAmount = m_initialFill/m_maxFillValue;
        m_currentFillValue = m_initialFill;
    }
    
    public virtual void ChangeFillAmount(float amount)
    {
        m_currentFillValue+=amount;
        m_barImage.fillAmount = m_currentFillValue/m_maxFillValue;
    }
}
