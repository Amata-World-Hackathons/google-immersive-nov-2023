using AmataWorld.Logging;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AmataWorld.Scene
{
    public class SceneInteractions : MonoBehaviour
    {
        [SerializeField]
        SceneConfig _sceneConfig;

        [SerializeField]
        [Range(0.0f, 0.1f)]
        float _offset = 0.02f;

        RaycastHit _hit;

        public SceneInteractable currentFocus { get; private set; }
        public SceneInteractable currentTarget { get; private set; }

        int counter = 0;

        LayerMask _layerMask;

        void Awake()
        {
            _layerMask = ~SceneConfig.GetSceneInteractableLayer();
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            Mouse mouse = Mouse.current;
            if (mouse.leftButton.wasPressedThisFrame)
            {
                var mousePos = mouse.position.ReadValue();
                var ray = _sceneConfig.mainCamera.ScreenPointToRay(mousePos);

                this.LogDebug($"mouse clicked CNTR={counter}");

                if (Physics.Raycast(ray, out _hit, 50.0f, _layerMask))
                {
                    var interactable = _hit.collider.gameObject.GetComponentInParent<SceneInteractable>();

                    if (interactable != null)
                    {
                        ProcessTargetObjectInCurrentFrame(interactable);
                        return;
                    }

                }

                if (counter++ > 5) ProcessTargetObjectInCurrentFrame(null);
                return;
            }

#else

            var pose = _sceneConfig.poseDriver;
            if (Physics.SphereCast(pose.transform.position + pose.transform.forward * _offset, 0.05f, pose.transform.forward, out _hit, 15, _layerMask))
            {
                var interactable = _hit.collider.gameObject.GetComponentInParent<SceneInteractable>();

                ProcessTargetObjectInCurrentFrame(interactable);
            }
            else if (currentFocus != null)
            {
                ProcessTargetObjectInCurrentFrame(null);
            }
#endif
        }

        void ProcessTargetObjectInCurrentFrame(SceneInteractable interactable)
        {
            if (interactable == null)
            {
                if (currentFocus == null) return;

                currentFocus = null;
                currentTarget = null;
                _sceneConfig.onFocusObject.Invoke(null);
                _sceneConfig.onTargetObject.Invoke(null);
            }
            else if (interactable.canBeFocused && (currentFocus == null || currentFocus.id != interactable.id))
            {
                currentTarget = interactable;
                currentFocus = interactable;
                _sceneConfig.onFocusObject.Invoke(interactable);
                _sceneConfig.onTargetObject.Invoke(interactable);

                counter = 0;
            }
        }
    }
}