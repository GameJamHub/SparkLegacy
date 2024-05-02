using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Codebase.Core;
using Codebase.Audio;

public class AudioScripts : MonoBehaviour
{
    public void AnimEvent_PlayFootsteps()
   {
      int Index = UnityEngine.Random.Range(0, AudioManager.Instance.Audios.playerFootSteps.Length);
      AudioManager.Instance.PlayOneShotSFX(AudioManager.Instance.Audios.playerFootSteps[Index],AudioChannelData.CHANNEL_2);      
   }

   private void AnimEvent_PlayClimbSFX()
   {
      int Index = UnityEngine.Random.Range(0, AudioManager.Instance.Audios.ladderClimb.Length);
      AudioManager.Instance.PlayOneShotSFX(AudioManager.Instance.Audios.ladderClimb[Index],AudioChannelData.CHANNEL_2);      
   }
}
