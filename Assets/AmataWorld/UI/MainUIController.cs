using AmataWorld.Algorithms;
using UnityEngine;
using UnityEngine.UIElements;

namespace AmataWorld.UI
{
    public partial class MainUIController : MonoBehaviour, IStateMachineProvider<MainUIController>
    {
        [SerializeField] Scene.SceneConfig _sceneConfig;

        [SerializeField]
        UIDocument _mainUI;

        [SerializeField]
        UIDocument _rewardsUI;

        [SerializeField]
        UIDocument _activityOverlayUI;

        Algorithms.StateMachine<MainUIController> _stateMachine;

        StateMachine<MainUIController> IStateMachineProvider<MainUIController>.stateMachine => _stateMachine;

        void Awake()
        {
            _stateMachine = new StateMachine<MainUIController>(this);

            _stateMachine.AddState<MainUIController_DefaultState>();
            _stateMachine.AddState<MainUIController_ActivityOverlayState>();

            _sceneConfig.onRewardEarned.AddListener(OnReward);
        }

        void OnEnable()
        {
            var container = _rewardsUI.rootVisualElement.Query<VisualElement>(name: "container").First();
            container.SetEnabled(false);
        }

        void OnDestroy()
        {
            _sceneConfig.onRewardEarned.RemoveListener(OnReward);
        }

        void OnReward()
        {
            var container = _rewardsUI.rootVisualElement.Query<VisualElement>(name: "container").First();
            container.SetEnabled(true);

            container.RegisterCallback<ClickEvent>((ev) =>
            {
                container.SetEnabled(false);
            });
        }
    }
}