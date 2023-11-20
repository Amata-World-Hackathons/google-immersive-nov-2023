using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using AmataWorld.Logging;

namespace AmataWorld.Platform
{
    public class DeviceLocationProvider : MonoBehaviour
    {
        [SerializeField]
        UnityEvent<LocationInfo> _onLocationChanged = new UnityEvent<LocationInfo>();

        LocationInfo _location = new LocationInfo();

        Coroutine _trackingCoroutine;

        public LocationInfo LastKnownLocation
        {
            get { return _location; }
        }

        public void AddOnLocationChangeListener(UnityAction<LocationInfo> onLocationChanged)
        {
            _onLocationChanged.AddListener(onLocationChanged);
        }

        public void RemoveOnLocationChangeListener(UnityAction<LocationInfo> onLocationChanged)
        {
            _onLocationChanged.RemoveListener(onLocationChanged);
        }

        void OnEnable()
        {
            if (_trackingCoroutine == null)
                _trackingCoroutine = StartCoroutine(StartTrackingLocation());
        }

        void OnDisable()
        {
            if (_trackingCoroutine != null)
            {
                StopCoroutine(_trackingCoroutine);
                Input.location.Stop();
                _trackingCoroutine = null;
            }
        }

        IEnumerator StartTrackingLocation()
        {
#if UNITY_ANDROID
            // TODO need to include some kind of wait, the permission isn’t immediately recognised
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
                UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);

            yield return new WaitForSeconds(2);
#endif

            if (!Input.location.isEnabledByUser)
            {
                this.LogDebug("Location services not enabled by the user");
                yield break;
            }

            // Input.compass.enabled = true;
            Input.location.Start();

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (maxWait < 1)
            {
                this.LogDebug("Timed out while waiting for location services");
                yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                this.LogDebug("Unable to determine device location");
                yield break;
            }

            while (true)
            {
                var lastData = Input.location.lastData;
                if (_location.timestamp != lastData.timestamp)
                {
                    _location = lastData;
                    _onLocationChanged.Invoke(_location);
                }

                yield return new WaitForSeconds(5);
            }
        }
    }
}