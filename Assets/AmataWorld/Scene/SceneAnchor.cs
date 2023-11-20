using AmataWorld.Logging;
using UnityEngine;

namespace AmataWorld.Scene
{
    public interface SAnchorManager
    {
        SceneAnchor GetAnchor(uint anchorId);
        void Clear();
    }

    public class SceneAnchor : MonoBehaviour
    {
        uint _id;

        Protobuf.SceneDef.Scene.Types.Anchor _data;

        public uint id
        {
            get { return _id; }
        }

        public Protobuf.SceneDef.Scene.Types.Anchor data
        {
            get { return _data; }
        }

        void OnEnable()
        {
            this.LogDebug($"spawned with id = {_id}");
        }

        public void Init(Protobuf.SceneDef.Scene.Types.Anchor anchor)
        {
            _id = anchor.Id;
            _data = anchor;
        }
    }
}