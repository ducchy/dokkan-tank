using System;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class BattleTankControlUiView : MonoBehaviour, IBehaviourSelector
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _damageButton;
        [SerializeField] private Button _shotCurveButton;
        [SerializeField] private Button _shotStraightButton;
        [SerializeField] private Slider _verticalSlider;
        [SerializeField] private Slider _horizontalSlider;
        
        public Action OnDamageListener { private get; set; }
        public Action OnShotCurveListener { private get; set; }
        public Action OnShotStraightListener { private get; set; }
        public Action<float> OnTurnValueChangedListener { private get; set; }
        public Action<float> OnMoveValueChangedListener { private get; set; }

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
            OnDamageListener?.Invoke();
        }

        private void OnShotCurveButtonClicked()
        {
            OnShotCurveListener?.Invoke();
        }

        private void OnShotStraightButtonClicked()
        {
            OnShotStraightListener?.Invoke();
        }

        private void OnVerticalSliderValueChanged(float value)
        {
            OnMoveValueChangedListener?.Invoke(value);
        }

        private void OnHorizontalSliderValueChanged(float value)
        {
            OnTurnValueChangedListener?.Invoke(value);
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