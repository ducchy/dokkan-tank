using System;
using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class TankBehaviourController : IDisposable
    {
        private readonly BattleModel _model;
        private readonly BattleTankModel _tankModel;
        private readonly BattleTankActor _tankActor;
        private readonly BattleTankControlUiView _controlUiView;
        private readonly ITankBehaviour _npcTankBehaviour;

        private ITankBehaviour _currentTankBehaviour;

        private readonly DisposableScope _scope = new();
        private readonly DisposableScope _behaviourScope = new();

        public TankBehaviourController(
            BattleModel model,
            BattleTankModel tankModel,
            BattleTankActor tankActor,
            BattleTankControlUiView controlUiView,
            ITankBehaviour npcTankBehaviour)
        {
            _model = model;
            _tankModel = tankModel;
            _tankActor = tankActor;
            _controlUiView = controlUiView;
            _npcTankBehaviour = npcTankBehaviour;

            SetEvents();

            SetBehaviourSelector(tankModel.CharacterType);
        }

        public void Dispose()
        {
            _scope.Dispose();
            _behaviourScope.Dispose();
        }

        public void Update()
        {
            _currentTankBehaviour?.Update();
        }

        private void SetEvents()
        {
            if (_controlUiView == null)
                return;
            
            _controlUiView.OnAutoToggleValueChangedAsObservable
                .TakeUntil(_scope)
                .Subscribe(autoFlag =>
                    SetBehaviourSelector(autoFlag ? CharacterType.NonPlayer : CharacterType.Player))
                .ScopeTo(_scope);
        }

        private void SetBehaviourSelector(CharacterType characterType)
        {
            if (characterType == CharacterType.Player && _controlUiView != null)
            {
                SetBehaviourSelector(_controlUiView);
                return;
            }

            SetBehaviourSelector(_npcTankBehaviour);
        }

        private void SetBehaviourSelector(ITankBehaviour tankBehaviour)
        {
            _currentTankBehaviour = tankBehaviour;

            _behaviourScope.Dispose();

            _model.CurrentState
                .TakeUntil(_behaviourScope)
                .Subscribe(OnChangedBattleState)
                .ScopeTo(_behaviourScope);

            _model.RuleModel.ResultType
                .TakeUntil(_scope)
                .Subscribe(result =>
                {
                    if (result != BattleResultType.None)
                        _currentTankBehaviour.SetActive(false);
                })
                .ScopeTo(_scope);

            _tankModel.CurrentState
                .TakeUntil(_behaviourScope)
                .Subscribe(OnChangedBattleTankState)
                .ScopeTo(_behaviourScope);

            _tankActor.OnStateExitAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(OnAnimatorStateExit)
                .ScopeTo(_behaviourScope);

            tankBehaviour.OnShotCurveAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(_ => _tankModel.SetState(BattleTankState.ShotCurve))
                .ScopeTo(_behaviourScope);

            tankBehaviour.OnShotStraightAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(_ => _tankModel.SetState(BattleTankState.ShotStraight))
                .ScopeTo(_behaviourScope);

            tankBehaviour.OnTurnValueChangedAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(_tankModel.SetInputTurnAmount)
                .ScopeTo(_behaviourScope);

            tankBehaviour.OnMoveValueChangedAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(_tankModel.SetInputMoveAmount)
                .ScopeTo(_behaviourScope);
        }

        private void OnAnimatorStateExit(BattleTankAnimatorState animState)
        {
            switch (animState)
            {
                case BattleTankAnimatorState.Damage:
                    _currentTankBehaviour.EndDamage();
                    break;
                case BattleTankAnimatorState.ShotStraight:
                    _currentTankBehaviour.EndShotStraight();
                    break;
            }
        }

        private void OnChangedBattleState(BattleState state)
        {
            _currentTankBehaviour.SetActive(state == BattleState.Playing);

            switch (state)
            {
                case BattleState.Ready:
                    _currentTankBehaviour.Reset();
                    break;
            }
        }

        private void OnChangedBattleTankState(BattleTankState state)
        {
            switch (state)
            {
                case BattleTankState.Damage:
                    _currentTankBehaviour.BeginDamage();
                    break;
                case BattleTankState.Dead:
                    _currentTankBehaviour.SetActive(false);
                    break;
            }
        }
    }
}