using System;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace dtank
{
    public class BattleUiView : MonoBehaviour
    {
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;

        public IObservable<Unit> OnClickRetryButtonAsObservable => _retryButton.OnClickAsObservable();
        public IObservable<Unit> OnClickQuitButtonAsObservable => _quitButton.OnClickAsObservable();

        public void Initialize()
        {
            Debug.Log("BattleUiView.Initialize()");
        }
    }
}