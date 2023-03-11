using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class BattlePlayingUiView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _endButton;
        
        public IObservable<Unit> OnEndObservable => _endButton.OnClickAsObservable();
        
        public void Construct()
        {
            Debug.Log("BattlePlayingUiView.Construct()");

            SetActive(false);
        }

        public void SetActive(bool flag)
        {
            _group.alpha = flag ? 1f : 0f;
            _group.blocksRaycasts = flag;
        }
    }
}