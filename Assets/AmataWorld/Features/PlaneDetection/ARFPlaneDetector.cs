using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace AmataWorld.Features.PlaneDetection
{
    public class ARFPlaneDetector : MonoBehaviour
    {
        public ARPlaneManager planeManager;

        void OnEnable()
        {
            planeManager.planesChanged += OnPlanesChanged;
        }

        void OnDisable()
        {
            planeManager.planesChanged -= OnPlanesChanged;
        }

        void OnPlanesChanged(ARPlanesChangedEventArgs args)
        {
        }
    }
}