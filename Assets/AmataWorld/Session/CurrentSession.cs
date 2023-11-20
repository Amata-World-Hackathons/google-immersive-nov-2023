using System;
using UnityEngine;

namespace AmataWorld.Session
{
    public static class CurrentSession
    {
        private static SessionOrigin _currentSession;

        public static bool IsPlayer(GameObject obj)
        {
            return true;
        }

        public static void SetTo(SessionOrigin session)
        {
            _currentSession = session;
        }

        public static void UnsetIfEquals(SessionOrigin session)
        {
            if (_currentSession == session)
                _currentSession = null;
        }

        public static void Unset()
        {
            _currentSession = null;
        }
    }
}