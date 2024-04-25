using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShortAttack : State
{

    private const string ANIM_RUN_SHOOT = "Run_shoot_Animation";
    private const string ANIM_IDLE_SHOOT = "Shoot_Animation";

    [SerializeField] private PlayerMovement m_playerMovement;

    private string m_animClipName;

    public override void Enter()
    {
        base.Enter();
        if(m_playerMovement.axisValue.x==0)
        {
            m_animClipName = ANIM_IDLE_SHOOT;
        }
        else if(Mathf.Abs(m_characterCore.rigidBody.velocity.x)>0)
        {
            m_animClipName = ANIM_RUN_SHOOT;
        }
        m_characterCore.animator.Play(m_animClipName);
        StartCoroutine(Helpers.DelayAndExecute(()=>{
                isComplete = true;
                m_characterCore.m_isAttackPressed = false;
            },Helpers.GetAnimationClipDuration(m_characterCore.animator,m_animClipName)));
    }

    public override void Do()
    {
        base.Do();
        if(!m_characterCore.groundSensor.isGrounded || m_playerMovement.axisValue.y<0)
        {
            isComplete = true;
        }
    }
}
