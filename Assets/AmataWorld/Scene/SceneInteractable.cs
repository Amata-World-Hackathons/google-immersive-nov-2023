using UnityEngine;

namespace AmataWorld.Scene
{
    /// <summary>
    /// Add SceneInteractable to mark an object as interactable on the scene. An
    /// interactable object will emit scene events on interaction.
    /// </summary>
    public class SceneInteractable : MonoBehaviour
    {
        private static LayerMask _layerMask = 0;

        public object target;

        public bool canBeFocused = true;
        public bool canBeActioned = true;

        public int id => GetInstanceID();

        void Awake()
        {
            if (_layerMask == 0)
                _layerMask = SceneConfig.GetSceneInteractableLayer();

            gameObject.layer = _layerMask;
        }
    }
}