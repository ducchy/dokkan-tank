using System;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
	public class TitleUiView : MonoBehaviour
	{
		[SerializeField] private Button _startButton;

		public Action OnStartButtonClickedListener;
		public Action OnCompleteStartListener;

		public void Construct()
		{
			Debug.Log("TitleUIView.Construct()");
			
			_startButton.onClick.AddListener(OnStartButtonClicked);
		}

		public void PlayStart()
		{
			Debug.Log("TitleUIView.PlayStart()");
			
			OnCompleteStartListener?.Invoke();
		}

		private void OnStartButtonClicked()
		{
			OnStartButtonClickedListener?.Invoke();
		}
	}
}
