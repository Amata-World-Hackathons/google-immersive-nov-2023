using System;
// using System.Diagnostics;
using UnityEngine;

namespace AmataWorld.Logging
{
    public class Logger
    {
        WeakReference<UnityEngine.Object> _contextReference;
        string _debugPrefix;
        string _infoPrefix;
        string _warnPrefix;
        string _errorPrefix;

        public Logger(MonoBehaviour component)
        {
            _contextReference = new WeakReference<UnityEngine.Object>(component);

            var name = component.GetType().Name;
            if (Application.isEditor)
            {
                _debugPrefix = " <color=lightblue><b>DEBUG</b></color> <b>" + name + "</b>: ";
                _infoPrefix = " <color=cyan><b>INFO</b></color>  <b>" + name + "</b>: ";
                _warnPrefix = " <color=yellow><b>WARN</b></color>  <b>" + name + "</b>: ";
                _errorPrefix = " <color=red><b>ERROR</b></color> <b>" + name + "</b>: ";
            }
            else
            {
                _debugPrefix = " DEBUG " + name + ": ";
                _infoPrefix = " INFO  " + name + ": ";
                _warnPrefix = " WARN  " + name + ": ";
                _errorPrefix = " ERROR " + name + ": ";
            }
        }

        public bool IsContextStillValid()
        {
            return _contextReference.TryGetTarget(out var context);
        }

        // [Conditional("LOG_LEVEL_DEBUG")]
        [HideInCallstack]
        public void Debug(string text)
        {
            if (_contextReference.TryGetTarget(out var context))
                UnityEngine.Debug.Log(DateTime.UtcNow.ToString("o") + _debugPrefix + text, context);
        }

        // [Conditional("LOG_LEVEL_DEBUG")]
        [HideInCallstack]
        public void Info(string text)
        {
            if (_contextReference.TryGetTarget(out var context))
                UnityEngine.Debug.Log(DateTime.UtcNow.ToString("o") + _infoPrefix + text, context);
        }

        // [Conditional("LOG_LEVEL_DEBUG")]
        [HideInCallstack]
        public void Warn(string text)
        {
            if (_contextReference.TryGetTarget(out var context))
                UnityEngine.Debug.LogWarning(DateTime.UtcNow.ToString("o") + _warnPrefix + text, context);
        }

        // [Conditional("LOG_LEVEL_DEBUG")]
        [HideInCallstack]
        public void Error(string text)
        {
            if (_contextReference.TryGetTarget(out var context))
                UnityEngine.Debug.LogError(DateTime.UtcNow.ToString("o") + _errorPrefix + text, context);
        }

        // [Conditional("LOG_LEVEL_DEBUG")]
        [HideInCallstack]
        public void Exception(Exception exception)
        {
            if (_contextReference.TryGetTarget(out var context))
                UnityEngine.Debug.LogError(DateTime.UtcNow.ToString("o") + _errorPrefix + exception.ToString(), context);
        }
    }
}
