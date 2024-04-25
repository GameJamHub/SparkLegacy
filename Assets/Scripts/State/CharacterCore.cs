using UnityEngine;
using UnityEngine.Serialization;

public abstract class CharacterCore : MonoBehaviour
{
        [FormerlySerializedAs("m_rigidBody")] public Rigidbody2D rigidBody;
        [FormerlySerializedAs("m_animator")] public Animator animator;
        public GroundSensor groundSensor;
        public LadderSensor ladderSensor;
        public StateMachine stateMachine;

        public bool m_isAttackPressed = false;

        public State state => stateMachine.state;

        public void Set(State newState, bool forceReset = false)
        {
                stateMachine.Set(newState, forceReset);
        }
        
        public void SetupInstances()
        {
                stateMachine = new StateMachine();
                State[] allChildStates = GetComponentsInChildren<State>();
                foreach (State childState in allChildStates)
                {
                        childState.SetCore(this);
                }
        }
}
