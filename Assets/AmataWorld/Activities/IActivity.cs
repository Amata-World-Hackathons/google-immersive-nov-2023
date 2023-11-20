using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AmataWorld.Activities
{
    public interface IActivity
    {
        /// <summary>
        /// Starts the activity and returns a coroutine that completes when the
        /// stop process is complete. Returns null if the process is synchronous
        /// </summary>
        /// <param name="onExit"></param>
        /// <returns>A coroutine that executes the async code. null if synchronous</returns>
        public Coroutine StartAsync(UnityAction<bool> onExit);
        /// <summary>
        /// Stops the activity and returns a coroutine that completes when the
        /// stop process is complete. Returns null if the process is synchronous
        /// </summary>
        /// <returns>A coroutine that executes the async code. null if synchronous</returns>
        public Coroutine StopAsync();
    }
}