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

        public IObservable<Unit> onClickRetryButtonAsObserbable => _retryButton.OnClickAsObservable();
        public IObservable<Unit> onClickQuitButtonAsObserbable => _quitButton.OnClickAsObservable();

        public void Initialize()
        {
            Debug.Log("BattleUiView.Initialize()");
        }
    }
}