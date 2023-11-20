using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace AmataWorld.Algorithms
{
    public interface IStateMachineProvider<T> where T : MonoBehaviour, IStateMachineProvider<T>
    {
        StateMachine<T> stateMachine { get; }
    }

    /// <summary>
    /// StateMachines are tied to a specific MonoBehaviour <typeparamref name="T"/>.
    /// Create implementations of the state that implements the <typeparamref name="IStateMachineState"/>
    /// interface and add them using AddState
    /// </summary>
    /// <typeparam name="T">The type of the component the StateMachine is attached to</typeparam>
    public class StateMachine<T> where T : MonoBehaviour, IStateMachineProvider<T>
    {
        public StateMachine(T component)
        {
            this.component = component;
        }

        public T component { get; private set; }

        public StateBehaviour<T> current { get; private set; }

        /// <summary>
        /// Registers a state with the machine. The first state added will be
        /// treated as the initial state of the machine
        /// </summary>
        /// <typeparam name="S">A MonoBehaviour which represents the state</typeparam>
        public void AddState<S>() where S : StateBehaviour<T>
        {
            var state = component.AddComponent<S>();

            if (current == null)
                current = state;
            else
                state.enabled = false;
        }

        public Coroutine TransitionToAsync<S>() where S : StateBehaviour<T>
        {
            // TODO handle transitional states here
            var state = component.GetComponent<S>();

            current.PrepareTransitionTo(state);
            state.PrepareTransitionFrom(current);

            current.enabled = false;
            state.enabled = true;

            current = state;

            return null;
        }
    }
}