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
        UIDocument _activityOverlayUI;

        Algorithms.StateMachine<MainUIController> _stateMachine;

        StateMachine<MainUIController> IStateMachineProvider<MainUIController>.stateMachine => _stateMachine;

        void Awake()
        {
            _stateMachine = new StateMachine<MainUIController>(this);

            _stateMachine.AddState<MainUIController_DefaultState>();
            _stateMachine.AddState<MainUIController_ActivityOverlayState>();
        }
    }
}