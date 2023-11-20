using System.Collections.Generic;
using AmataWorld.Logging;
using AmataWorld.Scene;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.ARFoundation;

namespace AmataWorld.Features.Synthesis
{
    public class FieldOfViewSpawnCoordinator : MonoBehaviour
    {
        [SerializeField]
        ARRaycastManager _raycastManager;

        // TODO update this to the new version when ARCore Extensions supports it
        [SerializeField]
        TrackedPoseDriver _trackedPoseDriver;

        List<ARRaycastHit> _raycastHits = new List<ARRaycastHit>();

        Dictionary<uint, SceneAnchor> _anchorsDict = new Dictionary<uint, SceneAnchor>();

        List<CriteriaTracker> _criteriaTrackers = new List<CriteriaTracker>();

        void Awake()
        {
        }

        void Update()
        {
            var len = _criteriaTrackers.Count;
            for (var i = len - 1; i >= 0; i--)
            {
                var tracker = _criteriaTrackers[i];

                if (tracker.DidMeetCriteria())
                {
                    if (_raycastManager.Raycast(tracker.ray, _raycastHits))
                    {
                        foreach (var hit in _raycastHits)
                        {
                            if (hit.trackable is ARPlane plane)
                            {
                                // TODO handle the scenario when the trackable is lost
                                var obj = tracker.sceneAnchor.gameObject;
                                obj.transform.SetParent(plane.transform, false);
                                // obj.transform.position = hit.pose.position;
                                obj.SetActive(true);

                                _criteriaTrackers.Remove(tracker);
                            }
                        }
                    }
                }
                else
                {
                    tracker.Update();
                }
            }
            // if (_raycastManager.Raycast(Input.GetTouch(0).position, _raycastHits))
            // {
            //     foreach (var hit in _raycastHits)
            //     {
            //         if (hit.trackable is ARPlane plane)
            //         {
            //             // do something
            //         }
            //     }
            // }
        }

        public void Init()
        {
        }

        public SceneAnchor AddAnchor(GameObject gameObject, Protobuf.SceneDef.Scene.Types.Anchor anchor)
        {
            if (anchor.Type.TypeCase != Protobuf.SceneDef.Scene.Types.AnchorType.TypeOneofCase.Plane)
            {
                this.LogError("tried to add an incompatible anchor");
                throw new System.Exception("tried to add an incompatible anchor into FieldOfViewSpawn");
            }

            gameObject.transform.SetParent(transform);
            gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            gameObject.SetActive(false);

            // world coordinates is very unstable, the object ends up flipping around
            // gameObject.transform.SetParent(null, true);

            var sceneAnchor = gameObject.AddComponent<SceneAnchor>();
            sceneAnchor.Init(anchor);

            _anchorsDict.Add(anchor.Id, sceneAnchor);

            if (anchor.Active)
            {
                var tracker = new CriteriaTracker(sceneAnchor);
                _criteriaTrackers.Add(tracker);
            }

            return sceneAnchor;
        }

        class CriteriaTracker
        {
            public readonly SceneAnchor sceneAnchor;
            public readonly Vector2 ray;
            float _score;

            public CriteriaTracker(SceneAnchor sceneAnchor)
            {
                this.sceneAnchor = sceneAnchor;
                ray = new Vector2(Screen.width / 2, Screen.height / 2);
                _score = 0.0f;
            }

            public void Update()
            {
                _score += 0.002f;
            }

            public bool DidMeetCriteria()
            {
                return _score > 0.0f;
            }
        }
    }
}