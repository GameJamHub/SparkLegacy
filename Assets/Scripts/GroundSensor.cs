using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    [SerializeField] private Transform m_leftGroundCheck;
    [SerializeField] private Transform m_rightGroundCheck;
    [SerializeField] private LayerMask m_groundLayer;
    [SerializeField] private float m_rayLength = 0.05f;
    
    public bool isGrounded { get; private set; }
    
    private void FixedUpdate()
    {
        CheckGround();
    }

    private void CheckGround()
    {
        RaycastHit2D leftCheckHit = Physics2D.Raycast(m_leftGroundCheck.position, Vector2.down,m_rayLength,m_groundLayer.value);
        RaycastHit2D rightCheckHit = Physics2D.Raycast(m_rightGroundCheck.position, Vector2.down, m_rayLength, m_groundLayer.value);
        isGrounded = (leftCheckHit || rightCheckHit);
    }
}
