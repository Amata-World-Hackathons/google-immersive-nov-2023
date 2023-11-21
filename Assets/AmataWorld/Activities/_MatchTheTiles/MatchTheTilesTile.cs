using System.Collections;
using AmataWorld.Logging;
using AmataWorld.Scene;
using DG.Tweening;
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

        [SerializeField]
        GameObject _prizeRoot;

        [SerializeField]
        GameObject _burgerModel;

        [SerializeField]
        GameObject _sushiModel;

        Material _tileMaterial;

        Tweener _prizeTweener;

        void Awake()
        {
            var interactable = gameObject.AddComponent<SceneInteractable>();
            interactable.target = this;

            _prizeTweener = _prizeRoot.transform.DORotate(new Vector3(0f, 360f, 0f), 5f);
            _prizeTweener.Pause();
        }

        void OnEnable()
        {
            _prizeTweener.Play();
        }

        void OnDisable()
        {
            _prizeTweener.Pause();
        }

        public void Init(MatchTheTiles matchTheTiles, Color color, int matchId, string assetName)
        {
            _matchTheTiles = matchTheTiles;
            this.color = color;
            this.matchId = matchId;

            _tileMaterial = _tileMesh.materials[0];
            _tileMaterial.SetColor("_BaseColor", UNFLIPPED_COLOR);
            _tileMaterial.SetFloat("_Dissolve", 0.0f);

            _burgerModel.SetActive(false);
            _sushiModel.SetActive(false);

            switch (assetName)
            {
                case "burger":
                    _burgerModel.SetActive(true);
                    break;

                case "sushi":
                    _sushiModel.SetActive(true);
                    break;
            }
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

            StartCoroutine(WaitForFlipAsync());
        }

        IEnumerator WaitForFlipAsync()
        {
            var elapsed = 0f;
            var maxTime = 1f;
            while (elapsed < maxTime)
            {
                var dissolve = Mathf.Clamp(elapsed / maxTime, 0f, 1f);
                _tileMaterial.SetFloat("_Dissolve", dissolve);

                var currentColor = Color.Lerp(UNFLIPPED_COLOR, color, Mathf.Clamp(1.5f * dissolve, 0f, 1f));
                _tileMaterial.SetColor("_BaseColor", currentColor);

                elapsed += Time.deltaTime;
                yield return null;
            }

            var tweener = _prizeRoot.transform.DOLocalJump(Vector3.zero, 0.2f, 1, 1f);
            tweener.Pause();
            tweener.SetLoops(1);
            tweener.Play();

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

            StartCoroutine(ExecUnflipAsync());
        }

        IEnumerator ExecUnflipAsync()
        {
            var elapsed = 0f;
            var maxTime = 0.5f;
            while (elapsed < maxTime)
            {
                var dissolve = 1f - Mathf.Clamp(elapsed / maxTime, 0f, 1f);
                _tileMaterial.SetFloat("_Dissolve", dissolve);

                var currentColor = Color.Lerp(UNFLIPPED_COLOR, color, Mathf.Clamp(1.5f * dissolve, 0f, 1f));
                _tileMaterial.SetColor("_BaseColor", currentColor);

                elapsed += Time.deltaTime;
                yield return null;
            }

            _tileMaterial.SetColor("_BaseColor", UNFLIPPED_COLOR);
        }
    }
}