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
                        Latitude = 51.50398354433733,
                        Longitude = -0.019888003256870058,
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
                                BoardSize = SceneTypes.Object.Types.ActivitySubject.Types.MatchTheTiles.Types.BoardSize.Four,
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

            _sceneCoordinator.LoadScene(scene);

            StartCoroutine(TestTest(ev2));
        }

        IEnumerator TestTest(SceneTypes.Event ev)
        {
            yield return new WaitForSeconds(3);

            _sceneConfig.onSceneEvent.Invoke(ev);
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