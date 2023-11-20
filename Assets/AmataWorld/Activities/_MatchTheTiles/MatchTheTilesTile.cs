using System.Collections;
using AmataWorld.Logging;
using AmataWorld.Scene;
using UnityEngine;

namespace AmataWorld.Activities
{
    public class MatchTheTilesTile : MonoBehaviour
    {
        private static Color UNFLIPPED_COLOR = Color.grey;

        MatchTheTiles _matchTheTiles;
        public Color color { get; private set; }

        public int matchId { get; private set; }

        public bool wasMatched { get; private set; } = false;

        public bool isFlipped { get; private set; } = false;

        [SerializeField]
        MeshRenderer _tileMesh;

        Material _tileMaterial;

        void Awake()
        {
            var interactable = gameObject.AddComponent<SceneInteractable>();
            interactable.target = this;
        }

        public void Init(MatchTheTiles matchTheTiles, Color color, int matchId)
        {
            _matchTheTiles = matchTheTiles;
            this.color = color;
            this.matchId = matchId;

            _tileMaterial = _tileMesh.materials[0];
            _tileMaterial.SetColor("_BaseColor", UNFLIPPED_COLOR);
        }

        public void SetMatched()
        {
            wasMatched = true;
        }

        public void Flip()
        {
            if (wasMatched)
            {
                this.LogWarning("attempted to flip a matched tile");
                return;
            }

            if (isFlipped)
            {
                this.LogWarning("attempted to flip a flipped tile");
                return;
            };

            isFlipped = true;

            _tileMaterial.SetColor("_BaseColor", color);

            StartCoroutine(WaitForFlipAsync());
        }

        IEnumerator WaitForFlipAsync()
        {
            yield return new WaitForSeconds(1);
            _matchTheTiles.OnTileFlipped(this);
        }

        public void Unflip()
        {
            if (wasMatched)
            {
                this.LogWarning("attempted to flip a matched tile");
                return;
            }

            if (!isFlipped)
            {
                this.LogWarning("attempted to unflip an unflipped tile");
                return;
            };

            isFlipped = false;

            _tileMaterial.SetColor("_BaseColor", UNFLIPPED_COLOR);
        }
    }
}