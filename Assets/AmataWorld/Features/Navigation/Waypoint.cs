using UnityEngine;

namespace AmataWorld.Features.Navigation
{
    public class Waypoint : MonoBehaviour
    {
        [SerializeField]
        [Range(0.01f, 10.0f)]
        float _scale = 1.0f;

        void Awake()
        {
            transform.localScale = _scale * transform.localScale;
        }
    }
}