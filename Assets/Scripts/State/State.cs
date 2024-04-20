using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    protected CharacterCore m_characterCore;
    
    protected float m_startTime;
    public bool isComplete { get; protected set; }
    public float time => Time.time - m_startTime;

    public StateMachine stateMachine;

    protected StateMachine parentStateMachine;

    public State state => stateMachine.state;

    public void Set(State newState, bool forceReset)
    {
        stateMachine.Set(newState, forceReset);
    }

    public virtual void Enter()
    {
        m_characterCore.animator.speed = 1;
        m_characterCore.rigidBody.gravityScale = 1;
    }
    public virtual void Do() {}
    public virtual void FixedDo() {}

    public void DoBranch()
    {
        Do();
        state?.DoBranch();
    }

    public void FixedDoBranch()
    {
        FixedDo();
        state?.FixedDoBranch();
    }
    
    public virtual void Exit() {}

    public void SetCore(CharacterCore characterCore)
    {
        stateMachine = new StateMachine();
        m_characterCore = characterCore;
    }

    public void Initialise(StateMachine parentMachine)
    {
        parentStateMachine = parentMachine;
        isComplete = false;
        m_startTime = Time.time;
    }
}
