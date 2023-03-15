using System;
using UniRx;
using UnityEngine;

namespace dtank
{
	public class TitlePresenter : IDisposable
	{
		private readonly TitleUiView _uiView;
		private readonly TitleCamera _camera;
		private readonly CompositeDisposable _disposable = new CompositeDisposable();
		
		public Action OnTouchToStart = null;
		public Action OnEndTitle = null;

		public TitlePresenter(TitleUiView uiView, TitleCamera camera)
		{
			Debug.Log("TitlePresenter.TitlePresenter()");

			_uiView = uiView;
			_camera = camera;
			
			SetEvent();
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
				case TitleState.Idle:
					_camera.Play();
					break;
				case TitleState.Start:
					_uiView.PlayStart();
					break;
			}
		}

		private void SetEvent()
		{
			_uiView.OnStartButtonClickedListener = OnStartButtonClicked;
			_uiView.OnCompleteStartListener = OnCompleteStart;
		}

		private void OnStartButtonClicked()
		{
			OnTouchToStart?.Invoke();
		}

		private void OnCompleteStart()
		{
			OnEndTitle?.Invoke();
		}
	}
}
