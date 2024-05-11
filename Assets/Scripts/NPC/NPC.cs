using System;
using UnityEngine;

public class NPC : CharacterCore
{

    [SerializeField] private string m_animDeath;
    [SerializeField] private Collider2D m_collider2D;
    private bool m_isDead;
    
    public PatrolState patrolState;
    public DetectState detectState;
    
    void Start()
    {
        SetupInstances();
        Set(patrolState);
    }
    
    void Update()
    {
        if (m_isDead)
        {
            return;
        }
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

    public void PlayDeath()
    {
        m_isDead = true;
        rigidBody.velocity = Vector2.zero;
        rigidBody.gravityScale = 0;
        animator.Play(m_animDeath);
        m_collider2D.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent.GetComponent<IDamage>().TakeDamage(-10f,-10,3,0.2f, transform);
        }
    }
}
