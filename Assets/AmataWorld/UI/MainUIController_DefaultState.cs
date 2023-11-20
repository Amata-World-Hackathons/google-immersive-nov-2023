using UnityEngine;
using UnityEngine.UIElements;

using AmataWorld.Scene;
using AmataWorld.Logging;
using AmataWorld.Algorithms;
using AmataWorld.Activities;

namespace AmataWorld.UI
{
    partial class MainUIController
    {
        public class MainUIController_DefaultState : StateBehaviour<MainUIController>
        {
            SceneInteractable _actionTarget;

            Button _actionButton;

            protected override void Awake()
            {
                base.Awake();

                source._sceneConfig.onFocusObject.AddListener(OnFocusObject);

                var root = source._mainUI.rootVisualElement;
                _actionButton = root.Query<Button>(name: "main-button").First();
                _actionButton.SetEnabled(false);

                _actionButton.RegisterCallback<ClickEvent>((ev) =>
                {
                    this.LogDebug("Button clicked");

                    if (_actionTarget != null)
                        source._sceneConfig.onActionObject.Invoke(_actionTarget);
                });
            }

            void OnDestroy()
            {
                source._sceneConfig.onFocusObject.RemoveListener(OnFocusObject);
            }

            void OnEnable()
            {
                source._mainUI.rootVisualElement.style.visibility = Visibility.Visible;
                source._sceneConfig.onActivityStarted.AddListener(OnActivityStarted);
            }

            void OnDisable()
            {
                source._sceneConfig.onActivityStarted.RemoveListener(OnActivityStarted);

                if (source._mainUI.rootVisualElement != null)
                    source._mainUI.rootVisualElement.style.visibility = Visibility.Hidden;
            }

            void OnActivityStarted(IActivity activity)
            {
                TransitionToAsync<MainUIController_ActivityOverlayState>();
            }

            void OnFocusObject(Scene.SceneInteractable sceneInteractable)
            {
                _actionTarget = sceneInteractable;
                _actionButton.SetEnabled(sceneInteractable != null);
            }
        }
    }
}