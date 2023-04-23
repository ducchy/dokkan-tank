using System;
using BrunoMikoski.AnimationSequencer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class BattlePlayerResultUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _rank;
        [SerializeField] private Image _rankBg;
        [SerializeField] private Image _arrow;
        [SerializeField] private AnimationSequencerController _arrowSeq;
        [SerializeField] private AnimationSequencerController _openSeq;

        public void Setup(string playerName, Color color, int rank, bool isMine)
        {
            _playerName.text = playerName;
            _rankBg.color = color;
            _rank.text = rank.ToString();
            _arrow.gameObject.SetActive(isMine);
            _arrow.color = color;
            
            _openSeq.Play(PlayArrow);
            _openSeq.Pause();
        }

        public void Dispose()
        {
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
    }
}