using DG.Tweening;
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

        private static int TARGET_MATCH_ID = 1;

        Protobuf.SceneDef.Scene.Types.Object.Types.ActivitySubject _data;

        public void Init(Protobuf.SceneDef.Scene.Types.Object.Types.ActivitySubject activitySubject, SceneConfig sceneConfig)
        {
            var matchTheTiles = activitySubject.Type.MatchTheTiles;
            _data = activitySubject;
            _sceneConfig = sceneConfig;

            _stateMachine = new Algorithms.StateMachine<MatchTheTiles>(this);

            _stateMachine.AddState<MatchTheTiles_IdleState>();
            _stateMachine.AddState<MatchTheTiles_ActiveState>();

            int boardSize = (int)matchTheTiles.BoardSize;

            var list = new List<(MatchTheTilesTypes.TileAsset asset, int matchId, Color color)>();
            var numPairs = boardSize * boardSize / 2;
            var colors = ListUtils.SampleUniqueWrapped(TILE_COLOR_PALETTE, numPairs);
            for (var i = 0; i < numPairs; i++)
            {
                this.LogDebug($"ABC = {boardSize} {numPairs} {i} {colors.Count}");
                var (item, index) = ListUtils.Sample(matchTheTiles.Assets);
                var color = colors[i];

                list.Add((item, i, color));
                list.Add((item, i, color));
            }
            if (boardSize % 2 == 1)
            {
                var (item, index) = ListUtils.Sample(matchTheTiles.Assets);

                list.Add((item, -1, Color.white));
            }

            ListUtils.Shuffle(list);
            _tiles.Clear();

            var physicalBoardSize = matchTheTiles.PhysicalBoardLengthMm / 1000.0f;
            var physicalTileSize = physicalBoardSize / boardSize;
            var centerOffset = (boardSize % 2 == 1) ? 0f : 0.5f;
            foreach (var (pair, i) in list.Select((value, i) => (value, i)))
            {
                var x = i % boardSize;
                var y = i / boardSize;

                var dx = (x + centerOffset - boardSize / 2) * physicalTileSize;
                var dy = (-y - centerOffset + boardSize / 2) * physicalTileSize;

                var obj = Instantiate(_tilePrefab, _activeGroup.transform);
                obj.transform.SetLocalPositionAndRotation(new Vector3(dx, dy, 0), Quaternion.identity);
                obj.transform.localScale = new Vector3(physicalTileSize, physicalTileSize, physicalTileSize);

                var tile = obj.GetComponent<MatchTheTilesTile>();
                var prizeAsset = pair.matchId == TARGET_MATCH_ID ? "burger" : "";
                tile.Init(this, pair.color, pair.matchId, prizeAsset);
                _tiles.Add(tile);
            }
        }

        Coroutine IActivity.StartAsync(UnityAction<bool> onExit)
        {
            _onExit = onExit;

            _sceneConfig.onNotification.Invoke("Find hidden item to unlock the next challenge");

            return _stateMachine.TransitionToAsync<MatchTheTiles_ActiveState>();
        }

        Coroutine IActivity.StopAsync()
        {
            return StartCoroutine(StopAsyncImpl(false));
        }

        IEnumerator StopAsyncImpl(bool didComplete)
        {
            var originalScale = transform.localScale;
            var tweener = transform.DOScale(Vector3.zero, 0.5f);

            yield return tweener.WaitForCompletion();

            yield return _stateMachine.TransitionToAsync<MatchTheTiles_IdleState>();
            transform.localScale = originalScale;

            foreach (var tile in _tiles)
            {
                tile.Unflip();
                _lastFlippedTile = null;
            }

            if (didComplete && _data.TriggersEventId != 0) _sceneConfig.onSceneEventTriggerIntent.Invoke(_data.TriggersEventId);

            if (_onExit != null) _onExit(didComplete);
            _onExit = null;
        }

        public void OnTileFlipped(MatchTheTilesTile tile)
        {
            if (tile.matchId != TARGET_MATCH_ID)
                tile.Unflip();
            else if (_lastFlippedTile == null)
                _lastFlippedTile = tile;
            else
            {
                onMatchResult.Invoke((tile1: _lastFlippedTile, tile2: tile, true));
                _lastFlippedTile = null;

                StartCoroutine(StopAsyncImpl(true));
            }
        }
    }
}