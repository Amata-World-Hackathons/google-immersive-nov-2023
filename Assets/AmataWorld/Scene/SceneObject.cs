using UnityEngine;
using UnityEngine.Events;

namespace AmataWorld.Scene
{
    using SceneTypes = Protobuf.SceneDef.Scene.Types;

    public class SceneObject : MonoBehaviour
    {
        uint _id;

        SceneTypes.Object _source;

        SceneAnchor _anchor;

        public AnchorCouplingType anchorCouplingType;

        public bool canBeFocused = true;

        public Activities.IActivity activity;

        public uint id => _id;
        public SceneAnchor anchor => _anchor;
        public SceneTypes.Object source => _source;

        public bool canBeActioned
        {
            get { return activity != null; }
        }

        public void Init(SceneTypes.Object source, SceneAnchor anchor, Activities.IActivity activity = null)
        {
            _id = source.Id;
            _source = source;
            _anchor = anchor;

            this.activity = activity;

            // if (source.Interaction != null)
            // {
            // }
        }

        public enum AnchorCouplingType
        {
            Tight,
            Loose,
        }
    }
}