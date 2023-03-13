using System;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class PlayerBattleTankPresenter : IDisposable
    {
        private readonly BattleTankModel _model;
        private readonly BattleTankActor _actor;
        private readonly BattleTankControlUiView _controlUiView;
        private readonly BattleTankStatusUiView _statusUiView;

        public Action OnGameOver;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public PlayerBattleTankPresenter(
            BattleTankModel model,
            BattleTankActor actor,
            BattleTankControlUiView controlUiView,
            BattleTankStatusUiView statusUiView)
        {
            _model = model;
            _actor = actor;
            _controlUiView = controlUiView;
            _statusUiView = statusUiView;

            Bind();
            SetEvents();
        }

        public void Dispose()
        {
            _disposable.Dispose();
            OnGameOver = null;
        }

        private void Bind()
        {
            _model.BattleState
                .Subscribe(state =>
                {
                    Debug.LogFormat("OnBattleStateChanged: state={0}", state);
                    switch (state)
                    {
                        case BattleTankState.Damage:
                            _actor.Play(BattleTankAnimatorState.Damage);
                            break;
                        case BattleTankState.ShotCurve:
                            _actor.Play(BattleTankAnimatorState.ShotCurve);
                            break;
                        case BattleTankState.ShotStraight:
                            _actor.Play(BattleTankAnimatorState.ShotStraight);
                            break;
                    }
                })
                .AddTo(_disposable);

            _model.Hp
                .Subscribe(hp => { _statusUiView.SetHp(hp); }).AddTo(_disposable);
        }

        private void SetEvents()
        {
            _controlUiView.OnDamageButtonClickedListener = _model.Damage;
            _controlUiView.OnShotCurveButtonClickedListener = _model.ShotCurve;
            _controlUiView.OnShotStraightButtonClickedListener = _model.ShotStraight;
            _controlUiView.OnHorizontalSliderValueChangedListener = _actor.SetTurnAmount;
            _controlUiView.OnVerticalSliderValueChangedListener = _actor.SetMoveAmount;

            _actor.OnStateExitListener = animState =>
            {
                Debug.LogFormat("OnStateExit: animState={0}", animState);
                switch (animState)
                {
                    case BattleTankAnimatorState.Damage:
                        _model.EndDamage();
                        if (_model.Hp.Value <= 0) {
                            _actor.Dead(); 
                            OnGameOver?.Invoke();
                        }
                        break;
                    case BattleTankAnimatorState.ShotCurve:
                        _model.EndShotCurve();
                        break;
                    case BattleTankAnimatorState.ShotStraight:
                        _model.EndShotStraight();
                        break;
                }
            };
            _actor.OnAnimationEventListener = id =>
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
            };
            _actor.OnDamageReceivedListener = () => { _model.Damage(); };
        }

        public void OnChangedState(BattleState prev, BattleState current)
        {
            switch (current)
            {
                case BattleState.Ready:
                    _model.Ready();
                    _controlUiView.SetActive(false);
                    break;
                case BattleState.Playing:
                    _model.Playing();
                    _controlUiView.SetActive(true);
                    break;
                case BattleState.Result:
                    _model.Result();
                    _controlUiView.SetActive(false);
                    break;
            }
        }
    }
}