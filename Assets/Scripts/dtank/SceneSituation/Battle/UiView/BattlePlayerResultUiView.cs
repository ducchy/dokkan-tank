using System;
using BrunoMikoski.AnimationSequencer;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace dtank
{
    public class BattlePlayerResultUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _rank;
        [SerializeField] private Image _rankBg;
        [SerializeField] private Image _arrow;
        [SerializeField] private AnimationSequencerController _arrowSeq;
        [SerializeField] private AnimationSequencerController _openSeq;

        private Sequence _sequence;

        public void Setup(string playerName, Color color, int rank, bool isMine)
        {
            _playerName.text = playerName;
            _rankBg.color = color;
            _rank.text = rank.ToString();
            _arrow.gameObject.SetActive(isMine);
            color.a = 0f;
            _arrow.color = color;
            
            _openSeq.Play(PlayArrow);
            _openSeq.Pause();
        }

        public void Dispose()
        {
            _sequence?.Kill();
            _openSeq.Kill();
            _arrowSeq.Kill();
        }

        private void PlayArrow()
        {
            if (!_arrow.gameObject.activeSelf || _arrowSeq.IsPlaying)
                return;

            _arrowSeq.Play();
        }

        public void Open()
        {
            _openSeq.Resume();
        }

        public void Close()
        {
            _sequence?.Kill();

            _sequence = DOTween.Sequence()
                .Append(_group.DOFade(0f, 0.3f))
                .SetLink(gameObject)
                .Play();
        }
    }
}