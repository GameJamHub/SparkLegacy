using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    private const string ANIM_IDLE = "idle-Animation";
    
    public override void Enter()
    {
        base.Enter();
        m_characterCore.animator.Play(ANIM_IDLE);
    }

    public override void Do()
    {
        base.Do();
        if (!m_characterCore.groundSensor.isGrounded)
        {
            isComplete = true;
        }
    }

    public override void FixedDo()
    {
        base.FixedDo();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
