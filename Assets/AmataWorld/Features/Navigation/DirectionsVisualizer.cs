using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AmataWorld.Logging;
using Google.XR.ARCoreExtensions;
using UnityEngine;

namespace AmataWorld.Features.Navigation
{
    using SceneTypes = Protobuf.SceneDef.Scene.Types;

    public class DirectionsVisualizer : MonoBehaviour
    {
        [SerializeField]
        Scene.SceneConfig _sceneConfig;

        [SerializeField]
        GameObject _waypointPrefab;

        Dictionary<uint, GoogleAPIs.Root> _directionsDict = new Dictionary<uint, GoogleAPIs.Root>();

        List<DirectionVisualization> _visualizations = new List<DirectionVisualization>();

        void OnEnable()
        {
            _sceneConfig.onSceneEvent.AddListener(OnSceneEvent);
        }

        void OnSceneEvent(SceneTypes.Event ev)
        {
            if (ev.Type.VariantCase != SceneTypes.Event.Types.EventType.VariantOneofCase.ShowDirections) return;

            var showDirections = ev.Type.ShowDirections;

            _sceneConfig.directionsService.GetDirectionsAsync((res) =>
            {
                _directionsDict[ev.Id] = res;

                this.LogDebug("received directions response");
                StartCoroutine(ExecAddRoutes(res));
            });
        }

        IEnumerator ExecAddRoutes(GoogleAPIs.Root root)
        {
            this.LogDebug("wait for VPS");
            yield return _sceneConfig.vpsProvider.waitUntilReady;
            this.LogDebug("done waiting for VPS");

            var visualization = new DirectionVisualization
            {
                raw = root,
                waypoints = new List<WeakReference<GameObject>>()
            };
            _visualizations.Add(visualization);

            foreach (var p in root.routes.First().overview_polyline.parsedPoints)
            {
                var promise = _sceneConfig.vpsProvider.anchorManager.ResolveAnchorOnTerrainAsync(p.lat, p.lng, 100, Quaternion.identity);
                yield return promise;

                var res = promise.Result;
                if (res.TerrainAnchorState == TerrainAnchorState.Success && res.Anchor != null)
                {
                    var go = Instantiate(_waypointPrefab, res.Anchor.transform);
                    go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                    visualization.waypoints.Add(new WeakReference<GameObject>(go));

                    var anchor2 = _sceneConfig.vpsProvider.anchorManager.AddAnchor(p.lat, p.lng, 50.0f, Quaternion.identity);
                    var go2 = Instantiate(_waypointPrefab, anchor2.transform);
                    go2.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                    this.LogDebug($"done adding new waypoint {go.GetInstanceID()} @ ({p.lat}, {p.lng})");
                }
                else
                {
                    this.LogWarning($"failed to resolve terrain for ({p.lat}, {p.lng}). state = {res.TerrainAnchorState}");
                }
            }
        }

        class DirectionVisualization
        {
            public GoogleAPIs.Root raw;
            public List<WeakReference<GameObject>> waypoints;
        }
    }
}