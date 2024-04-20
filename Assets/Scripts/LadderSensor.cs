using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderSensor : MonoBehaviour
{
    [SerializeField] private LayerMask m_ladderLayer;
    [SerializeField] private float m_radiusLength = 1;
    
    public bool isOnLadder { get; private set; }

    private void FixedUpdate()
    {
        CheckStatus();
    }

    private void CheckStatus()
    {
        isOnLadder = Physics2D.OverlapCircle(transform.position, m_radiusLength, m_ladderLayer.value);
    }
}
