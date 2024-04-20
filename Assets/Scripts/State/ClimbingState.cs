using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingState : State
{
    private const string ANIM_CLIMB = "ladder-Animation";

    public float m_climbSpeed = 5f;
    
    public override void Enter()
    {
        base.Enter();
        m_characterCore.rigidBody.gravityScale = 0;
        m_characterCore.animator.Play(ANIM_CLIMB);
    }

    public override void Do()
    {
        base.Do();
        m_characterCore.animator.speed = Mathf.Abs(m_characterCore.rigidBody.velocity.y/m_climbSpeed/2f);
        if (!m_characterCore.ladderSensor.isOnLadder)
        {
            m_characterCore.animator.speed = 1;
            m_characterCore.rigidBody.gravityScale = 1;
            isComplete = true;
        }
    }
}
