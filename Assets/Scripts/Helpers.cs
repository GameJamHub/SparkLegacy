using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static float Map(float value, float min1, float max1, float min2, float max2, bool clamp = false)
    {
        float val = min2 + (max2 - min2) * ((value - min1) / (max1 - min1));
        return clamp?Mathf.Clamp(val,Mathf.Min(min2,max2),Mathf.Max(min2,max2)):val;
    }

    public static float GetAnimationClipDuration(Animator animator, string animationClipName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if(clip.name!=animationClipName)
            {
                continue;
            }
            return clip.length;
        }
        return 0;
    }

    public static IEnumerator DelayAndExecute(Action callback, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

       callback();
    }
}
