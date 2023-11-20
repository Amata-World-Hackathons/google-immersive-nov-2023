using System;
using System.Collections;
using System.Collections.Generic;
using AmataWorld.Logging;
using UnityEngine;

namespace AmataWorld.Utils.Async
{
    public static class MonoBehaviourExtensions
    {
        public class Promise<T> : IEnumerator<T>
        {
            public T Current => default;

            object IEnumerator.Current => this.Current;

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        public delegate IEnumerator AsyncFunc();

        // public static Coroutine StartSafeCoroutine(this MonoBehaviour component, IEnumerator enumerator)
        // {
        //     return component.StartCoroutine(WrapWithErrorHandling(component, enumerator));
        // }

        // private static IEnumerator WrapWithErrorHandling(MonoBehaviour component, IEnumerator enumerator)
        // {
        //     while (true)
        //     {
        //         object current;
        //         try
        //         {
        //             if (!enumerator.MoveNext()) break;
        //             current = enumerator.Current;
        //         }
        //         catch (Exception ex)
        //         {
        //             component.LogException(ex);
        //             yield break;
        //         }

        //         yield return current;
        //     }
        // }
    }
}