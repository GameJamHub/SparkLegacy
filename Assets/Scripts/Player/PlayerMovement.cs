using System;
using Codebase.Audio;
using Codebase.Core;
using UnityEngine;

public class PlayerMovement : CharacterCore
{
   private string ANIM_SHORT_ATTACK = "Lightning01";

   [SerializeField] private float m_maxAcceleration = 0.5f;
   [SerializeField] private float m_deadZoneThreshhold = 0.1f;
   [Range(0f,1f)] [SerializeField] private float m_groundDrag = 0.9f;
   [SerializeField] private IdleState m_idleState;
   [SerializeField] private RunState m_runState;
   [SerializeField] private AirState m_airState;
   [SerializeField] private DuckState m_duckState;
   [SerializeField] private ClimbingState m_climbingState;
   [SerializeField] private PlayerAttack m_playerAttack;
   [SerializeField] private Animator m_shortAttackAnimator;
   [SerializeField] private ThunderArrowSpawner m_thunderArrowSpawner;

   private bool m_canMove;
   public bool m_isShortAttack {get; private set;}
   public Vector2 axisValue { get; private set; }
   
   private void OnEnable()
   {
      GameplayEvents.OnMovement += HandleOnMovement;
      GameplayEvents.OnJump += HandleOnJump;
      GameplayEvents.OnShortAttackPerformed += HandleOnShortAttackPerformed;
      GameplayEvents.OnLongAttackPerformed += HandleOnLongAttackPerformed;
   }

   private void Start()
   {
      SetupInstances();
      stateMachine.Set(m_idleState);
   }

   private void HandleOnShortAttackPerformed()
   {
      m_isAttackPressed = true;
      m_isShortAttack = true;
      if(groundSensor.isGrounded)
      {
         Attack();
      }
   }

   private void HandleOnLongAttackPerformed()
   {
      m_isAttackPressed = true;
      m_isShortAttack = false;
      if(groundSensor.isGrounded)
      {
         Attack();
      }
   }

   private void HandleOnMovement(Vector2 _axisValue, bool canMove)
   {
      axisValue = _axisValue.normalized;
      m_canMove = canMove;
   }

   private void Update()
   {
      SelectState();
      stateMachine.state.DoBranch();
   }

   private void SelectState()
   {
      if (groundSensor.isGrounded)
      {
         if (axisValue.y < 0 && axisValue.x == 0)
         {
            stateMachine.Set(m_duckState);
         }
         else if(m_isAttackPressed)
         {
            stateMachine.Set(m_playerAttack, true);
         }
         else if (axisValue.x == 0)
         {
            stateMachine.Set(m_idleState);
         }
         else if(!m_isAttackPressed)
         {
            stateMachine.Set(m_runState);
         }
      }
      else if (ladderSensor.isOnLadder)
      {
         stateMachine.Set(m_climbingState);
      }
      else
      {
         stateMachine.Set(m_airState);
      }
   }

   private void FixedUpdate()
   {
      Move();
      // ApplyFriction();
      Climb();
   }

   private void HandleOnJump()
   {
      if (groundSensor.isGrounded || ladderSensor.isOnLadder)
      {
         Jump();
      }
   }

   private void Climb()
   {
      if (ladderSensor.isOnLadder)
      {
         rigidBody.velocity = new Vector2(rigidBody.velocity.x, m_climbingState.m_climbSpeed * axisValue.y);
      }
   }

   private void ApplyFriction()
   {
      if (groundSensor.isGrounded && rigidBody.velocity.y<=0f || ladderSensor.isOnLadder)
      {
         rigidBody.velocity *= m_groundDrag;
      }
   }

   private void Attack()
   {
      if(m_isShortAttack)
      {
         m_shortAttackAnimator.Play(ANIM_SHORT_ATTACK);
      }
      else
      {
         m_thunderArrowSpawner.Spawn(Vector3.right * transform.localScale.x);
      }
   }

   private void Jump()
   {
      rigidBody.velocity = new Vector2(rigidBody.velocity.x, m_airState.JumpForce * m_airState.JumpSpeed);
   }

   private void Move()
   {
      if (m_canMove && Math.Abs(axisValue.x)>m_deadZoneThreshhold)
      {
         float increment = axisValue.x * m_maxAcceleration;
         float newSpeed = Mathf.Clamp(rigidBody.velocity.x + increment, -m_runState.MaxSpeed, m_runState.MaxSpeed);
         rigidBody.velocity = new Vector2(newSpeed, rigidBody.velocity.y);
         FaceDirection();
      }
   }

   private void FaceDirection()
   {
      float direction = Mathf.Sign(axisValue.x);
      Vector3 newScale = transform.localScale;
      newScale.x= Mathf.Abs(newScale.x) *  direction;
      transform.localScale = newScale;
   }
   
   private void OnDisable()
   {
      GameplayEvents.OnMovement -= HandleOnMovement;
      GameplayEvents.OnJump -= HandleOnJump;
      GameplayEvents.OnShortAttackPerformed -= HandleOnShortAttackPerformed;
      GameplayEvents.OnLongAttackPerformed -= HandleOnLongAttackPerformed;
   }

   private void OnDrawGizmos()
   {
#if UNITY_EDITOR
      if(Application.isPlaying)
      {
         UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, "Active State : "+state);
      }
#endif

   }
}
