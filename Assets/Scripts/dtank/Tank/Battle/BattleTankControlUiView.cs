using System;
using BrunoMikoski.AnimationSequencer;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class BattleTankControlUiView : MonoBehaviour, ITankBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private CanvasGroup _controllerGroup;
        [SerializeField] private Button _shotCurveButton;
        [SerializeField] private Button _shotStraightButton;
        [SerializeField] private Slider _verticalSlider;
        [SerializeField] private Slider _horizontalSlider;
        [SerializeField] private Toggle _autoToggle;
        [SerializeField] private AnimationSequencerController _autoOnSeq;
        [SerializeField] private AnimationSequencerController _autoOffSeq;

        private readonly Subject<bool> _onAutoToggleValueChanged = new();

        public IObservable<Unit> OnShotCurveAsObservable => _shotCurveButton.OnClickAsObservable();
        public IObservable<Unit> OnShotStraightAsObservable => _shotStraightButton.OnClickAsObservable();
        public IObservable<float> OnTurnValueChangedAsObservable => _horizontalSlider.OnValueChangedAsObservable();
        public IObservable<float> OnMoveValueChangedAsObservable => _verticalSlider.OnValueChangedAsObservable();
        public IObservable<bool> OnAutoToggleValueChangedAsObservable => _onAutoToggleValueChanged;

        private Sequence _sequence;

        public void Setup()
        {
            _autoToggle.onValueChanged.AddListener(OnAutoToggleValueChanged);

            Reset();
        }

        public void Dispose()
        {
            _autoToggle.onValueChanged.RemoveListener(OnAutoToggleValueChanged);
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

        public void BeginDamage()
        {
        }

        public void EndDamage()
        {
        }

        public void EndShotStraight()
        {
        }

        void ITankBehaviour.Update()
        {
#if UNITY_EDITOR
            var vertical = Input.GetAxis("Vertical");
            var horizontal = Input.GetAxis("Horizontal");

            _verticalSlider.value = Mathf.Abs(vertical) > 0.01f ? vertical : 0f;
            _horizontalSlider.value = Mathf.Abs(horizontal) > 0.01f ? horizontal : 0f;

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

        private void OnAutoToggleValueChanged(bool isOn)
        {
            _onAutoToggleValueChanged.OnNext(isOn);

            _autoOnSeq.Kill();
            _autoOffSeq.Kill();

            if (isOn)
            {
                _autoOnSeq.Play();
                _controllerGroup.blocksRaycasts = false;
            }
            else
                _autoOffSeq.Play(() =>
                    _controllerGroup.blocksRaycasts = true);
        }
    }
}