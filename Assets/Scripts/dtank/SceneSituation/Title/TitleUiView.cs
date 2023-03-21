using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class TitleUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private Button _startButton;

        public IObservable<Unit> OnStartButtonClickAsObservable => _startButton.OnClickAsObservable();
        
        public void Dispose()
        {
        }
    }
}