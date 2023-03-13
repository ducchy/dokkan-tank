using System;
using UniRx;
using UnityEngine;

namespace dtank
{
    public abstract class BattleTankPresenterBase : IDisposable
    {
        protected readonly BattleTankModel Model;
        protected readonly BattleTankActor Actor;
        protected readonly IBehaviourSelector BehaviourSelector;

        protected readonly CompositeDisposable Disposable = new CompositeDisposable();

        protected BattleTankPresenterBase(
            BattleTankModel model,
            BattleTankActor actor,
            IBehaviourSelector behaviourSelector)
        {
            Model = model;
            Actor = actor;
            BehaviourSelector = behaviourSelector;

            Bind();
            SetEvents();
        }

        public virtual void Dispose()
        {
            Disposable.Dispose();
        }

        private void Bind()
        {
            Model.BattleState
                .Subscribe(OnStateChanged)
                .AddTo(Disposable);
        }

        private void SetEvents()
        {
            BehaviourSelector.OnDamageListener = Model.Damage;
            BehaviourSelector.OnShotCurveListener = Model.ShotCurve;
            BehaviourSelector.OnShotStraightListener = Model.ShotStraight;
            BehaviourSelector.OnTurnValueChangedListener = Actor.SetTurnAmount;
            BehaviourSelector.OnMoveValueChangedListener = Actor.SetMoveAmount;

            Actor.OnStateExitListener = OnAnimatorStateExit;
            Actor.OnAnimationEventListener = OnAnimationEvent;
            Actor.OnDamageReceivedListener = Model.Damage;
            Actor.OnPositionChangedListener = Model.SetPosition;
            Actor.OnForwardChangedListener = Model.SetForward;
        }

        protected virtual void BindInternal()
        {
        }

        protected virtual void OnStateChanged(BattleTankState state)
        {
            Debug.LogFormat("OnStateChanged: state={0}", state);

            switch (state)
            {
                case BattleTankState.Damage:
                    Actor.Play(BattleTankAnimatorState.Damage);
                    break;
                case BattleTankState.ShotCurve:
                    Actor.Play(BattleTankAnimatorState.ShotCurve);
                    break;
                case BattleTankState.ShotStraight:
                    Actor.Play(BattleTankAnimatorState.ShotStraight);
                    break;
            }
        }

        protected virtual void OnAnimatorStateExit(BattleTankAnimatorState animState)
        {
            Debug.LogFormat("OnAnimatorStateExit: animState={0}", animState);

            switch (animState)
            {
                case BattleTankAnimatorState.Damage:
                    Model.EndDamage();
                    if (Model.Hp.Value <= 0)
                        OnDead();
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

        public virtual void OnChangedState(BattleState prev, BattleState current)
        {
            switch (current)
            {
                case BattleState.Ready:
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