using System;
using UniRx;
using UnityEngine;

namespace dtank
{
	public class TitlePresenter : IDisposable
	{
		private readonly TitleUiView _uiView = null;
		private readonly CompositeDisposable _disposable = new CompositeDisposable();
		
		public Action OnTouchToStart = null;
		public Action OnEndTitle = null;

		public TitlePresenter(TitleUiView uiView)
		{
			Debug.Log("TitlePresenter.TitlePresenter()");

			_uiView = uiView;

			uiView.OnClickObservable
				.Subscribe(_ => OnTouchToStart?.Invoke())
				.AddTo(_disposable);

			uiView.OnCompleteStartObservable
				.Subscribe(_ => OnEndTitle?.Invoke())
				.AddTo(_disposable);
		}

		public void Dispose()
		{
			_disposable.Dispose();

			OnTouchToStart = null;
			OnEndTitle = null;
		}

		public void OnChangeState(TitleState prev, TitleState current)
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
