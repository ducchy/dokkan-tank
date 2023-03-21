using System;
using GameFramework.Core;
using UniRx;
using UnityEngine;

namespace dtank
{
    public abstract class BattleTankPresenterBase : IDisposable
    {
        protected readonly BattleTankController _controller;
        protected readonly BattleTankModel _model;
        protected readonly BattleTankActor _actor;
        protected readonly IBehaviourSelector _behaviourSelector;
        protected readonly DisposableScope _scope = new DisposableScope();

        protected BattleTankPresenterBase(
            BattleTankController controller,
            BattleTankModel model,
            BattleTankActor actor,
            IBehaviourSelector behaviourSelector)
        {
            _controller = controller;
            _model = model;
            _actor = actor;
            _behaviourSelector = behaviourSelector;
        }

        public virtual void Dispose()
        {
            _scope.Dispose();
        }

        protected void Bind()
        {
            _model.BattleState
                .TakeUntil(_scope)
                .Subscribe(OnStateChanged)
                .ScopeTo(_scope);

            _model.MoveAmount
                .TakeUntil(_scope)
                .Subscribe(_actor.SetMoveAmount)
                .ScopeTo(_scope);

            _model.TurnAmount
                .TakeUntil(_scope)
                .Subscribe(_actor.SetTurnAmount)
                .ScopeTo(_scope);

            _model.InvincibleFlag
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
                .Subscribe(_model.Damage)
                .ScopeTo(_scope);

            _behaviourSelector.OnShotCurveAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _model.ShotCurve())
                .ScopeTo(_scope);

            _behaviourSelector.OnShotStraightAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _model.ShotStraight())
                .ScopeTo(_scope);

            _behaviourSelector.OnTurnValueChangedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_model.SetInputTurnAmount)
                .ScopeTo(_scope);
            
            _behaviourSelector.OnMoveValueChangedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_model.SetInputMoveAmount)
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
                .Subscribe(_model.Damage)
                .ScopeTo(_scope);
            
            _actor.OnPositionChangedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_model.SetPosition)
                .ScopeTo(_scope);
            
            _actor.OnForwardChangedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_model.SetForward)
                .ScopeTo(_scope);
        }

        protected virtual void OnStateChanged(BattleTankState state)
        {
            switch (state)
            {
                case BattleTankState.Ready:
                    _actor.Ready();
                    break;
                case BattleTankState.Damage:
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
                    break;
            }
        }

        private void OnAnimatorStateExit(BattleTankAnimatorState animState)
        {
            Debug.LogFormat("OnAnimatorStateExit: animState={0}", animState);

            switch (animState)
            {
                case BattleTankAnimatorState.Damage:
                    _model.EndDamage();
                    _behaviourSelector.EndDamage();
                    break;
                case BattleTankAnimatorState.ShotCurve:
                    _model.EndShotCurve();
                    break;
                case BattleTankAnimatorState.ShotStraight:
                    _model.EndShotStraight();
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

        public virtual void OnChangedState(BattleState current)
        {
            switch (current)
            {
                case BattleState.Ready:
                    _behaviourSelector.Reset();
                    _controller.SetStartPoint();
                    _model.Ready();
                    break;
                case BattleState.Playing:
                    _model.Playing();
                    break;
                case BattleState.Result:
                    _model.Result();
                    break;
            }
        }
    }
}