using UnityEngine;

using AmataWorld.Features.Synthesis;
using UnityEngine.Events;
using UnityEngine.SpatialTracking;
using AmataWorld.Activities;

namespace AmataWorld.Scene
{
    public class SceneConfig : MonoBehaviour
    {
        public static LayerMask GetSceneInteractableLayer() => LayerMask.NameToLayer("Scene Interactable");

        [field: Header("Settings")]
        [field: SerializeField] public Camera mainCamera { get; private set; }
        [field: SerializeField] public TrackedPoseDriver poseDriver { get; private set; }

        [field: Header("Services")]
        [field: SerializeField] public Features.Navigation.GoogleDirectionsService directionsService { get; private set; }

        [field: Header("Scene Anchoring")]
        [field: SerializeField] public FieldOfViewSpawnCoordinator fieldOfViewSpawnCoordinator { get; private set; }
        [field: SerializeField] public SceneInteractions sceneInteractions { get; private set; }
        [field: SerializeField] public DeviceRelativeSpawnCoordinator deviceRelativeSpawnCoordinator { get; private set; }
        [field: SerializeField] public Features.VPS.ARCoreVPSProvider vpsProvider { get; private set; }

        [field: Header("Prefabs")]
        [field: SerializeField] public GameObject placeholder3DPrefab { get; private set; }
        [field: SerializeField] public GameObject cryptexPuzzlePrefab { get; private set; }
        [field: SerializeField] public GameObject matchTheTilesPrefab { get; private set; }

        /// <summary>
        /// Invoked when the player is facing an object for at least one second
        /// </summary>
        [field: Header("Events")]
        [field: SerializeField] public UnityEvent<SceneInteractable> onFocusObject { get; private set; }
        /// <summary>
        /// Invoked when the player looks at a new interactable object
        /// </summary>
        [field: SerializeField] public UnityEvent<SceneInteractable> onTargetObject { get; private set; }
        [field: SerializeField] public UnityEvent<SceneInteractable> onActionObject { get; private set; }
        [field: SerializeField] public UnityEvent<IActivity> onActivityStarted { get; private set; }
        [field: SerializeField] public UnityEvent<IActivity, bool> onActivityEnded { get; private set; }
        /// <summary>
        /// Called when a Scene event has been triggered
        /// </summary>
        [field: SerializeField] public UnityEvent<Protobuf.SceneDef.Scene.Types.Event> onSceneEvent { get; private set; }
    }
}