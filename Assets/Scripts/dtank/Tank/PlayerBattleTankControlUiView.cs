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
        [SerializeField] private Slider _verticalSlider;
        [SerializeField] private Slider _horizontalSlider;

        public Action OnDamageButtonClickedListener;
        public Action OnShotCurveButtonClickedListener;
        public Action OnShotStraightButtonClickedListener;
        public Action<float> OnVerticalSliderValueChangedListener;
        public Action<float> OnHorizontalSliderValueChangedListener;

        public void Construct()
        {
            Debug.Log("PlayerBattleTankControlUiView.Construct()");

            _damageButton.onClick.AddListener(OnDamageButtonClicked);
            _shotCurveButton.onClick.AddListener(OnShotCurveButtonClicked);
            _shotStraightButton.onClick.AddListener(OnShotStraightButtonClicked);
            _verticalSlider.onValueChanged.AddListener(OnVerticalSliderValueChanged);
            _horizontalSlider.onValueChanged.AddListener(OnHorizontalSliderValueChanged);

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

        private void OnVerticalSliderValueChanged(float value)
        {
            OnVerticalSliderValueChangedListener?.Invoke(value);
        }

        private void OnHorizontalSliderValueChanged(float value)
        {
            OnHorizontalSliderValueChangedListener?.Invoke(value);
        }

#if UNITY_EDITOR
        private void Update()
        {
            var vertical = Input.GetAxis("Vertical");
            var horizontal = Input.GetAxis("Horizontal");
            if (Mathf.Abs(vertical) > 0.01f)
                OnVerticalSliderValueChanged(vertical);
            if (Mathf.Abs(horizontal) > 0.01f)
                OnHorizontalSliderValueChanged(horizontal);
            if (Input.GetKeyDown(KeyCode.Space))
                OnShotStraightButtonClicked();
        }
#endif
    }
}