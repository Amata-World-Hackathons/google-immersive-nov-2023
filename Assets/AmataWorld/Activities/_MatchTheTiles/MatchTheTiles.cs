using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AmataWorld.Algorithms;
using AmataWorld.Logging;
using AmataWorld.Scene;
using AmataWorld.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AmataWorld.Activities
{
    using ActivitySubjectTypes = Protobuf.SceneDef.Scene.Types.Object.Types.ActivitySubject.Types;
    using MatchTheTilesTypes = Protobuf.SceneDef.Scene.Types.Object.Types.ActivitySubject.Types.MatchTheTiles.Types;

    public partial class MatchTheTiles : MonoBehaviour, IActivity, IStateMachineProvider<MatchTheTiles>
    {
        private static readonly List<Color> TILE_COLOR_PALETTE = new List<Color>{
            Color.red,
            Color.blue,
            Color.cyan,
            Color.green,
            Color.magenta,
            Color.yellow,
        };

        SceneConfig _sceneConfig;

        [SerializeField]
        GameObject _tilePrefab;

        [SerializeField]
        GameObject _activeGroup;

        [SerializeField]
        GameObject _idleGroup;

        [SerializeField]
        UIDocument _matchTheTilesUI;

        public UnityEvent<(MatchTheTilesTile tile1, MatchTheTilesTile tile2, bool didMatch)> onMatchResult = new UnityEvent<(MatchTheTilesTile tile1, MatchTheTilesTile tile2, bool didMatch)>();

        UnityAction<bool> _onExit;

        List<MatchTheTilesTile> _tiles = new List<MatchTheTilesTile>();

        MatchTheTilesTile _lastFlippedTile;

        Algorithms.StateMachine<MatchTheTiles> _stateMachine;

        Algorithms.StateMachine<MatchTheTiles> IStateMachineProvider<MatchTheTiles>.stateMachine => _stateMachine;

        public void Init(ActivitySubjectTypes.MatchTheTiles matchTheTiles, SceneConfig sceneConfig)
        {
            _sceneConfig = sceneConfig;
            _sceneConfig.onActionObject.AddListener(OnActionObject);

            _stateMachine = new Algorithms.StateMachine<MatchTheTiles>(this);

            _stateMachine.AddState<MatchTheTiles_IdleState>();
            _stateMachine.AddState<MatchTheTiles_ActiveState>();

            int boardSize = 0;
            switch (matchTheTiles.BoardSize)
            {
                case MatchTheTilesTypes.BoardSize.Two:
                    boardSize = 2;
                    break;

                case MatchTheTilesTypes.BoardSize.Four:
                    boardSize = 4;
                    break;

                case MatchTheTilesTypes.BoardSize.Six:
                    boardSize = 6;
                    break;
            }

            var list = new List<(MatchTheTilesTypes.TileAsset asset, int matchId, Color color)>();
            var numPairs = boardSize * boardSize / 2;
            var colors = ListUtils.SampleUniqueWrapped(TILE_COLOR_PALETTE, numPairs);
            for (var i = 0; i < numPairs; i++)
            {
                var (item, index) = ListUtils.Sample(matchTheTiles.Assets);
                var color = colors[i];

                list.Add((item, i, color));
                list.Add((item, i, color));
            }

            ListUtils.Shuffle(list);
            _tiles.Clear();

            var physicalBoardSize = matchTheTiles.PhysicalBoardLengthMm / 1000.0f;
            var physicalTileSize = physicalBoardSize / boardSize;
            foreach (var (pair, i) in list.Select((value, i) => (value, i)))
            {
                var x = i % boardSize;
                var y = i / boardSize;

                var dx = (x + 0.5f - boardSize / 2) * physicalTileSize;
                var dy = (-y - 0.5f + boardSize / 2) * physicalTileSize;

                var obj = Instantiate(_tilePrefab, _activeGroup.transform);
                obj.transform.SetLocalPositionAndRotation(new Vector3(dx, dy, 0), Quaternion.identity);
                obj.transform.localScale = new Vector3(physicalTileSize, physicalTileSize, physicalTileSize);

                var tile = obj.GetComponent<MatchTheTilesTile>();
                tile.Init(this, pair.color, pair.matchId);
                _tiles.Add(tile);
            }
        }

        Coroutine IActivity.StartAsync(UnityAction<bool> onExit)
        {
            _onExit = onExit;

            return _stateMachine.TransitionToAsync<MatchTheTiles_ActiveState>();
        }

        Coroutine IActivity.StopAsync()
        {
            return StartCoroutine(StopAsyncImpl(false));
        }

        IEnumerator StopAsyncImpl(bool didComplete)
        {
            yield return _stateMachine.TransitionToAsync<MatchTheTiles_IdleState>();

            if (_onExit != null) _onExit(didComplete);
            _onExit = null;
        }

        void OnDestroy()
        {
            if (_sceneConfig != null) _sceneConfig.onActionObject.RemoveListener(OnActionObject);
        }

        public void OnTileFlipped(MatchTheTilesTile tile)
        {
            if (_lastFlippedTile != null)
            {
                var didMatch = _lastFlippedTile.matchId == tile.matchId;
                if (didMatch)
                {
                    _lastFlippedTile.SetMatched();
                    tile.SetMatched();
                }
                else
                {
                    _lastFlippedTile.Unflip();
                    tile.Unflip();
                }

                onMatchResult.Invoke((tile1: _lastFlippedTile, tile2: tile, didMatch));
                _lastFlippedTile = null;

                var isComplete = !_tiles.Any((t) => !t.wasMatched);

                if (isComplete) StartCoroutine(StopAsyncImpl(true));
            }
            else
            {
                _lastFlippedTile = tile;
            }
        }

        void OnActionObject(SceneInteractable interactable)
        {
            switch (interactable.target)
            {
                case MatchTheTilesTile tile:
                    break;

                default:
                    // do nothing
                    break;
            }
        }
    }
}