using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace AmataWorld.Platform
{
    public class ARFoundationSessionOrigin : MonoBehaviour
    {
        [SerializeField]
        Session.SessionOrigin _sessionOrigin;

        [SerializeField]
        Features.PlaneDetection.ARFPlaneDetector _planeDetector;

        [SerializeField]
        Features.VPS.ARCoreVPSProvider _vpsProvider;

        void Awake()
        {
            _sessionOrigin = GetComponent<Session.SessionOrigin>();
            _planeDetector = GetComponent<Features.PlaneDetection.ARFPlaneDetector>();
        }
    }
}