using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckState : State
{
    private const string ANIM_DUCK = "duck-Animation";

    [SerializeField] private PlayerMovement m_playerMovement;
    
    public override void Enter()
    {
        base.Enter();
        m_characterCore.animator.Play(ANIM_DUCK);
    }

    public override void Do()
    {
        base.Do();
        if (!m_characterCore.groundSensor.isGrounded || m_playerMovement.axisValue.y >= 0)
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
