using GameFramework.Core;
using GameFramework.SituationSystems;
using GameFramework.StateSystems;
using System;
using UniRx;
using UnityEngine;

namespace dtank
{
	public class TitlePresenter : IDisposable
	{
		private readonly TitleUiView _uiView;
		
		private readonly StateContainer<TitleStateBase, TitleState> _stateContainer;
		private readonly CompositeDisposable _disposable = new CompositeDisposable();


		public TitlePresenter(TitleUiView uiView)
		{
			Debug.Log("TitlePresenter.TitlePresenter()");

			_uiView = uiView;

			_stateContainer = Services.Get<StateContainer<TitleStateBase, TitleState>>();
			_stateContainer.OnChangedState += OnChangeState;

			var situationContainer = Services.Get<SceneSituationContainer>();

			uiView.OnClickAsObservable
				.Subscribe(_ => _stateContainer.Change(TitleState.Start))
				.AddTo(_disposable);

			uiView.OnCompleteStart
				.Subscribe(_ => situationContainer.Transition(new BattleSceneSituation()))
				.AddTo(_disposable);
		}

		public void Dispose()
		{
			_stateContainer.OnChangedState -= OnChangeState;
			_disposable.Dispose();
		}

		private void OnChangeState(TitleState prev, TitleState current)
		{
			switch (current)
			{
				case TitleState.Start:
					_uiView.PlayStart();
					break;
			}
		}
	}
}
