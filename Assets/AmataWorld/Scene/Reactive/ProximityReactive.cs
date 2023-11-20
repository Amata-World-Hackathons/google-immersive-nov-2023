using UnityEngine;

namespace AmataWorld.Scene.Reactive
{
    [RequireComponent(typeof(SceneObject))]
    public class ProximityReactive : MonoBehaviour
    {
        public float thresholdInMeters;
        public int bufferDurationInMilliseconds = 1000;

        bool _isCollidingWithPlayer;

        void Update()
        {
        }

        void OnCollisionEnter(Collision collision)
        {
            if (Session.CurrentSession.IsPlayer(collision.gameObject))
            {
                _isCollidingWithPlayer = true;
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (Session.CurrentSession.IsPlayer(collision.gameObject))
            {
                _isCollidingWithPlayer = false;
            }
        }
    }
}