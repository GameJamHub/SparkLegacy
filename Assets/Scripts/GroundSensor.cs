using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    [SerializeField] private BoxCollider2D m_groundCollider;
    [SerializeField] private LayerMask m_groundLayer;
    
    public bool isGrounded { get; private set; }
    
    private void FixedUpdate()
    {
        CheckGround();
    }

    private void CheckGround()
    {
        isGrounded =
            Physics2D.OverlapAreaAll(m_groundCollider.bounds.min, m_groundCollider.bounds.max, m_groundLayer.value)
                .Length > 0;
    }
}
