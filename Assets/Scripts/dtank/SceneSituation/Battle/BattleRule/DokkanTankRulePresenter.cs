using System;
using System.Linq;
using UniRx;

namespace dtank
{
    public class DokkanTankRulePresenter : BattleRulePresenterBase
    {
        private readonly BattleResultData _resultData;
        private readonly BattleTankModel _mainPlayer;
        private readonly BattleTankModel[] _players;

        public Action OnGameEnd;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public DokkanTankRulePresenter(
            BattleResultData resultData,
            BattleTankModel mainPlayer,
            BattleTankModel[] players)
        {
            _resultData = resultData;
            _mainPlayer = mainPlayer;
            _players = players;

            Bind();
        }

        public override void Dispose()
        {
            _disposable.Dispose();
        }

        private void Bind()
        {
            foreach (var player in _players)
                player.Hp.Subscribe(_ => CheckResult()).AddTo(_disposable);
        }

        private void CheckResult()
        {
            if (_resultData.ResultType != BattleResultType.None)
                return;

            if (_mainPlayer.DeadFlag)
            {
                _resultData.SetResultType(BattleResultType.Lose);
                OnGameEnd?.Invoke();
                return;
            }

            if (_players.Count(p => p != _mainPlayer && !p.DeadFlag) == 0) {
                _resultData.SetResultType(BattleResultType.Win);
                OnGameEnd?.Invoke();
            }
        }

        public void OnChangedState(BattleState prev, BattleState current)
        {
            if (current == BattleState.Ready)
                _resultData.SetResultType(BattleResultType.None);
        }
    }
}