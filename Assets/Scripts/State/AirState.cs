using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : State
{
    private const string ANIM_JUMP = "Jump-Animation";

    private const string ANIM_JUMP_IDLE = "FallingIdle";
    private const string ANIM_JUMP_FALL = "Smoke_Falling";

    [SerializeField] private float m_jumpSpeed = 3.5f;
    [SerializeField] private float m_jumpForce = 1;

    [SerializeField] private Animator m_fallPFXAnimator;

    public float JumpForce => m_jumpForce;
    public float JumpSpeed => m_jumpSpeed;
    
    public override void Enter()
    {
        base.Enter();
        m_characterCore.animator.Play(ANIM_JUMP);
    }

    public override void Do()
    {
        base.Do();
        float time = Helpers.Map(m_characterCore.rigidBody.velocity.y, m_jumpSpeed * m_jumpForce, -m_jumpSpeed * m_jumpForce, 0, 1, true);
        m_characterCore.animator.Play(ANIM_JUMP,0,time);
        m_characterCore.animator.speed = 0;
        if (m_characterCore.groundSensor.isGrounded)
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
        if (m_characterCore.groundSensor.isGrounded)
        {
            m_fallPFXAnimator.Play(ANIM_JUMP_FALL);
        }
    }
}
