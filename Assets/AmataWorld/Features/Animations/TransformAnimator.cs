using Unity.VisualScripting;
using UnityEngine;

namespace AmataWorld.Features.Animations
{
    public class TransformAnimator : MonoBehaviour
    {
        private Vector3 _originalScale;
        private Vector3 _originalPos;
        private Quaternion _originalRot;

        void Awake()
        {
            _originalScale = gameObject.transform.localScale;
            _originalPos = gameObject.transform.position;
            _originalRot = gameObject.transform.rotation;
        }
    }
}