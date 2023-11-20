using AmataWorld.Algorithms;
using AmataWorld.Activities;
using UnityEngine.UIElements;
using AmataWorld.Logging;

namespace AmataWorld.UI
{
    partial class MainUIController
    {
        public class MainUIController_ActivityOverlayState : StateBehaviour<MainUIController>
        {
            Button _closeActivityButton;

            IActivity _activity;

            protected override void Awake()
            {
                base.Awake();

                source._sceneConfig.onActivityStarted.AddListener(OnActivityStarted);
                source._sceneConfig.onActivityEnded.AddListener(OnActivityEnded);
            }

            void OnDestroy()
            {
                source._sceneConfig.onActivityStarted.RemoveListener(OnActivityStarted);
                source._sceneConfig.onActivityEnded.RemoveListener(OnActivityEnded);
            }

            void OnEnable()
            {
                source._activityOverlayUI.rootVisualElement.style.visibility = Visibility.Visible;

                var root = source._activityOverlayUI.rootVisualElement;
                _closeActivityButton = root.Query<Button>(name: "close-activity-button").First();

                _closeActivityButton.RegisterCallback<ClickEvent>((ev) =>
                {
                    this.LogDebug($"stop button clicked {_activity != null}");
                    if (_activity != null) _activity.StopAsync();
                });
            }

            void OnDisable()
            {
                if (source._activityOverlayUI.rootVisualElement != null)
                    source._activityOverlayUI.rootVisualElement.style.visibility = Visibility.Hidden;
            }

            void OnActivityStarted(IActivity activity)
            {
                _activity = activity;
            }

            void OnActivityEnded(IActivity activity, bool didComplete)
            {
                _activity = null;

                if (enabled) TransitionToAsync<MainUIController_DefaultState>();
            }
        }
    }
}