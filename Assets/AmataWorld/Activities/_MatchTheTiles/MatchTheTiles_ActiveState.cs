using AmataWorld.Algorithms;
using AmataWorld.Logging;
using AmataWorld.Scene;
using UnityEngine.UIElements;

namespace AmataWorld.Activities
{
    partial class MatchTheTiles
    {
        public class MatchTheTiles_ActiveState : StateBehaviour<MatchTheTiles>
        {
            Button _actionButton;

            MatchTheTilesTile _target;

            void OnEnable()
            {
                // cannot have more than one UI document active at one time?
                var root = source._matchTheTilesUI.rootVisualElement;
                root.style.display = DisplayStyle.Flex;
                source._activeGroup.SetActive(true);

                _actionButton = root.Query<Button>(name: "action-button").First();

                _actionButton.RegisterCallback<ClickEvent>((ev) =>
                {
                    if (_target == null) return;

                    if (!_target.isFlipped) _target.Flip();
                });

                source._sceneConfig.onFocusObject.AddListener(onFocusObject);
            }

            void OnDisable()
            {
                source._sceneConfig.onFocusObject.RemoveListener(onFocusObject);
                source._activeGroup.SetActive(false);
                _actionButton = null;

                if (source._matchTheTilesUI.rootVisualElement != null)
                    source._matchTheTilesUI.rootVisualElement.style.display = DisplayStyle.None;
            }

            void onFocusObject(SceneInteractable interactable)
            {
                if (interactable != null && interactable.target is MatchTheTilesTile tile && !tile.isFlipped && !tile.wasMatched)
                {
                    _actionButton.style.visibility = Visibility.Visible;
                    _target = tile;
                }
                else
                {
                    _actionButton.style.visibility = Visibility.Hidden;
                    _target = null;
                }
            }
        }
    }
}