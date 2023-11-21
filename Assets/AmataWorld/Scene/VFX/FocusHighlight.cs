using UnityEngine;

namespace AmataWorld.Scene.VFX
{
    public class FocusHighlight : MonoBehaviour
    {
        [SerializeField]
        SceneConfig _sceneConfig;

        SceneInteractable _lastFocused;

        void Awake()
        {
            _sceneConfig.onFocusObject.AddListener(OnFocusObject);
        }

        void OnFocusObject(SceneInteractable interactable)
        {
            if (_lastFocused != interactable && _lastFocused != null)
            {
                var meshes = _lastFocused.gameObject.GetComponentsInChildren<MeshRenderer>();
                foreach (var mesh in meshes)
                {
                    var outline = mesh.gameObject.GetComponent<QuickOutline.Outline>();

                    if (outline != null) outline.enabled = false;
                }
            }

            if (interactable != null)
            {
                var meshes = interactable.gameObject.GetComponentsInChildren<MeshRenderer>();

                foreach (var mesh in meshes)
                {
                    var outline = mesh.gameObject.GetComponent<QuickOutline.Outline>();

                    if (outline == null)
                    {
                        outline = mesh.gameObject.AddComponent<QuickOutline.Outline>();

                        outline.OutlineColor = Color.cyan;
                        outline.OutlineWidth = 10;
                        outline.OutlineMode = QuickOutline.Outline.Mode.OutlineVisible;
                    }

                    outline.enabled = true;
                }
            }

            _lastFocused = interactable;
        }
    }
}