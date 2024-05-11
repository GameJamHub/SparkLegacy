using System;
using System.Collections;
using Codebase.Audio;
using Codebase.Core;
using UnityEngine;
using UnityEngine.XR;

public class PlayerMovement : CharacterCore, IDamage, IAbsorbElectric
{
   private string ANIM_SHORT_ATTACK = "Lightning01";
   private string ANIM_HURT= "Hurt-Animation";

   [SerializeField] private float m_maxAcceleration = 0.5f;

   [SerializeField] private float m_maxDeceleration = 0.5f;
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
   [SerializeField] private int m_maxJumpCounts = 2;
   [SerializeField] private SpriteRenderer m_afterTrailSprite;
   [SerializeField] private SpriteRenderer m_playerSprite;
   [SerializeField] private Color m_afterTrailColor;
   [SerializeField] private float m_afterTrailLifeTime;
   [SerializeField] private float m_timeBetweenAfterTrail;
   [SerializeField] private float m_dashhSpeed;
   [SerializeField] private float m_dashTime;

   private bool m_canMove;

   private Collectable m_collectable;

   private bool m_isGameOver = false;
   private bool m_canClimb = true;

   private bool m_isDashing = false;

   private float m_afterTrailCounter,m_dashCounter;
   private bool m_knockBack = false;

   public bool m_isShortAttack {get; private set;}
   public Vector2 axisValue { get; private set; }


   private int m_currentJumpCount;
   
   private void OnEnable()
   {
      GameplayEvents.OnMovement += HandleOnMovement;
      GameplayEvents.OnJump += HandleOnJump;
      GameplayEvents.OnShortAttackPerformed += HandleOnShortAttackPerformed;
      GameplayEvents.OnLongAttackPerformed += HandleOnLongAttackPerformed;
      GameplayEvents.OnDashPerformed += HandleOnDashPerformed;
      GameplayEvents.OnAbsorbPerformed+=  HandleOnAbsorbPerformed;
      GameplayEvents.OnGameOver += HandleOnGameover;
   }

   private void Start()
   {
      SetupInstances();
      stateMachine.Set(m_idleState);
   }

   public void TakeDamage(float amount, float forceX = 0f, float forceY = 0f, float duration = 0f, Transform otherTransform = null)
   {
      AudioManager.Instance.PlayOneShotSFX(AudioManager.Instance.Audios.playerDamageTaken,AudioChannelData.CHANNEL_2);
      animator.Play(ANIM_HURT);
      GameManager.Instance.UpdateHealthBar(amount);
      StartCoroutine(HurtEffect(duration/1.3f));
      StartCoroutine(KnockBack(forceX,forceY, duration, otherTransform));
   }

   private IEnumerator HurtEffect(float duration)
   {
      float currentTime = 0;
      bool isNormalAlpha = false;
      while (currentTime<duration)
      {
         m_playerSprite.color = isNormalAlpha ? Color.white : Color.red;
         currentTime += Time.deltaTime * 5f;
         isNormalAlpha = !isNormalAlpha;
         yield return new WaitForSeconds(0.1f);
      }
      m_playerSprite.color = Color.white;
   }

   public void Absorb(float amount)
   {
   GameManager.Instance.UpdateElectricBar(amount);
   }

   public void Detection(Collectable collectable)
   {
      m_collectable = collectable;
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
      if(GameManager.Instance.CanUseElectric(3f))
      {
         m_isAttackPressed = true;
         m_isShortAttack = false;
         if(groundSensor.isGrounded)
         {
            GameManager.Instance.UpdateElectricBar(-3f);
            Attack();
         }
      }
   }

   private void HandleOnAbsorbPerformed()
   {
      if(m_collectable!=null)
      {
         AudioManager.Instance.PlayOneShotSFX(AudioManager.Instance.Audios.sparkAbsorb,AudioChannelData.CHANNEL_2);
         GameManager.Instance.UpdateElectricBar(m_collectable.absorbAmount);
         Destroy(m_collectable.gameObject);
      }
   }

   private void HandleOnMovement(Vector2 _axisValue, bool canMove)
   {
      axisValue = _axisValue.normalized;
      m_canMove = canMove;
   }

   private void Update()
   {
      if(m_isGameOver)
      {
         return;
      }
      SelectState();
      stateMachine.state.DoBranch();
      if(m_dashCounter>0)
      {
         m_isDashing = true;
         Dash();
      }
      else if(m_isDashing)
      {
         m_isDashing = false;
         rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
         if(groundSensor.isGrounded)
         {  
            stateMachine.Set(m_idleState,true);
         }
      }
   }

   private void HandleOnDashPerformed()
   {
      if(GameManager.Instance.CanUseElectric(5f))
      {
         AudioManager.Instance.PlayOneShotSFX(AudioManager.Instance.Audios.dash,AudioChannelData.CHANNEL_2);
         GameManager.Instance.UpdateElectricBar(-5f);
         m_dashCounter = m_dashTime;
         if(groundSensor.isGrounded)
         {
            stateMachine.Set(m_runState);
         }
         ShowAfterImage();
      }
   }

   private void Dash()
   {
      m_dashCounter -= Time.deltaTime;
      rigidBody.velocity = new Vector2(m_dashhSpeed * transform.localScale.x, rigidBody.velocity.y);
      m_afterTrailCounter -= Time.deltaTime;
      if (m_afterTrailCounter <=0)
      {
         ShowAfterImage();
      }
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
      else if (ladderSensor.isOnLadder && m_canClimb)
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
      if(m_isGameOver)
      {
         return;
      }
      if(m_knockBack)
      {
         return;
      }
      Move();
      // ApplyFriction();
      Climb();
   }

   private void HandleOnJump()
   {
      if (groundSensor.isGrounded)
      {
         m_currentJumpCount = 0;
         m_currentJumpCount++;
         Jump();
      }
      else if (ladderSensor.isOnLadder)
      {
         m_currentJumpCount = 0;
         m_currentJumpCount++;
         m_canClimb = false;
         animator.speed = 1;
         rigidBody.gravityScale = 1;
         stateMachine.Set(m_airState);
         Jump();
      }
      else if (m_currentJumpCount<m_maxJumpCounts)
      {
         m_currentJumpCount++;
         Jump();
      }
   }

   private void Climb()
   {
      if (ladderSensor.isOnLadder && m_canClimb)
      {
         rigidBody.velocity = new Vector2(rigidBody.velocity.x, m_climbingState.m_climbSpeed * axisValue.y);
      }
      else if (groundSensor.isGrounded)
      {
         m_canClimb = true;
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
         AudioManager.Instance.PlayOneShotSFX(AudioManager.Instance.Audios.playerShortAttack,AudioChannelData.CHANNEL_2);
      }
      else
      {
         m_thunderArrowSpawner.Spawn(Vector3.right * transform.localScale.x);
         AudioManager.Instance.PlayOneShotSFX(AudioManager.Instance.Audios.playerLongAttack,AudioChannelData.CHANNEL_2);
      }
   }

   private void Jump()
   {
      AudioManager.Instance.PlayOneShotSFX(AudioManager.Instance.Audios.playerJump,AudioChannelData.CHANNEL_2);
      rigidBody.velocity = new Vector2(rigidBody.velocity.x, m_airState.JumpForce * m_airState.JumpSpeed);
   }

   private void Move()
   {
      if (m_canMove && Math.Abs(axisValue.x)>m_deadZoneThreshhold && !m_isDashing)
      {
         float increment = axisValue.x * m_maxAcceleration;
         float newSpeed = Mathf.Clamp(rigidBody.velocity.x + increment, -m_runState.MaxSpeed, m_runState.MaxSpeed);
         newSpeed*=Time.deltaTime;
         rigidBody.velocity = new Vector2(newSpeed, rigidBody.velocity.y);
         FaceDirection();
      }
      else if(Mathf.Abs(axisValue.x)==0 && m_dashCounter<=0)
      {
         float decrement = m_maxDeceleration * Time.deltaTime * m_runState.MaxSpeed;
         if(rigidBody.velocity.x<0)
         {
            rigidBody.velocity = new Vector2(decrement, rigidBody.velocity.y);
            if(rigidBody.velocity.x>0)
            {
               rigidBody.velocity = new Vector2(0,rigidBody.velocity.y);
            }
         }
         else if(rigidBody.velocity.x>0)
         {
            rigidBody.velocity = new Vector2(-decrement, rigidBody.velocity.y);
            if(rigidBody.velocity.x<0)
            {
               rigidBody.velocity = new Vector2(0,rigidBody.velocity.y);
            }
         }
      }
   }

   private void FaceDirection()
   {
      float direction = Mathf.Sign(axisValue.x);
      Vector3 newScale = transform.localScale;
      newScale.x= Mathf.Abs(newScale.x) *  direction;
      transform.localScale = newScale;
   }

   private void ShowAfterImage()
    {
        SpriteRenderer image = Instantiate(m_afterTrailSprite, transform.position, transform.rotation);
        image.sprite = m_playerSprite.sprite;
        image.transform.localScale = transform.localScale;
        image.color = m_afterTrailColor;
        Destroy(image.gameObject,m_afterTrailLifeTime);
        m_afterTrailCounter = m_timeBetweenAfterTrail;
    }

   public IEnumerator KnockBack(float forceX,float forceY, float duration, Transform otherobject)
   {
      int knockBackDirection;
      if (transform.position.x < otherobject.position.x)
         knockBackDirection = -1;
      else
         knockBackDirection = 1;

      m_knockBack = true;
      rigidBody.velocity = Vector2.zero;
      Vector2 theForce = new Vector2(knockBackDirection * forceX, forceY);
      rigidBody.AddForce(theForce, ForceMode2D.Impulse);
      yield return new WaitForSeconds(duration);
      m_knockBack = false;
      rigidBody.velocity = Vector2.zero;
   }

   private void HandleOnGameover()
   {
      m_isGameOver = true;
   }
   
   private void OnDisable()
   {
      GameplayEvents.OnMovement -= HandleOnMovement;
      GameplayEvents.OnJump -= HandleOnJump;
      GameplayEvents.OnShortAttackPerformed -= HandleOnShortAttackPerformed;
      GameplayEvents.OnLongAttackPerformed -= HandleOnLongAttackPerformed;
      GameplayEvents.OnDashPerformed -= HandleOnDashPerformed;
      GameplayEvents.OnAbsorbPerformed-=  HandleOnAbsorbPerformed;
      GameplayEvents.OnGameOver -= HandleOnGameover;
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
