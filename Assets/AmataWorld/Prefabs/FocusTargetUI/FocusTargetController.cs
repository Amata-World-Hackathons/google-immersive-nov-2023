using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmataWorld.Prefabs
{
    public class FocusTargetController : MonoBehaviour
    {
        [SerializeField]
        TMPro.TextMeshProUGUI _text;

        public void SetText(string txt)
        {
            if (txt == "")
            {
                _text.text = txt;
                _text.gameObject.SetActive(false);
            }
            else
            {
                _text.text = txt;
                _text.gameObject.SetActive(true);
            }
        }
    }
}