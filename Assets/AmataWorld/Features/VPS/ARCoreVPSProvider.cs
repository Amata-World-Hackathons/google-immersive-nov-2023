using System.Collections;
using AmataWorld.Logging;
using AmataWorld.Platform;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace AmataWorld.Features.VPS
{
    /// <summary>
    /// Wraps the ARCore VPS functionality
    /// </summary>
    [RequireComponent(typeof(AREarthManager))]
    public class ARCoreVPSProvider : MonoBehaviour
    {
        [SerializeField]
        ARCoreExtensions _arcoreExtensions;

        [SerializeField]
        public ARAnchorManager anchorManager { get; private set; }

        [SerializeField]
        DeviceLocationProvider _locationProvider;

        public AREarthManager earthManager { get; private set; }

        [HideInInspector]
        public VpsAvailabilityPromise VpsAvailability;

        public IEnumerator waitUntilReady { get; private set; }

        void Awake()
        {
            earthManager = GetComponent<AREarthManager>();

            waitUntilReady = new WaitUntil(() =>
            {
                return earthManager.EarthTrackingState == TrackingState.Tracking;
            });
        }

        public void StartVPS()
        {
            StartCoroutine(_StartVPSCoroutine());
        }

        void OnEnable()
        {
            _locationProvider.AddOnLocationChangeListener(OnLocationChanged);

            StartVPS();
        }

        void OnDisable()
        {
            _locationProvider.RemoveOnLocationChangeListener(OnLocationChanged);
        }

        void OnLocationChanged(LocationInfo location)
        {
            VpsAvailability = AREarthManager.CheckVpsAvailabilityAsync(location.latitude, location.longitude);
        }

        IEnumerator _StartVPSCoroutine()
        {
            yield return null;

            // #if UNITY_IOS
            //             var request = new TMP_ENDPOINTRequest();
            //             var resp = _api.Client.TMP_ENDPOINTAsync(request).GetAwaiter();

            //             var done = false;

            //             resp.OnCompleted(() =>
            //             {
            //                 done = true;
            //             });

            //             yield return new WaitUntil(() => done);

            //             var response = resp.GetResult();

            //             _anchorManager.SetAuthToken(response.Val);

            //             yield return new WaitForEndOfFrame();
            // #endif

            //             _arCoreExtensions.enabled = true;
            //             _earthManager.enabled = true;

            yield return new WaitForFixedUpdate();

            var support = earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
            if (support == FeatureSupported.Supported)
            {
                this.LogDebug("Geospatial API is supported");
                // this.gameObject.SetActive(true);
            }
            else
            {
                this.LogWarning("Geospatial API is not supported on this device");
            }

            yield return waitUntilReady;

            this.LogDebug("Earth state is now being tracked");

            // if (_onEarthManagerReady != null)
            // {
            //     _onEarthManagerReady.Invoke();

            //     _onEarthManagerReady.RemoveAllListeners();
            //     _onEarthManagerReady = null;
            // }

            // OnLocationChanged(_playerLocation.LastKnownLocation);
        }

        public string ToPrettyDebugString()
        {
            var geoPose = earthManager.CameraGeospatialPose;

            return "EarthState = " + earthManager.EarthState.ToString() + "\nEarthTrackingState = " + earthManager.EarthTrackingState.ToString() + "\nLatitude = " + geoPose.Latitude + "\nLongitude = " + geoPose.Longitude + "\nAltitude = " + geoPose.Altitude + "\nOrientation Yaw Accuracy = " + geoPose.OrientationYawAccuracy + "\nHorizontal Accuracy = " + geoPose.HorizontalAccuracy + "\nVertical Accuracy = " + geoPose.VerticalAccuracy + "\nVPS Availability = " + (VpsAvailability != null ? VpsAvailability.Result.ToString() : "N/A");
        }
    }
}