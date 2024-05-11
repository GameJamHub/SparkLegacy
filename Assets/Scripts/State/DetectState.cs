using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectState : State
{
    public List<Transform> souls;
    
    public Transform target;

    public NavigateState navigateState;

    public NPCIdle idleState;

    public NPCShortAttack shortAttackState;

    public float detectRadius;

    public float vision = 1;

    public override void Enter()
    {
        base.Enter();
        navigateState.destination = target.position;
        Set(navigateState,true);
    }

    public override void Do()
    {
        if (state == navigateState)
        {
            if (CloseEnough(target.position))
            {
                Set(shortAttackState, true);
                m_characterCore.rigidBody.velocity = new Vector2(0, m_characterCore.rigidBody.velocity.y);
                return; 
            }
            else if(!InVision(target.position))
            {
                Set(idleState, true);
                m_characterCore.rigidBody.velocity = new Vector2(0, m_characterCore.rigidBody.velocity.y);
            }
            else
            {
                navigateState.destination = target.position;
                Set(navigateState, true);
            }
        }
        else
        {
            if (state.time > 2)
            {
                isComplete = true;
            }
        }

        if (target == null)
        {
            isComplete = true;
            return;
        }
    }

    public bool InVision(Vector2 targetPos)
    {
        return Vector2.Distance(m_characterCore.transform.position, targetPos) < vision;
    }

    public bool CloseEnough(Vector2 targetPos)
    {
        return Vector2.Distance(m_characterCore.transform.position, targetPos) < detectRadius;
    }

    public void CheckForTarget()
    {
        foreach (Transform soulTransform in souls)
        {
            if (InVision(soulTransform.position) && soulTransform.gameObject.activeSelf)
            {
                target = soulTransform;
                return;
            }
        }
        target = null;
    }
}
