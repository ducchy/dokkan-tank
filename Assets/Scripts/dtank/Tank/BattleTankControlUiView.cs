using System;
using DG.Tweening;
using GameFramework.TaskSystems;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class BattleTankControlUiView : MonoBehaviour, IBehaviourSelector, ITask
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _damageButton;
        [SerializeField] private Button _shotCurveButton;
        [SerializeField] private Button _shotStraightButton;
        [SerializeField] private Slider _verticalSlider;
        [SerializeField] private Slider _horizontalSlider;

        public Action<IAttacker> OnDamageListener { private get; set; }
        public Action OnShotCurveListener { private get; set; }
        public Action OnShotStraightListener { private get; set; }
        public Action<float> OnTurnValueChangedListener { private get; set; }
        public Action<float> OnMoveValueChangedListener { private get; set; }

        public bool IsActive => isActiveAndEnabled;

        private Sequence _sequence;

        public void Setup()
        {
            Debug.Log("PlayerBattleTankControlUiView.Setup()");

            _damageButton.onClick.AddListener(OnDamageButtonClicked);
            _shotCurveButton.onClick.AddListener(OnShotCurveButtonClicked);
            _shotStraightButton.onClick.AddListener(OnShotStraightButtonClicked);
            _verticalSlider.onValueChanged.AddListener(OnVerticalSliderValueChanged);
            _horizontalSlider.onValueChanged.AddListener(OnHorizontalSliderValueChanged);

            SetActive(false);
        }

        public void Dispose()
        {
            OnDamageListener = null;
            OnShotCurveListener = null;
            OnShotStraightListener = null;
            OnTurnValueChangedListener = null;
            OnMoveValueChangedListener = null;
        }

        public void Reset()
        {
            _verticalSlider.value = 0f;
            _horizontalSlider.value = 0f;
        }

        public void SetActive(bool flag)
        {
            if (_group == null)
                return;

            _group.alpha = flag ? 1f : 0f;
            _group.blocksRaycasts = flag;
        }

        public void BeginDamage()
        {
        }

        public void EndDamage()
        {
        }

        public void EndShotStraight()
        {
        }

        void ITask.Update()
        {
#if UNITY_EDITOR
            var vertical = Input.GetAxis("Vertical");
            var horizontal = Input.GetAxis("Horizontal");
            if (Mathf.Abs(vertical) > 0.01f)
                OnVerticalSliderValueChanged(vertical);
            if (Mathf.Abs(horizontal) > 0.01f)
                OnHorizontalSliderValueChanged(horizontal);
            if (Input.GetKeyDown(KeyCode.Space))
                OnShotStraightButtonClicked();
#endif
        }

        private void OnDamageButtonClicked()
        {
            OnDamageListener?.Invoke(null);
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

        public void Open()
        {
            _sequence?.Kill();

            SetActive(false);

            _sequence = DOTween.Sequence()
                .Append(_group.DOFade(1f, 0.3f))
                .OnComplete(() => SetActive(true))
                .SetLink(gameObject)
                .Play();
        }

        public void Close()
        {
            _sequence?.Kill();

            _sequence = DOTween.Sequence()
                .Append(_group.DOFade(0f, 0.3f))
                .OnComplete(() => SetActive(false))
                .SetLink(gameObject)
                .Play();
        }
    }
}