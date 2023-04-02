using System;
using DG.Tweening;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace dtank
{
    public class HpGaugeView : MonoBehaviour, IDisposable
    {
        [SerializeField] private HpGaugeNodeView _nodePrefab;
        [SerializeField] private Transform _nodeParent;

        public int DisplayHp { get; private set; }
        public int CurrentHp { get; private set; }
        
        private HpGaugeNodeView[] _nodes;
        private Sequence _sequence;

        public void Setup(int maxHp)
        {
            _nodes = new HpGaugeNodeView[maxHp];
            for (var i = 0; i < maxHp; i++)
                _nodes[i] = Instantiate(_nodePrefab, _nodeParent);
        }

        public void Dispose()
        {
            _sequence?.Kill();
        }

        public void SetHp(int hp)
        {
            if (CurrentHp == hp)
                return;

            Debug.Log($"[BattleTankStatusUiView] SetHp: {CurrentHp} -> {hp}");

            CurrentHp = hp;
        }

        public void SetHpImmediate(int hp)
        {
            for (var i = 0; i < _nodes.Length; i++)
                _nodes[i].SetEnable(i < hp);

            CurrentHp = DisplayHp = hp;
        }

        public void Play(Action onComplete = null)
        {
            PlayChangeHp(DisplayHp, CurrentHp, onComplete);

            DisplayHp = CurrentHp;
        }

        private void PlayChangeHp(int from, int to, Action onComplete = null)
        {
            if (from == to)
            {
                onComplete?.Invoke();
                return;
            }

            Debug.Log($"[BattleTankStatusUiView] PlayChangeHp: {from} -> {to}");

            _sequence?.Complete();
            _sequence = DOTween.Sequence()
                .SetLink(gameObject);

            if (from < to)
            {
                for (var i = from; i < to; i++)
                {
                    var node = _nodes[i];
                    node.SetEnable(false);
                    _sequence.Append(node.EnableSequence());
                }
            }
            else
            {
                for (var i = from - 1; i >= to; i--)
                {
                    var node = _nodes[i];
                    node.SetEnable(true);
                    _sequence.Append(node.DisableSequence());
                }
            }

            _sequence
                .OnComplete(() => onComplete?.Invoke())
                .Play();
        }
    }
}