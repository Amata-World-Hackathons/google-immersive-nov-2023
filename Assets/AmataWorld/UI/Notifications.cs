using System.Collections;
using AmataWorld.Scene;
using UnityEngine;
using UnityEngine.UIElements;

namespace AmataWorld.UI
{
    public class Notifications : MonoBehaviour
    {
        [SerializeField]
        SceneConfig _sceneConfig;

        [SerializeField]
        [Range(3.0f, 20.0f)]
        float _duration = 5.0f;

        UIDocument _doc;

        string _text;

        Label GetLabel() => _doc.rootVisualElement.Query<Label>(name: "notification-text").First();

        void Awake()
        {
            _sceneConfig.onNotification.AddListener(OnNotification);
            _doc = GetComponent<UIDocument>();

            GetLabel().SetEnabled(false);
        }

        void OnDestroy()
        {
            _sceneConfig.onNotification.RemoveListener(OnNotification);
        }

        void RefreshText()
        {
            var label = GetLabel();
            label.text = _text;
            label.SetEnabled(true);

            StartCoroutine(DelayedClearText());
        }

        IEnumerator DelayedClearText()
        {
            var textToClear = _text;

            yield return new WaitForSeconds(_duration);

            if (textToClear == _text)
            {
                GetLabel().SetEnabled(false);
                _text = "";
            }
        }

        void OnNotification(string text)
        {
            _text = text;

            RefreshText();
        }
    }
}