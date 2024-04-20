using System;
using Codebase.Core;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : CharacterCore
{
   [SerializeField] private float m_maxAcceleration = 0.5f;
   [SerializeField] private float m_deadZoneThreshhold = 0.1f;
   [Range(0f,1f)] [SerializeField] private float m_groundDrag = 0.9f;
   [SerializeField] private IdleState m_idleState;
   [SerializeField] private RunState m_runState;
   [SerializeField] private AirState m_airState;
   [SerializeField] private DuckState m_duckState;

   private bool m_canMove;
   public Vector2 axisValue { get; private set; }
   
   private void OnEnable()
   {
      GameplayEvents.OnMovement += HandleOnMovement;
      GameplayEvents.OnJump += HandleOnJump;
   }

   private void Start()
   {
      SetupInstances();
      stateMachine.Set(m_idleState);
   }

   private void HandleOnMovement(Vector2 _axisValue, bool canMove)
   {
      axisValue = _axisValue.normalized;
      m_canMove = canMove;
   }

   private void Update()
   {
      SelectState();
      stateMachine.state.Do();
   }

   private void SelectState()
   {
      if (groundSensor.isGrounded)
      {
         if (axisValue.y < 0 && axisValue.x == 0)
         {
            stateMachine.Set(m_duckState);
         }
         else if (axisValue.x == 0)
         {
            stateMachine.Set(m_idleState);
         }
         else
         {
            stateMachine.Set(m_runState);
         }
      }
      else
      {
         stateMachine.Set(m_airState);
      }
   }

   private void FixedUpdate()
   {
      Move();
      ApplyFriction();
   }

   private void HandleOnJump()
   {
      if (groundSensor.isGrounded)
      {
         Jump();
      }
   }

   private void ApplyFriction()
   {
      if (groundSensor.isGrounded && rigidBody.velocity.y<=0f)
      {
         rigidBody.velocity *= m_groundDrag;
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
      transform.localScale = new Vector3(direction, 1, 1);
   }
   
   private void OnDisable()
   {
      GameplayEvents.OnMovement -= HandleOnMovement;
      GameplayEvents.OnJump -= HandleOnJump;
   }
}
