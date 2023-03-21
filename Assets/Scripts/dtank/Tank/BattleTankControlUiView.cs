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

        private readonly Subject<IAttacker> _onDamageSubject = new Subject<IAttacker>();
        public IObservable<IAttacker> OnDamageAsObservable => _onDamageSubject;
        public IObservable<Unit> OnShotCurveAsObservable => _shotCurveButton.OnClickAsObservable();
        public IObservable<Unit> OnShotStraightAsObservable => _shotStraightButton.OnClickAsObservable();
        public IObservable<float> OnTurnValueChangedAsObservable => _horizontalSlider.OnValueChangedAsObservable();
        public IObservable<float> OnMoveValueChangedAsObservable => _verticalSlider.OnValueChangedAsObservable();
        
        private Sequence _sequence;

        public void Setup()
        {
            Debug.Log("PlayerBattleTankControlUiView.Setup()");

            _damageButton.onClick.AddListener(OnDamageButtonClicked);

            SetActive(false);
        }

        public void Dispose()
        {
            _onDamageSubject.Dispose();
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

        public void OnUpdate()
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
    }
}