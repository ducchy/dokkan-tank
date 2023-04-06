using System;
using DG.Tweening;
using UnityEngine;

namespace dtank
{
    public class BattleUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private BattleTankControlUiView _tankControlUiView;
        [SerializeField] private BattlePlayerStatusUiView _playerStatusUiView;
        [SerializeField] private BattleReadyUiView _readyUiView;
        [SerializeField] private BattlePlayingUiView _playingUiView;
        [SerializeField] private BattleResultUiView _resultUiView;

        public BattleTankControlUiView TankControlUiView => _tankControlUiView;
        public BattlePlayerStatusUiView PlayerStatusUiView => _playerStatusUiView;
        public BattleReadyUiView ReadyUiView => _readyUiView;
        public BattlePlayingUiView PlayingUiView => _playingUiView;
        public BattleResultUiView ResultUiView => _resultUiView;
        private Sequence _sequence;

        public void Setup()
        {
            _tankControlUiView.Setup();
        }

        public void Dispose()
        {
            _tankControlUiView.Dispose();
            _playerStatusUiView.Dispose();
            _readyUiView.Dispose();
            _playingUiView.Dispose();
            _resultUiView.Dispose();

            _sequence.Kill();
        }

        public void Reset()
        {
            _tankControlUiView.Reset();
            _playerStatusUiView.Reset();
            _readyUiView.Reset();
            _playingUiView.Reset();
            _resultUiView.Reset();
        }
    }
}