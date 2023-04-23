using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameFramework.Core;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class BattleResultUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private BattlePlayerResultUiView _playerResultUiPrefab;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TextMeshProUGUI _resultLabel;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private RectTransform _retryButtonRect;
        [SerializeField] private RectTransform _quitButtonRect;
        [SerializeField] private RectTransform _playerResultUiParent;

        public IObservable<Unit> OnRetryButtonClickObservable => _retryButton.OnClickAsObservable();
        public IObservable<Unit> OnQuitButtonClickObservable => _quitButton.OnClickAsObservable();

        private Sequence _resultSeq;
        private FadeController _fadeController;
        private readonly List<BattlePlayerResultUiView> _playerResultUiViews = new();
        private readonly DisposableScope _fadeScope = new();

        public void Setup(FadeController fadeController)
        {
            _fadeController = fadeController;
        }

        public void Dispose()
        {
            _resultSeq?.Kill();

            _fadeScope.Dispose();
        }

        public void Reset()
        {
            SetActive(false);
        }

        public void SetActive(bool flag)
        {
            if (_group == null)
                return;

            _group.alpha = flag ? 1f : 0f;
            _group.blocksRaycasts = flag;
        }

        public void PlayResult(bool winFlag, IReadOnlyList<BattleTankModel> tankModels)
        {
            _fadeScope.Dispose();

            SetActive(false);

            foreach (var tankModel in tankModels.OrderByDescending(model => model.Rank.Value))
            {
                var playerResultUiView = Instantiate(_playerResultUiPrefab, _playerResultUiParent);
                playerResultUiView.transform.SetAsFirstSibling();
                playerResultUiView.Setup(tankModel.Name, tankModel.ActorModel.SetupData.Color, tankModel.Rank.Value,
                    tankModel.CharacterType == CharacterType.Player);
                _playerResultUiViews.Add(playerResultUiView);
            }

            void OnCompleteFadeIn()
            {
                _resultSeq?.Kill();

                _resultLabel.text = winFlag ? "WIN!" : "LOSE!";
                _resultLabel.color = new Color(0.2f, 0.2f, 0.2f, 0f);
                _resultLabel.transform.localScale = Vector3.one * 3f;

                _retryButtonRect.anchoredPosition = new Vector2(200f, 50f);
                _quitButtonRect.anchoredPosition = new Vector2(200f, 250f);

                _resultSeq = DOTween.Sequence()
                    .Append(_group.DOFade(1f, 0.3f));

                const float interval = 0.3f;
                foreach (var playerResultUiView in _playerResultUiViews)
                {
                    _resultSeq
                        .AppendInterval(interval)
                        .AppendCallback(playerResultUiView.Open);
                }

                _resultSeq
                    .Append(_resultLabel.DOFade(1f, 0.3f).SetEase(Ease.OutQuad))
                    .Join(_resultLabel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutQuad))
                    .AppendInterval(1f)
                    .Append(_retryButtonRect.DOAnchorPosX(-50f, 0.3f).SetEase(Ease.OutQuad))
                    .Append(_quitButtonRect.DOAnchorPosX(-50f, 0.3f).SetEase(Ease.OutQuad))
                    .OnComplete(() => { SetActive(true); })
                    .SetLink(gameObject)
                    .Play();
            }

            _fadeController.FadeIn(0.5f, OnCompleteFadeIn, _fadeScope);
        }
    }
}