using System.Collections.Generic;
using System.Collections;
using AmataWorld.Activities;
using AmataWorld.Logging;
using UnityEngine;
using Google.XR.ARCoreExtensions;

namespace AmataWorld.Scene
{
    using SceneTypes = Protobuf.SceneDef.Scene.Types;
    using ObjectTypes = Protobuf.SceneDef.Scene.Types.Object.Types;

    public class SceneLayer
    {
        public readonly uint id;

        public readonly SceneTypes.Layer source;

        public readonly Dictionary<uint, SceneObject> objects;

        public SceneLayer(SceneTypes.Layer source, Dictionary<uint, SceneObject> objects)
        {
            id = source.Id;
            this.source = source;
            this.objects = objects;
        }
    }

    public class SceneCoordinator : MonoBehaviour
    {
        [SerializeField] SceneConfig _sceneConfig;

        Protobuf.SceneDef.Scene _importedScene;

        Dictionary<uint, SceneAnchor> _anchorsDict = new Dictionary<uint, SceneAnchor>();

        Dictionary<uint, SceneObject> _sceneObjectsDict = new Dictionary<uint, SceneObject>();

        Dictionary<uint, SceneLayer> _sceneLayersDict = new Dictionary<uint, SceneLayer>();

        void Awake()
        {
            _sceneConfig.onActionObject.AddListener(OnActionObject);
            _sceneConfig.onSceneEventTriggerIntent.AddListener(onSceneEventTriggerIntent);
        }

        void OnDestroy()
        {
            _sceneConfig.onSceneEventTriggerIntent.RemoveListener(onSceneEventTriggerIntent);
            _sceneConfig.onActionObject.RemoveListener(OnActionObject);
        }

        public void Init()
        {
        }

        public void Clear()
        {
        }

        void onSceneEventTriggerIntent(uint id)
        {
            foreach (var ev in _importedScene.Events)
            {
                if (ev.Id == id)
                {
                    _sceneConfig.onSceneEventTriggered.Invoke(ev);
                    return;
                }
            }
        }

        void OnActionObject(SceneInteractable sceneInteractable)
        {
            if (sceneInteractable.canBeActioned)
            {
                switch (sceneInteractable.target)
                {
                    case SceneObject:
                        {
                            var sceneObject = (SceneObject)sceneInteractable.target;
                            // var immersionConfig = sceneObject.activity.GetConfig();
                            // do something with immersion config

                            var activity = sceneObject.activity;
                            StartCoroutine(StartActivityAsync(activity));
                            break;
                        }

                    default:
                        this.LogWarning($"attempted to action an object with an unknown target = {sceneInteractable.target.GetType().Name}");
                        break;
                }
            }
        }

        IEnumerator StartActivityAsync(IActivity activity)
        {
            yield return activity.StartAsync((didComplete) =>
            {
                _sceneConfig.onActivityEnded.Invoke(activity, didComplete);
            });

            _sceneConfig.onActivityStarted.Invoke(activity);
        }

        bool TryLoadObject(SceneTypes.Object obj, out SceneObject sceneObject)
        {
            SceneAnchor sceneAnchor;
            if (_anchorsDict.TryGetValue(obj.AnchorId, out sceneAnchor))
            {
                var gameObject = new GameObject($"SceneObject#{obj.Id}");

                gameObject.transform.SetParent(sceneAnchor.transform);
                var pos = new Vector3(obj.Transform.PosX, obj.Transform.PosY, obj.Transform.PosZ);
                var rot = Quaternion.Euler(obj.Transform.EulerX, obj.Transform.EulerY, obj.Transform.EulerZ);
                gameObject.transform.localScale = new Vector3(obj.Transform.ScaleX, obj.Transform.ScaleY, obj.Transform.ScaleZ);
                gameObject.transform.SetLocalPositionAndRotation(pos, rot);

                sceneObject = gameObject.AddComponent<SceneObject>();
                var interactable = gameObject.AddComponent<SceneInteractable>();
                interactable.target = sceneObject;

                Activities.IActivity activity = null;
                switch (obj.Type.VariantCase)
                {
                    case ObjectTypes.ObjectType.VariantOneofCase.None:
                        // do nothing
                        break;

                    case ObjectTypes.ObjectType.VariantOneofCase.Custom:
                        // TODO something smarter
                        Instantiate(_sceneConfig.placeholder3DPrefab, gameObject.transform);
                        break;

                    case ObjectTypes.ObjectType.VariantOneofCase.ActivitySubject:
                        switch (obj.Type.ActivitySubject.Type.VariantCase)
                        {
                            case ObjectTypes.ActivitySubject.Types.ActivityType.VariantOneofCase.None:
                                // do nothing
                                break;

                            case ObjectTypes.ActivitySubject.Types.ActivityType.VariantOneofCase.CryptexPuzzle:
                                {
                                    var cryptexObj = Instantiate(_sceneConfig.cryptexPuzzlePrefab, gameObject.transform);
                                    var cryptexPuzzle = cryptexObj.GetComponent<Activities.CryptexPuzzle>();
                                    cryptexPuzzle.Init(obj.Type.ActivitySubject.Type.CryptexPuzzle);

                                    activity = cryptexPuzzle;

                                    break;
                                }

                            case ObjectTypes.ActivitySubject.Types.ActivityType.VariantOneofCase.MatchTheTiles:
                                {
                                    var matchTheTilesObj = Instantiate(_sceneConfig.matchTheTilesPrefab, gameObject.transform);
                                    var matchTheTiles = matchTheTilesObj.GetComponent<Activities.MatchTheTiles>();
                                    matchTheTiles.Init(obj.Type.ActivitySubject, _sceneConfig);

                                    activity = matchTheTiles;

                                    break;
                                }
                        }
                        break;
                }

                sceneObject.Init(obj, sceneAnchor, activity: activity);

                return true;
            }
            else
            {
                sceneObject = null;
                return false;
            }
        }

        bool TryLoadAnchor(SceneTypes.Anchor anchorData, out SceneAnchor sceneAnchor)
        {
            var gameObject = new GameObject($"SceneAnchor#{anchorData.Id}");

            switch (anchorData.Type.TypeCase)
            {
                case SceneTypes.AnchorType.TypeOneofCase.None:
                    this.LogWarning("unexpected None type while importing SceneAnchor");
                    break;

                case SceneTypes.AnchorType.TypeOneofCase.Plane:
                    {
                        sceneAnchor = _sceneConfig.fieldOfViewSpawnCoordinator.AddAnchor(gameObject, anchorData);
                        return true;
                    }

                case SceneTypes.AnchorType.TypeOneofCase.Geospatial:
                    break;

                case SceneTypes.AnchorType.TypeOneofCase.Image:
                    break;

                case SceneTypes.AnchorType.TypeOneofCase.DeviceRelative:
                    {
                        sceneAnchor = _sceneConfig.deviceRelativeSpawnCoordinator.AddAnchor(gameObject, anchorData);
                        return true;
                    }
            }

            sceneAnchor = null;
            return false;
        }

        bool TryLoadLayer(SceneTypes.Layer layer, out SceneLayer slayer)
        {
            var objs = new Dictionary<uint, SceneObject>();
            foreach (var obj in layer.Objects)
            {
                SceneObject sceneObject;
                if (TryLoadObject(obj, out sceneObject))
                    objs.Add(sceneObject.id, sceneObject);
                else
                    this.LogError($"failed to load SceneObject with ID = {obj.Id}");
            }

            slayer = new SceneLayer(layer, objs);

            return true;
        }

        public void LoadScene(Protobuf.SceneDef.Scene scene)
        {
            Clear();

            _importedScene = scene;

            foreach (var anchorData in scene.Anchors)
            {
                SceneAnchor sceneAnchor;
                if (TryLoadAnchor(anchorData, out sceneAnchor))
                    _anchorsDict.Add(sceneAnchor.id, sceneAnchor);
                else
                    this.LogError($"failed to load SceneAnchor with ID = {anchorData.Id}");
            }

            foreach (var layer in scene.Layers)
            {
                SceneLayer slayer;
                if (TryLoadLayer(layer, out slayer))
                    _sceneLayersDict.Add(slayer.id, slayer);
                else
                    this.LogError($"failed to load SceneLayer with ID = {layer.Id}");
            }
        }

        ARGeospatialAnchor _rewardAnchor;

        public void SetRewardLocation(double lat, double lng, float radius)
        {
            StartCoroutine(WatchLocation(lat, lng, radius));
        }

        IEnumerator WatchLocation(double lat, double lng, float radius)
        {
            yield return _sceneConfig.vpsProvider.waitUntilReady;

            _rewardAnchor = _sceneConfig.vpsProvider.anchorManager.AddAnchor(lat, lng, 50.0f, Quaternion.identity);

            while (true)
            {
                yield return new WaitForSeconds(2.0f);
                var pose = _sceneConfig.poseDriver;

                var diff = _rewardAnchor.pose.position - pose.transform.position;

                if (diff.magnitude < radius)
                {
                    _sceneConfig.onRewardEarned.Invoke();
                    yield return new WaitForSeconds(0.5f);
                    _sceneConfig.onNotification.Invoke("Well done! You found the hidden treasure");
                    yield break;
                }
            }
        }
    }
}