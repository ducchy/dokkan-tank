using System;
using GameFramework.Core;
using UniRx;
using UnityEngine;

namespace dtank
{
    public abstract class BattleTankPresenterBase : IDisposable
    {
        protected readonly BattleTankController _controller;
        protected readonly BattleModel _model;
        protected readonly BattleTankModel _tankModel;
        protected readonly BattleTankActor _actor;
        protected readonly IBehaviourSelector _behaviourSelector;
        protected readonly DisposableScope _scope = new DisposableScope();

        protected BattleTankPresenterBase(
            BattleTankController controller,
            BattleModel model,
            BattleTankModel tankModel,
            BattleTankActor actor,
            IBehaviourSelector behaviourSelector)
        {
            _controller = controller;
            _model = model;
            _tankModel = tankModel;
            _actor = actor;
            _behaviourSelector = behaviourSelector;
        }

        public virtual void Dispose()
        {
            _scope.Dispose();
        }

        protected void Bind()
        {
            _model.CurrentState
                .TakeUntil(_scope)
                .Subscribe(OnChangedState)
                .ScopeTo(_scope);

            _model.RuleModel.ResultType
                .TakeUntil(_scope)
                .Subscribe(result =>
                {
                    if (result != BattleResultType.None)
                        _behaviourSelector.SetActive(false);
                })
                .ScopeTo(_scope);

            _tankModel.BattleState
                .TakeUntil(_scope)
                .Subscribe(OnStateChanged)
                .ScopeTo(_scope);

            _tankModel.MoveAmount
                .TakeUntil(_scope)
                .Subscribe(_actor.SetMoveAmount)
                .ScopeTo(_scope);

            _tankModel.TurnAmount
                .TakeUntil(_scope)
                .Subscribe(_actor.SetTurnAmount)
                .ScopeTo(_scope);

            _tankModel.InvincibleFlag
                .TakeUntil(_scope)
                .Subscribe(_actor.SetInvincible)
                .ScopeTo(_scope);

            BindInternal();
        }

        protected virtual void BindInternal()
        {
        }

        protected void SetEvents()
        {
            _behaviourSelector.OnDamageAsObservable
                .TakeUntil(_scope)
                .Subscribe(_tankModel.Damage)
                .ScopeTo(_scope);

            _behaviourSelector.OnShotCurveAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _tankModel.ShotCurve())
                .ScopeTo(_scope);

            _behaviourSelector.OnShotStraightAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _tankModel.ShotStraight())
                .ScopeTo(_scope);

            _behaviourSelector.OnTurnValueChangedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_tankModel.SetInputTurnAmount)
                .ScopeTo(_scope);

            _behaviourSelector.OnMoveValueChangedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_tankModel.SetInputMoveAmount)
                .ScopeTo(_scope);

            _actor.OnStateExitAsObservable
                .TakeUntil(_scope)
                .Subscribe(OnAnimatorStateExit)
                .ScopeTo(_scope);

            _actor.OnAnimationEventAsObservable
                .TakeUntil(_scope)
                .Subscribe(OnAnimationEvent)
                .ScopeTo(_scope);

            _actor.OnDamageReceivedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_tankModel.Damage)
                .ScopeTo(_scope);

            _actor.OnPositionChangedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_tankModel.SetPosition)
                .ScopeTo(_scope);

            _actor.OnForwardChangedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_tankModel.SetForward)
                .ScopeTo(_scope);
        }

        private void OnStateChanged(BattleTankState state)
        {
            switch (state)
            {
                case BattleTankState.Ready:
                    _actor.Ready();
                    break;
                case BattleTankState.Damage:
                    _behaviourSelector.BeginDamage();
                    _actor.Play(BattleTankAnimatorState.Damage);
                    break;
                case BattleTankState.ShotCurve:
                    _actor.Play(BattleTankAnimatorState.ShotCurve);
                    break;
                case BattleTankState.ShotStraight:
                    _actor.Play(BattleTankAnimatorState.ShotStraight);
                    break;
                case BattleTankState.Dead:
                    _actor.Dead();
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

        protected void OnAnimationEvent(string id)
        {
            Debug.LogFormat("OnAnimationEvent: id={0}", id);

            switch (id)
            {
                case "ShotCurve":
                    _actor.ShotCurve();
                    break;
                case "ShotStraight":
                    _actor.ShotStraight();
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
                    _controller.SetStartPoint();
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