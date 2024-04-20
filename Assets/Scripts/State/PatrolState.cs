using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    public NavigateState navigateState;

    public IdleState idleState;

    public Transform anchor1;

    public Transform anchor2;

    private void GoToNextDestination()
    {
        float randomSpot = Random.Range(anchor1.position.x, anchor2.position.x);
        navigateState.destination = new Vector2(randomSpot, m_characterCore.transform.position.y);
        Set(navigateState, true);
    }

    public override void Enter()
    {
        base.Enter();
        GoToNextDestination();
    }

    public override void Do()
    {
        base.Do();
        if (stateMachine.state == navigateState)
        {
            if (navigateState.isComplete)
            {
                Set(idleState, true);
                m_characterCore.rigidBody.velocity = new Vector2(0, m_characterCore.rigidBody.velocity.y);
            }
        }
        else
        {
            if (stateMachine.state.time > 1)
            {
                GoToNextDestination();
            }
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
