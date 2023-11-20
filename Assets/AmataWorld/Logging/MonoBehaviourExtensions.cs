using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmataWorld.Logging
{
    public static class MonoBehaviourExtensions
    {
        static int _counter = 0;
        static Dictionary<MonoBehaviour, Logger> _logCache = new Dictionary<MonoBehaviour, Logger>();

        static Logger GetLoggerFor(MonoBehaviour component)
        {
            if (_counter++ > 1000)
            {
                _counter = 0;
                int removedCount = 0;

                Debug.Log("Running periodic purge of logger references");

                foreach (var entry in _logCache)
                    if (!entry.Value.IsContextStillValid())
                    {
                        _logCache.Remove(entry.Key);
                        removedCount++;
                    }

                Debug.Log("Done, purged " + removedCount + " items");

                // do some cleanup of references
            }

            if (_logCache.TryGetValue(component, out var logger))
                return logger;
            else
            {
                logger = new Logger(component);

                _logCache.Add(component, logger);

                return logger;
            }
        }

        public static Logger GetLogger(this MonoBehaviour component)
        {
            return GetLoggerFor(component);
        }

        [HideInCallstack]
        public static void LogDebug(this MonoBehaviour component, string text)
        {
            GetLoggerFor(component).Debug(text);
        }

        [HideInCallstack]
        public static void LogInfo(this MonoBehaviour component, string text)
        {
            GetLoggerFor(component).Info(text);
        }

        [HideInCallstack]
        public static void LogWarning(this MonoBehaviour component, string text)
        {
            GetLoggerFor(component).Warn(text);
        }

        [HideInCallstack]
        public static void LogError(this MonoBehaviour component, string text)
        {
            GetLoggerFor(component).Error(text);
        }

        [HideInCallstack]
        public static void LogException(this MonoBehaviour component, Exception exception)
        {
            GetLoggerFor(component).Exception(exception);
        }
    }
}
