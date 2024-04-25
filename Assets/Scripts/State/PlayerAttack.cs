using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : State
{
    [SerializeField] private PlayerShortAttack m_shortAttack;
    [SerializeField] private PlayerLongAttack m_longAttack;

    [SerializeField] private PlayerMovement m_playerMovement;

    public override void Enter()
    {
        base.Enter();
        Set(m_playerMovement.m_isShortAttack?m_shortAttack:m_longAttack,true);
    }

    public override void Do()
    {
        base.Do();
        if(state.isComplete)
        {
            isComplete = true;
            m_characterCore.m_isAttackPressed = false;
        }
    }
}
