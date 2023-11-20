using System.Collections.Generic;
using AmataWorld.Logging;
using AmataWorld.Scene;
using UnityEngine;

namespace AmataWorld.Features.Synthesis
{
    using SceneTypes = Protobuf.SceneDef.Scene.Types;

    public class DeviceRelativeSpawnCoordinator : MonoBehaviour
    {
        [SerializeField]
        SceneConfig _sceneConfig;

        Dictionary<uint, SceneAnchor> _anchorsDict = new Dictionary<uint, SceneAnchor>();

        public SceneAnchor AddAnchor(GameObject gameObject, SceneTypes.Anchor anchor)
        {
            if (anchor.Type.TypeCase != SceneTypes.AnchorType.TypeOneofCase.DeviceRelative)
            {
                this.LogError("tried to add an incompatible anchor");
                throw new System.Exception("tried to add an incompatible anchor into RelativeDeviceSpawn");
            }

            var data = anchor.Type.DeviceRelative;

            gameObject.transform.SetParent(transform);
            gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            gameObject.SetActive(false);

            var sceneAnchor = gameObject.AddComponent<SceneAnchor>();
            sceneAnchor.Init(anchor);

            _anchorsDict.Add(anchor.Id, sceneAnchor);

            if (anchor.Active)
            {
                switch (data.Alignment)
                {
                    case SceneTypes.AnchorType.Types.DeviceRelative.Types.Alignment.CameraForward:
                        {
                            var t = _sceneConfig.poseDriver.transform;
                            var p = t.position + data.Transform.OffsetZ * t.forward + data.Transform.OffsetY * t.up + data.Transform.OffsetX * t.right;

                            // TODO fix rotation
                            gameObject.transform.SetPositionAndRotation(p, Quaternion.identity);
                            break;
                        }

                    default:
                        this.LogError($"unhandled alignment = {data.Alignment}");
                        throw new System.Exception($"unhandled DeviceRelative alignment = {data.Alignment}");
                }

                gameObject.SetActive(true);
            }

            return sceneAnchor;
        }
    }
}