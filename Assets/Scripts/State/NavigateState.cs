using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigateState : State
{
    public Vector2 destination;

    public float speed = 1;

    public float threshold = 0.1f;

    public RunState runState;

    public override void Enter()
    {
        base.Enter();
        Set(runState, true);
    }

    public override void Do()
    {
        base.Do();
        if (Vector2.Distance(m_characterCore.transform.position, destination) < threshold)
        {
            isComplete = true;
        }
        FaceDestination();
    }

    private void FaceDestination()
    {
        m_characterCore.transform.localScale = new Vector3(Mathf.Sign(m_characterCore.rigidBody.velocity.x), 1, 1);
    }
    
    public override void FixedDo()
    {
        base.FixedDo();
        Vector2 direction = (destination - (Vector2)m_characterCore.transform.position).normalized;
        m_characterCore.rigidBody.velocity = new Vector2(direction.x * speed, m_characterCore.rigidBody.velocity.y);
    }
}
