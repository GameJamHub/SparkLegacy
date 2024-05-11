using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCShortAttack : State
{
    public string animShortAttack;
    
    public override void Enter()
    {
        base.Enter();
        m_characterCore.animator.Play(animShortAttack);
        StartCoroutine(Helpers.DelayAndExecute(()=>{
            isComplete = true;
        },Helpers.GetAnimationClipDuration(m_characterCore.animator,animShortAttack)));
    }
}
