using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : CharacterCore
{
    public PatrolState patrolState;

    public DetectState detectState;
    
    void Start()
    {
        SetupInstances();
        Set(patrolState);
    }
    
    void Update()
    {
        if (state.isComplete)
        {
            if (state == detectState)
            {
                Set(patrolState);
            }
        }

        if (state == patrolState)
        {
            detectState.CheckForTarget();
            if (detectState.target != null)
            {
                Set(detectState);
            }
        }
        
        state.DoBranch();
    }

    private void FixedUpdate()
    {
        state.FixedDoBranch();
    }
}
