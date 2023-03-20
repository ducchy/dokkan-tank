using System;
using UniRx;
using UnityEngine;

namespace dtank
{
    public abstract class BattleTankPresenterBase : IDisposable
    {
        protected readonly BattleTankController Controller;
        protected readonly BattleTankModel Model;
        protected readonly BattleTankActor Actor;
        protected readonly IBehaviourSelector BehaviourSelector;
        protected readonly CompositeDisposable Disposable = new CompositeDisposable();

        protected BattleTankPresenterBase(
            BattleTankController controller,
            BattleTankModel model,
            BattleTankActor actor,
            IBehaviourSelector behaviourSelector)
        {
            Controller = controller;
            Model = model;
            Actor = actor;
            BehaviourSelector = behaviourSelector;
        }

        public virtual void Dispose()
        {
            Disposable.Dispose();
        }

        protected void Bind()
        {
            Model.BattleState
                .Subscribe(OnStateChanged)
                .AddTo(Disposable);

            Model.Hp
                .Subscribe(OnHpChanged)
                .AddTo(Disposable);

            Model.MoveAmount
                .Subscribe(Actor.SetMoveAmount)
                .AddTo(Disposable);

            Model.TurnAmount
                .Subscribe(Actor.SetTurnAmount)
                .AddTo(Disposable);

            Model.InvincibleFlag
                .Subscribe(Actor.SetInvincible)
                .AddTo(Disposable);
        }

        protected void SetEvents()
        {
            BehaviourSelector.OnDamageListener = Model.Damage;
            BehaviourSelector.OnShotCurveListener = Model.ShotCurve;
            BehaviourSelector.OnShotStraightListener = Model.ShotStraight;
            BehaviourSelector.OnTurnValueChangedListener = Model.SetInputTurnAmount;
            BehaviourSelector.OnMoveValueChangedListener = Model.SetInputMoveAmount;

            Actor.OnStateExitListener = OnAnimatorStateExit;
            Actor.OnAnimationEventListener = OnAnimationEvent;
            Actor.OnDamageReceivedListener = Model.Damage;
            Actor.OnPositionChangedListener = Model.SetPosition;
            Actor.OnForwardChangedListener = Model.SetForward;
        }

        protected virtual void OnStateChanged(BattleTankState state)
        {
            Debug.LogFormat("OnStateChanged: state={0}", state);

            switch (state)
            {
                case BattleTankState.Ready:
                    Actor.Ready();
                    break;
                case BattleTankState.Damage:
                    Actor.Play(BattleTankAnimatorState.Damage);
                    break;
                case BattleTankState.ShotCurve:
                    Actor.Play(BattleTankAnimatorState.ShotCurve);
                    break;
                case BattleTankState.ShotStraight:
                    Actor.Play(BattleTankAnimatorState.ShotStraight);
                    break;
                case BattleTankState.Dead:
                    OnDead();
                    break;
            }
        }

        protected virtual void OnHpChanged(int hp)
        {
        }

        protected virtual void OnAnimatorStateExit(BattleTankAnimatorState animState)
        {
            Debug.LogFormat("OnAnimatorStateExit: animState={0}", animState);

            switch (animState)
            {
                case BattleTankAnimatorState.Damage:
                    Model.EndDamage();
                    break;
                case BattleTankAnimatorState.ShotCurve:
                    Model.EndShotCurve();
                    break;
                case BattleTankAnimatorState.ShotStraight:
                    Model.EndShotStraight();
                    break;
            }
        }

        protected virtual void OnAnimationEvent(string id)
        {
            Debug.LogFormat("OnAnimationEvent: id={0}", id);

            switch (id)
            {
                case "ShotCurve":
                    Actor.ShotCurve();
                    break;
                case "ShotStraight":
                    Actor.ShotStraight();
                    break;
            }
        }

        protected virtual void OnDead()
        {
            Actor.Dead();
        }

        public virtual void OnChangedState(BattleState current)
        {
            switch (current)
            {
                case BattleState.Ready:
                    BehaviourSelector.Reset();
                    Controller.SetStartPoint();
                    Model.Ready();
                    break;
                case BattleState.Playing:
                    Model.Playing();
                    break;
                case BattleState.Result:
                    Model.Result();
                    break;
            }
        }
    }
}