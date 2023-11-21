using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AmataWorld.Logging;
using DG.Tweening;
using Google.XR.ARCoreExtensions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace AmataWorld.Features.Navigation
{
    using SceneTypes = Protobuf.SceneDef.Scene.Types;

    public class DirectionsVisualizer : MonoBehaviour
    {
        [SerializeField]
        Scene.SceneConfig _sceneConfig;

        [SerializeField]
        GameObject _waypointPrefab;

        [SerializeField]
        GameObject _trailPrefab;

        Dictionary<uint, GoogleAPIs.Root> _directionsDict = new Dictionary<uint, GoogleAPIs.Root>();

        List<ARGeospatialAnchor> _waypoints = new List<ARGeospatialAnchor>();

        TrailManager _trailManager;

        void Awake()
        {
            var go = new GameObject("Trail manager");
            go.transform.SetParent(transform);
            go.SetActive(false);

            _trailManager = go.AddComponent<TrailManager>();
            _trailManager.Init(_trailPrefab, _sceneConfig.poseDriver);
        }

        void OnEnable()
        {
            _sceneConfig.onSceneEventTriggered.AddListener(OnSceneEvent);
        }

        void OnSceneEvent(SceneTypes.Event ev)
        {
            if (ev.Type.VariantCase != SceneTypes.Event.Types.EventType.VariantOneofCase.ShowDirections) return;

            var showDirections = ev.Type.ShowDirections;

            var geoPose = _sceneConfig.vpsProvider.earthManager.CameraGeospatialPose;

            _sceneConfig.onNotification.Invoke("Follow the trail for the next challenge");

            _sceneConfig.directionsService.GetDirectionsAsync(
                originLat: geoPose.Latitude,
                originLng: geoPose.Longitude,
                destinationLat: showDirections.Latitude,
                destinationLng: showDirections.Longitude,
                onResult: (res) =>
            {
                _directionsDict[ev.Id] = res;

                StartCoroutine(ExecAddRoutes(res));
            });
        }

        IEnumerator ExecAddRoutes(GoogleAPIs.Root root)
        {
            yield return _sceneConfig.vpsProvider.waitUntilReady;

            foreach (var waypoint in _waypoints)
            {
                Destroy(waypoint);
            }
            _waypoints.Clear();

            var route = root.routes.First();

            var distance = route.legs.Aggregate(0f, (total, leg) => leg.distance.value + total);

            this.LogDebug($"total distance = {distance}");

            foreach (var p in route.overview_polyline.parsedPoints)
            {
                // var promise = _sceneConfig.vpsProvider.anchorManager.ResolveAnchorOnTerrainAsync(p.lat, p.lng, 100, Quaternion.identity);
                // yield return promise;

                // var res = promise.Result;
                // if (res.TerrainAnchorState == TerrainAnchorState.Success && res.Anchor != null)
                // {

                // var go = Instantiate(_waypointPrefab, res.Anchor.transform);
                // go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                var anchor2 = _sceneConfig.vpsProvider.anchorManager.AddAnchor(p.lat, p.lng, 50.0f, Quaternion.identity);

                _waypoints.Add(anchor2);

                var go2 = Instantiate(_waypointPrefab, anchor2.transform);
                go2.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                this.LogDebug($"done adding new waypoint {go2.GetInstanceID()} @ ({p.lat}, {p.lng})");

                // }
                // else
                // {
                //     this.LogWarning($"failed to resolve terrain for ({p.lat}, {p.lng}). state = {res.TerrainAnchorState}");
                // }
            }

            _trailManager.Clear();
            yield return new WaitForSeconds(1.0f);

            _trailManager.gameObject.SetActive(true);
            _trailManager.SetWaypoints(_waypoints, distance);
        }

        class TrailManager : MonoBehaviour
        {
            GameObject _trailPrefab;

            TrackedPoseDriver _poseDriver;

            int _runId = 0;

            public void Init(GameObject trailPrefab, TrackedPoseDriver poseDriver)
            {
                _trailPrefab = trailPrefab;
                _poseDriver = poseDriver;
            }

            public void Clear()
            {
                _runId = 0;
                while (transform.childCount > 0) Destroy(transform.GetChild(0));
            }

            public void SetWaypoints(List<ARGeospatialAnchor> anchors, float distance)
            {
                Clear();

                StartCoroutine(ExecCreateTrails(anchors, distance));
            }

            IEnumerator ExecCreateTrails(List<ARGeospatialAnchor> anchors, float distance)
            {
                var totalTrailObjs = Mathf.CeilToInt(distance / 33.0f);
                var secondsPer100m = 8.0f;

                var delayBetweenSpawn = secondsPer100m * 100f / distance;

                var localRunId = anchors[0].GetInstanceID();
                _runId = localRunId;

                this.LogDebug($"num trails to create = {totalTrailObjs}");

                var trails = new List<GameObject>();
                for (var i = 0; i < totalTrailObjs; i++)
                {
                    var trail = Instantiate(_trailPrefab, transform);
                    trail.SetActive(false);

                    trails.Add(trail);

                    this.LogDebug($"instantiated trail obj = {trail.GetInstanceID()}");
                }

                while (_runId == localRunId)
                {
                    Tweener _firstTween = null;
                    foreach (var trail in trails)
                    {
                        // recompute the waypoint positions as they may have changed
                        var waypointPosArray = new Vector3[anchors.Count];
                        for (var i = 0; i < anchors.Count; i++)
                            waypointPosArray[i] = anchors[i].transform.position;

                        if (trail.activeSelf)
                            trail.transform.position = waypointPosArray[0];
                        else
                        {
                            // start from the player’s position
                            trail.transform.position = _poseDriver.transform.position;
                            trail.SetActive(true);
                        }

                        var tweener = trail.transform.DOPath(waypointPosArray, secondsPer100m, PathType.CatmullRom, PathMode.Ignore, 8);
                        tweener.Pause();
                        tweener.SetLoops(1);
                        tweener.Play();

                        if (_firstTween == null)
                            _firstTween = tweener;

                        this.LogDebug($"start trail for trail obj = {trail.GetInstanceID()}");

                        yield return new WaitForSeconds(delayBetweenSpawn);
                    }

                    if (_firstTween != null)
                        yield return _firstTween.WaitForCompletion();
                }
            }
        }
    }
}