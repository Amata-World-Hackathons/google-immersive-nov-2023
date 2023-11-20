using System.Collections.Generic;
using System.Linq;
using AmataWorld.Utils;
using UnityEngine;

namespace AmataWorld.Activities
{
    public class CryptexSegment : MonoBehaviour
    {
        [SerializeField]
        GameObject _cryptexCharPrefab;

        string _alphabet;

        public void Init(string alphabet)
        {
            _alphabet = alphabet;
            var chars = _alphabet.ToCharArray().ToList();
            ListUtils.Shuffle(chars);

            var len = (float)chars.Count();
            foreach (var pair in chars.Select((value, i) => (value, i)))
            {
                var obj = Instantiate(_cryptexCharPrefab, transform);
                var t = obj.transform;
                t.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                t.RotateAround(t.position, t.right, pair.i * 360.0f / len);
                t.position = t.position - 0.1f * t.forward;

                var textComp = obj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                textComp.text = pair.value.ToString();
            }
        }
    }
}