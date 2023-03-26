using GameFramework.Core;
using GameFramework.EntitySystems;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleTankLogic : EntityLogic
    {
        private readonly BattleModel _model;
        private readonly BattleTankModel _tankModel;
        private readonly BattleTankActor _tankActor;
        
        private IBehaviourSelector _behaviourSelector;
        
        private readonly DisposableScope _scope = new DisposableScope();
        private readonly DisposableScope _behaviourScope = new DisposableScope();

        public BattleTankLogic(
            BattleModel model,
            BattleTankModel tankModel,
            BattleTankActor tankActor)
        {
            _model = model;
            _tankModel = tankModel;
            _tankActor = tankActor;
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();
            
            _scope.Dispose();
            _behaviourScope.Dispose();
        }

        protected override void ActivateInternal(IScope scope)
        {
            base.ActivateInternal(scope);
            
            Bind();
            SetEvents();
        }

        private void Bind()
        {
            _tankModel.BattleState
                .TakeUntil(_scope)
                .Subscribe(OnStateChanged)
                .ScopeTo(_scope);

            _tankModel.MoveAmount
                .TakeUntil(_scope)
                .Subscribe(_tankActor.SetMoveAmount)
                .ScopeTo(_scope);

            _tankModel.TurnAmount
                .TakeUntil(_scope)
                .Subscribe(_tankActor.SetTurnAmount)
                .ScopeTo(_scope);

            _tankModel.InvincibleFlag
                .TakeUntil(_scope)
                .Subscribe(_tankActor.SetInvincible)
                .ScopeTo(_scope);
        }

        private void SetEvents()
        {
            _tankActor.OnStateExitAsObservable
                .TakeUntil(_scope)
                .Subscribe(OnAnimatorStateExit)
                .ScopeTo(_scope);

            _tankActor.OnAnimationEventAsObservable
                .TakeUntil(_scope)
                .Subscribe(OnAnimationEvent)
                .ScopeTo(_scope);

            _tankActor.OnDamageReceivedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_tankModel.Damage)
                .ScopeTo(_scope);

            _tankActor.OnPositionChangedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_tankModel.SetPosition)
                .ScopeTo(_scope);

            _tankActor.OnForwardChangedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_tankModel.SetForward)
                .ScopeTo(_scope);
        }

        public void SetBehaviourSelector(IBehaviourSelector behaviourSelector)
        {
            _behaviourSelector = behaviourSelector;
            
            _behaviourScope.Dispose();
            
            _model.CurrentState
                .TakeUntil(_behaviourScope)
                .Subscribe(OnChangedState)
                .ScopeTo(_behaviourScope);

            _model.RuleModel.ResultType
                .TakeUntil(_scope)
                .Subscribe(result =>
                {
                    if (result != BattleResultType.None)
                        _behaviourSelector.SetActive(false);
                })
                .ScopeTo(_scope);
            
            _behaviourSelector.OnDamageAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(_tankModel.Damage)
                .ScopeTo(_behaviourScope);

            _behaviourSelector.OnShotCurveAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(_ => _tankModel.ShotCurve())
                .ScopeTo(_behaviourScope);

            _behaviourSelector.OnShotStraightAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(_ => _tankModel.ShotStraight())
                .ScopeTo(_behaviourScope);

            _behaviourSelector.OnTurnValueChangedAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(_tankModel.SetInputTurnAmount)
                .ScopeTo(_behaviourScope);

            _behaviourSelector.OnMoveValueChangedAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(_tankModel.SetInputMoveAmount)
                .ScopeTo(_behaviourScope);
        }

        private void OnStateChanged(BattleTankState state)
        {
            switch (state)
            {
                case BattleTankState.Ready:
                    _tankActor.Ready();
                    break;
                case BattleTankState.Damage:
                    _behaviourSelector.BeginDamage();
                    _tankActor.Play(BattleTankAnimatorState.Damage);
                    break;
                case BattleTankState.ShotCurve:
                    _tankActor.Play(BattleTankAnimatorState.ShotCurve);
                    break;
                case BattleTankState.ShotStraight:
                    _tankActor.Play(BattleTankAnimatorState.ShotStraight);
                    break;
                case BattleTankState.Dead:
                    _tankActor.Dead();
                    _behaviourSelector.SetActive(false);
                    break;
            }
        }

        private void OnAnimatorStateExit(BattleTankAnimatorState animState)
        {
            Debug.LogFormat("OnAnimatorStateExit: animState={0}", animState);

            switch (animState)
            {
                case BattleTankAnimatorState.Damage:
                    _tankModel.EndDamage();
                    _behaviourSelector.EndDamage();
                    break;
                case BattleTankAnimatorState.ShotCurve:
                    _tankModel.EndShotCurve();
                    break;
                case BattleTankAnimatorState.ShotStraight:
                    _tankModel.EndShotStraight();
                    _behaviourSelector.EndShotStraight();
                    break;
            }
        }

        private void OnAnimationEvent(string id)
        {
            Debug.LogFormat("OnAnimationEvent: id={0}", id);

            switch (id)
            {
                case "ShotCurve":
                    _tankActor.ShotCurve();
                    break;
                case "ShotStraight":
                    _tankActor.ShotStraight();
                    break;
            }
        }

        private void OnChangedState(BattleState state)
        {
            _behaviourSelector.SetActive(state == BattleState.Playing);

            switch (state)
            {
                case BattleState.Ready:
                    _behaviourSelector.Reset();
                    _tankModel.Ready();
                    break;
                case BattleState.Playing:
                    _tankModel.Playing();
                    break;
                case BattleState.Result:
                    _tankModel.Result();
                    break;
            }
        }
    }
}