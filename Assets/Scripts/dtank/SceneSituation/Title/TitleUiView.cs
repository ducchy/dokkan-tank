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

		public IObservable<Unit> OnClickObservable => _startButton.OnClickAsObservable();
		public IObservable<Unit> OnCompleteStartObservable => _onCompleteStart;


		public void Construct()
		{
			Debug.Log("TitleUIView.Construct()");
		}

		public void PlayStart()
		{
			Debug.Log("TitleUIView.PlayStart()");
			
			_onCompleteStart.OnNext(Unit.Default);
		}
	}
}
