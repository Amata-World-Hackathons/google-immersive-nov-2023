using System.Collections;
using UnityEngine;

namespace AmataWorld.Algorithms
{
    public abstract class StateBehaviour<T> : MonoBehaviour where T : MonoBehaviour, IStateMachineProvider<T>
    {
        public StateMachine<T> stateMachine { get; private set; }
        protected T source { get; set; }

        protected virtual void Awake()
        {
            source = GetComponent<T>();
            stateMachine = source.stateMachine;
        }

        protected Coroutine TransitionToAsync<S>() where S : StateBehaviour<T>
        {
            return stateMachine.TransitionToAsync<S>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextState"></param>
        public virtual void PrepareTransitionTo(StateBehaviour<T> nextState) { }

        /// <summary>
        /// Called prior to enabling/disabling the state components. Instead of
        /// relying on OnEnable or OnDisable to prepare state transitioning, of
        /// which the order of execution is not guaranteed, we use this method
        /// as a reliable way to prepare the state transition
        /// </summary>
        /// <param name="previousState"></param>
        public virtual void PrepareTransitionFrom(StateBehaviour<T> previousState) { }
    }
}