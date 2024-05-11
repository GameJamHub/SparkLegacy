using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRun : State
{

    [SerializeField] private float m_maxXSpeed = 2.5f;
    [SerializeField] private string m_animRunning;
    public float MaxSpeed => m_maxXSpeed;
    
    public override void Enter()
    {
        base.Enter();
        m_characterCore.animator.Play(m_animRunning);
    }

    public override void Do()
    {
        base.Do();
        float velX = m_characterCore.rigidBody.velocity.x;
        m_characterCore.animator.speed = Helpers.Map(m_maxXSpeed, 0, 1, 0, 1.6f, true);
      
        if (!m_characterCore.groundSensor.isGrounded || m_characterCore.rigidBody.velocity.x == 0)
        {
            isComplete = true;
        }
    }
}
