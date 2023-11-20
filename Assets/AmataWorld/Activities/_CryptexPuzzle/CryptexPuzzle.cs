using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace AmataWorld.Activities
{
    using ActivitySubjectTypes = Protobuf.SceneDef.Scene.Types.Object.Types.ActivitySubject.Types;

    public class CameraController
    {
        public void MoveTo(Vector3 target, Vector3 up, bool animate = true)
        {
        }

        /// <summary>
        /// Removes any camera movement behaviour and reverts to the the default
        /// (copy XR pose)
        /// </summary>
        public void Reset(bool animate = true)
        {
        }
    }

    public record ImmersionConfig
    {
        public readonly CameraMovement cameraMovement;

        public ImmersionConfig(CameraMovement cameraMovement)
        {
            this.cameraMovement = cameraMovement;
        }

        public record CameraMovement
        {
            public static CameraMovement From(Vector3 pos, Vector3 up)
            {
                return new CameraMovement();
            }
        }
    }

    public class CryptexPuzzle : MonoBehaviour, IActivity
    {
        [SerializeField]
        GameObject _segmentPrefab;

        List<CryptexSegment> _segments = new List<CryptexSegment>();

        ActivitySubjectTypes.CryptexPuzzle _source;

        public void Clear()
        {
            _source = null;
            _segments.Clear();
        }

        public void Init(ActivitySubjectTypes.CryptexPuzzle cryptexPuzzle)
        {
            _segments.Clear();

            _source = cryptexPuzzle;

            var segmentChars = _source.Answer.ToCharArray();

            foreach (var pair in segmentChars.Select((value, i) => (value, i)))
            {
                var obj = Instantiate(_segmentPrefab, transform);
                var t = obj.transform;
                t.position += pair.i * 0.05f * t.right;

                var segment = obj.GetComponent<CryptexSegment>();
                segment.Init(_source.Alphabet);

                _segments.Add(segment);
            }
        }

        Coroutine IActivity.StartAsync(UnityAction<bool> onComplete)
        {
            // set UI visibility and hook events
            return null;
        }

        Coroutine IActivity.StopAsync()
        {
            return null;
        }
    }
}