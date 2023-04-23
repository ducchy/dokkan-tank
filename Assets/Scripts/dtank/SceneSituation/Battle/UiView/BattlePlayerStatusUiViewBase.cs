using System;
using BrunoMikoski.AnimationSequencer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public abstract class BattlePlayerStatusUiViewBase : MonoBehaviour, IBattlePlayerStatusUiView, IDisposable
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private TextMeshProUGUI _rank;
        [SerializeField] private Image _rankBg;
        [SerializeField] private HpGaugeView _hpGauge;
        [SerializeField] private AnimationSequencerController _openSeq;
        [SerializeField] private AnimationSequencerController _closeSeq;
        [SerializeField] private AnimationSequencerController _rankSeq;
        [SerializeField] private AnimationSequencerController _scoreSeq;
        [SerializeField] private AnimationSequencerController _deadSeq;

        private bool _openFlag;

        public void Setup(string playerName, int maxHp, Color color)
        {
            _playerName.text = playerName;
            _hpGauge.Setup(maxHp);
            _rankBg.color = color;
        }

        public void Dispose()
        {
            _openSeq.Kill();
            _closeSeq.Kill();
            _rankSeq.Kill();
            _scoreSeq.Kill();
            _deadSeq.Kill();
        }

        public void Reset()
        {
            _openFlag = false;
            SetActive(false);

            _hpGauge.SetHpImmediate(0);
        }

        private void SetActive(bool flag)
        {
            if (_group == null)
                return;

            _group.alpha = flag ? 1f : 0f;
        }

        public void SetScore(int score)
        {
            _scoreSeq.Kill();

            _score.text = score.ToString();

            _scoreSeq.Play();
        }

        public void SetRank(int rank)
        {
            _rankSeq.Kill();

            _rank.text = rank.ToString();

            _rankSeq.Play();
        }

        public void SetHp(int hp)
        {
            _hpGauge.SetHp(hp);

            if (_openFlag)
                _hpGauge.Play();
        }

        public void SetDeadFlag(bool flag)
        {
            if (!flag)
                return;
            
            _deadSeq.Kill();
            _deadSeq.Play();
        }

        public void Open()
        {
            if (_openFlag)
                return;

            _openFlag = true;

            _openSeq.Kill();
            _closeSeq.Kill();

            SetActive(true);

            _openSeq.Play();
        }

        public void Close()
        {
            if (!_openFlag)
                return;

            _openFlag = false;

            _openSeq.Kill();
            _closeSeq.Kill();

            _closeSeq.Play(() => SetActive(false));
        }
    }
}