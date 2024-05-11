using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCIdle : State
{
    [SerializeField] private string m_animIdle;
    
    public override void Enter()
    {
        base.Enter();
        m_characterCore.animator.Play(m_animIdle);
    }

    public override void Do()
    {
        base.Do();
        if (!m_characterCore.groundSensor.isGrounded)
        {
            isComplete = true;
        }
    }
}
