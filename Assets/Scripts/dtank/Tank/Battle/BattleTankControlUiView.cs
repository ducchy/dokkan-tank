using System;
using DG.Tweening;
using UniRx;
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
        [SerializeField] private Toggle _autoToggle;

        private readonly Subject<IAttacker> _onDamageSubject = new();
        private readonly Subject<bool> _onAutoToggleValueChanged = new();

        public IObservable<IAttacker> OnDamageAsObservable => _onDamageSubject;
        public IObservable<Unit> OnShotCurveAsObservable => _shotCurveButton.OnClickAsObservable();
        public IObservable<Unit> OnShotStraightAsObservable => _shotStraightButton.OnClickAsObservable();
        public IObservable<float> OnTurnValueChangedAsObservable => _horizontalSlider.OnValueChangedAsObservable();
        public IObservable<float> OnMoveValueChangedAsObservable => _verticalSlider.OnValueChangedAsObservable();
        public IObservable<bool> OnAutoToggleValueChangedAsObservable => _onAutoToggleValueChanged;

        private Sequence _sequence;

        public void Setup()
        {
            _damageButton.onClick.AddListener(OnDamageButtonClicked);
            _autoToggle.onValueChanged.AddListener(OnAutoToggleValueChanged);

            Reset();
        }

        public void Dispose()
        {
            _damageButton.onClick.RemoveListener(OnDamageButtonClicked);
            _autoToggle.onValueChanged.RemoveListener(OnAutoToggleValueChanged);

            _onDamageSubject.Dispose();
        }

        public void Reset()
        {
            _verticalSlider.value = 0f;
            _horizontalSlider.value = 0f;

            SetActive(false);
        }

        public void SetActive(bool flag)
        {
            if (_group == null)
                return;

            _group.alpha = flag ? 1f : 0f;
            _group.blocksRaycasts = flag;
        }

        private void SetInteractive(bool interactable)
        {
            _damageButton.interactable = interactable;
            _shotCurveButton.interactable = interactable;
            _shotStraightButton.interactable = interactable;
            _horizontalSlider.interactable = interactable;
            _verticalSlider.interactable = interactable;
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

        void IBehaviourSelector.Update()
        {
#if UNITY_EDITOR
            var vertical = Input.GetAxis("Vertical");
            var horizontal = Input.GetAxis("Horizontal");
            if (Mathf.Abs(vertical) > 0.01f)
                _verticalSlider.value = vertical;
            if (Mathf.Abs(horizontal) > 0.01f)
                _horizontalSlider.value = horizontal;
            if (Input.GetKeyDown(KeyCode.Space))
                _shotStraightButton.onClick.Invoke();
#endif
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

        private void OnDamageButtonClicked()
        {
            _onDamageSubject.OnNext(null);
        }

        private void OnAutoToggleValueChanged(bool isOn)
        {
            _onAutoToggleValueChanged.OnNext(isOn);
            
            SetInteractive(!isOn);
        }
    }
}