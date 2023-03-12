using System;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class PlayerBattleTankControlUiView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _damageButton;
        [SerializeField] private Button _shotCurveButton;
        [SerializeField] private Button _shotStraightButton;

        public Action OnDamageButtonClickedListener;
        public Action OnShotCurveButtonClickedListener;
        public Action OnShotStraightButtonClickedListener;
        
        public void Construct()
        {
            Debug.Log("PlayerBattleTankControlUiView.Construct()");

            _damageButton.onClick.AddListener(OnDamageButtonClicked);
            _shotCurveButton.onClick.AddListener(OnShotCurveButtonClicked);
            _shotStraightButton.onClick.AddListener(OnShotStraightButtonClicked);
            
            SetActive(false);
        }

        public void SetActive(bool flag)
        {
            if (_group == null)
                return;
            
            _group.alpha = flag ? 1f : 0f;
            _group.blocksRaycasts = flag;
        }

        private void OnDamageButtonClicked()
        {
            OnDamageButtonClickedListener?.Invoke();
        }

        private void OnShotCurveButtonClicked()
        {
            OnShotCurveButtonClickedListener?.Invoke();
        }

        private void OnShotStraightButtonClicked()
        {
            OnShotStraightButtonClickedListener?.Invoke();
        }
    }
}