using System.Collections;
using System.Collections.Generic;
using AmataWorld.Scene;
using Unity.VisualScripting;
using UnityEngine;

namespace AmataWorld.Session
{
    using SceneTypes = AmataWorld.Protobuf.SceneDef.Scene.Types;

    /// <summary>
    /// 
    /// </summary>
    public class SessionOrigin : MonoBehaviour
    {
        [SerializeField]
        SceneCoordinator _sceneCoordinator;

        [SerializeField]
        SceneConfig _sceneConfig;

        void OnEnable()
        {
            CurrentSession.SetTo(this);

            uint idgen = 1;

            var scene = new Protobuf.SceneDef.Scene();
            var anchor1 = new SceneTypes.Anchor
            {
                Id = idgen++,
                Active = true,
                Type = new SceneTypes.AnchorType
                {
                    // Plane = new SceneTypes.AnchorType.Types.Plane
                    // {
                    //     SurfaceOrientation = SceneTypes.AnchorType.Types.Plane.Types.SurfaceOrientation.Any,
                    //     RandomFieldOfView = new SceneTypes.AnchorType.Types.Plane.Types.RandomFieldOfView
                    //     {
                    //         X = 50,
                    //         Y = 50,
                    //         DeviceMovementScoreThreshold = 5,
                    //     }
                    // }
                    DeviceRelative = new SceneTypes.AnchorType.Types.DeviceRelative
                    {
                        Transform = new SceneTypes.AnchorType.Types.DeviceRelative.Types.Transform
                        {
                            OffsetX = 0,
                            OffsetY = 0,
                            OffsetZ = 1,
                            EulerX = 0,
                            EulerY = 0,
                            EulerZ = 0,
                        },
                        Alignment = SceneTypes.AnchorType.Types.DeviceRelative.Types.Alignment.CameraForward
                    }
                },
            };

            scene.Anchors.Add(anchor1);

            var layer1 = new SceneTypes.Layer
            {
                Id = idgen++,
            };

            var ev1 = new SceneTypes.Event
            {
                Id = idgen++,
                Type = new SceneTypes.Event.Types.EventType
                {
                    ToggleLayers = new SceneTypes.Event.Types.ToggleLayers
                    {
                        LayerIds = { layer1.Id },
                        Behaviour = SceneTypes.Event.Types.ToggleLayers.Types.Behaviour.Enable,
                    }
                }
            };
            scene.Events.Add(ev1);

            var ev2 = new SceneTypes.Event
            {
                Id = idgen++,
                Type = new SceneTypes.Event.Types.EventType
                {
                    ShowDirections = new SceneTypes.Event.Types.ShowDirections
                    {
                        Latitude = 51.50403455601147,
                        Longitude = -0.02005565121979416,
                    }
                }
            };
            scene.Events.Add(ev2);

            var obj1 = new SceneTypes.Object
            {
                Id = idgen++,
                AnchorId = anchor1.Id,
                Transform = new SceneTypes.Transform
                {
                    PosX = 0,
                    PosY = 0,
                    PosZ = 0,
                    EulerX = 0,
                    EulerY = 0,
                    EulerZ = 0,
                    ScaleX = 1,
                    ScaleY = 1,
                    ScaleZ = 1,
                },
                Type = new SceneTypes.Object.Types.ObjectType
                {
                    ActivitySubject = new SceneTypes.Object.Types.ActivitySubject
                    {
                        TriggersEventId = ev2.Id,
                        Type = new SceneTypes.Object.Types.ActivitySubject.Types.ActivityType
                        {
                            // CryptexPuzzle = new SceneTypes.Object.Types.ActivitySubject.Types.CryptexPuzzle
                            // {
                            //     Answer = "deaf",
                            //     Alphabet = "abcdefghi",
                            //     Hint = "This is a sample hint"
                            // }
                            MatchTheTiles = new SceneTypes.Object.Types.ActivitySubject.Types.MatchTheTiles
                            {
                                BoardSize = 3,
                                PhysicalBoardLengthMm = 800,
                            }
                        }
                    }
                    // Custom = new SceneTypes.Object.Types.Custom
                    // {
                    //     Interaction = new SceneTypes.Object.Types.Interaction
                    //     {
                    //         Focus = new SceneTypes.Object.Types.Interaction.Types.Focus
                    //         {
                    //             EventId = ev1.Id,
                    //         },
                    //     },
                    // }
                },
            };

            obj1.Type.ActivitySubject.Type.MatchTheTiles.Assets.Add(new SceneTypes.Object.Types.ActivitySubject.Types.MatchTheTiles.Types.TileAsset());

            layer1.Objects.Add(obj1);

            scene.Layers.Add(layer1);

            _sceneConfig.onSceneEventTriggered.AddListener((ev) =>
            {
                if (ev.Type.VariantCase != SceneTypes.Event.Types.EventType.VariantOneofCase.ShowDirections) return;

                _sceneCoordinator.SetRewardLocation(ev2.Type.ShowDirections.Latitude, ev2.Type.ShowDirections.Longitude, 80.0f);
            });

            StartCoroutine(AsyncInit(scene, ev2));
        }

        IEnumerator AsyncInit(Protobuf.SceneDef.Scene scene, SceneTypes.Event ev)
        {
            yield return new WaitForSeconds(2);
            _sceneConfig.onNotification.Invoke("How to play: Complete each challenge to earn a reward at the end");

            yield return new WaitForSeconds(3);
            _sceneCoordinator.LoadScene(scene);

            yield return new WaitForSeconds(3);

            // _sceneConfig.onRewardEarned.Invoke();
            // _sceneConfig.onNotification.Invoke("hello world");
            // _sceneConfig.onSceneEventTriggered.Invoke(ev);
        }

        void OnDisable()
        {
            // CurrentSession.UnsetIfEquals(this);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}