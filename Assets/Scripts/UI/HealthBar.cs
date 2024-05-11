using System.Collections;
using System.Collections.Generic;
using Codebase.Audio;
using Codebase.Core;
using UnityEngine;

public class HealthBar : FillBar
{
    public override void ChangeFillAmount(float amount)
    {
        base.ChangeFillAmount(amount);
        AudioManager.Instance.PlayOneShotSFX(AudioManager.Instance.Audios.healthReduce, AudioChannelData.CHANNEL_2);
        if(m_currentFillValue<=0)
        {
            GameplayEvents.SendOnGameOver();
        }
    }
}
