using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
	public class TitleUiView : MonoBehaviour
	{
		[SerializeField] private Button _startButton;

		private readonly Subject<Unit> _onCompleteStart = new Subject<Unit>();

		public IObservable<Unit> OnClickAsObservable => _startButton.OnClickAsObservable();
		public IObservable<Unit> OnCompleteStart => _onCompleteStart;


		public void Initialize()
		{
			Debug.Log("TitleUIView.Initialize()");
		}

		public void PlayStart()
		{
			Debug.Log("TitleUIView.PlayStart()");
			
			_onCompleteStart.OnNext(Unit.Default);
		}
	}
}
