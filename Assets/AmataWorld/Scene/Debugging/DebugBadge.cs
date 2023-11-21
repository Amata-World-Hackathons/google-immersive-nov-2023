using System;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace AmataWorld.Scene.Debugging
{
    public class DebugBadge : MonoBehaviour
    {
        [SerializeField]
        UIDocument _badgeUI;

        [SerializeField]
        UIDocument _overlayUI;

        [SerializeField]
        SceneConfig _sceneConfig;

        Label _debugText;

        StringBuilder _stringBuilder = new StringBuilder();

        bool _expanded = false;

        SceneInteractable _focused;

        void Awake()
        {
            SetExpanded(false);

            _sceneConfig.onFocusObject.AddListener(OnFocusObject);
            OnFocusObject(_sceneConfig.sceneInteractions.currentFocus);

        }

        void OnDestroy()
        {
            _sceneConfig.onFocusObject.RemoveListener(OnFocusObject);
        }

        void OnEnable()
        {
            SetExpanded(_expanded);

            var badgeRoot = _badgeUI.rootVisualElement;

            var overlayRoot = _overlayUI.rootVisualElement;

            _debugText = overlayRoot.Query<Label>(name: "debug-info-text-dump").First();
            _debugText.text = "waiting for information...";

            badgeRoot.Query<Button>(name: "debug-badge-button").First().RegisterCallback<ClickEvent>((ev) =>
            {
                SetExpanded(true);
            });

            overlayRoot.Query<VisualElement>(name: "outside-scroll-view").First().RegisterCallback<ClickEvent>((ev) =>
            {
                SetExpanded(false);
            });
        }

        void OnDisable()
        {
            if (_badgeUI.rootVisualElement != null)
                _badgeUI.rootVisualElement.style.display = DisplayStyle.None;

            if (_overlayUI.rootVisualElement != null)
                _overlayUI.rootVisualElement.style.display = DisplayStyle.None;
        }

        void Update()
        {
            if (_expanded)
            {
                _stringBuilder.Clear();

                _stringBuilder.Append(FormatAsHeader("Interactions"));
                _stringBuilder.Append($"Focused: {FormatObject(_focused)}\n");

                _stringBuilder.Append(SectionSeparator());
                _stringBuilder.Append(FormatAsHeader("VPS"));
                _stringBuilder.Append(_sceneConfig.vpsProvider.ToPrettyDebugString());

                _debugText.text = _stringBuilder.ToString();
            }
        }

        void OnFocusObject(SceneInteractable interactable)
        {
            _focused = interactable;
        }

        string FormatObject(MonoBehaviour component)
        {
            if (component == null) return "<i>N/A</i>";

            var nm = component.gameObject.name;
            var formattedName = nm == "" ? "<i></i>" : nm;
            return $"{formattedName} | <i>{component.GetInstanceID()}</i>";
        }

        string SectionSeparator()
        {
            return "- - - - - - - - -\n";
        }

        string FormatAsHeader(string text)
        {
            return $"<b>{text}</b>\n\n";
        }

        void SetExpanded(bool expanded)
        {
            _badgeUI.rootVisualElement.style.display = expanded ? DisplayStyle.None : DisplayStyle.Flex;
            _overlayUI.rootVisualElement.style.display = expanded ? DisplayStyle.Flex : DisplayStyle.None;

            _expanded = expanded;
        }
    }
}