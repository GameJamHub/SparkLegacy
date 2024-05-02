using System.Collections;
using System.Collections.Generic;
using Codebase.Core;
using UnityEngine;

public class HealthBar : FillBar
{
    public override void ChangeFillAmount(float amount)
    {
        base.ChangeFillAmount(amount);
        if(m_currentFillValue<=0)
        {
            GameplayEvents.SendOnGameOver();
        }
    }
}
