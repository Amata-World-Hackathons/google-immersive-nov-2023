using UnityEngine;
using UnityEngine.UI;

namespace AmataWorld.Scene.UI
{
    public class FocusTarget : MonoBehaviour
    {
        [SerializeField]
        SceneConfig _sceneConfig;

        [SerializeField]
        GameObject _focusTargetPrefab;

        void Awake()
        {
            var graphic = _focusTargetPrefab.GetComponentInChildren<Graphic>();
            var material = graphic.materialForRendering;

            // see https://discussions.unity.com/t/world-space-canvas-on-top-of-everything/128165/6
            // disable the z-test, always draw the UI
            material.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always);
            graphic.material = material;
        }

        void OnEnable()
        {
            _sceneConfig.onFocusObject.AddListener(onFocusObject);
        }

        void OnDisable()
        {
            _sceneConfig.onFocusObject.RemoveListener(onFocusObject);
        }

        void onFocusObject(SceneInteractable interactable)
        {
            if (interactable != null)
            {
                var instanceId = interactable.id;
                _focusTargetPrefab.transform.SetParent(interactable.transform);
                _focusTargetPrefab.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _focusTargetPrefab.SetActive(true);

                var controller = _focusTargetPrefab.GetComponentInChildren<Prefabs.FocusTargetController>();

                controller.SetText($"Object {instanceId}");
            }
            else
            {
                _focusTargetPrefab.SetActive(false);
                _focusTargetPrefab.transform.SetParent(null, true);
            }
        }
    }
}