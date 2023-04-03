using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace dtank
{
    public class HpGaugeView : MonoBehaviour, IDisposable
    {
        [Serializable]
        private enum Alignment
        {
            Left,
            Right, 
        }
        
        [SerializeField] private HpGaugeNodeView _nodePrefab;
        [SerializeField] private HorizontalLayoutGroup _nodeGroup;
        [SerializeField] private Alignment _alignment;

        public int DisplayHp { get; private set; }
        public int CurrentHp { get; private set; }
        
        private HpGaugeNodeView[] _nodes;
        private Sequence _sequence;

        public void Setup(int maxHp)
        {
            _nodeGroup.childAlignment = _alignment == Alignment.Left
                ? TextAnchor.MiddleLeft
                : TextAnchor.MiddleRight;
            
            _nodes = new HpGaugeNodeView[maxHp];
            for (var i = 0; i < maxHp; i++) {
                var node = Instantiate(_nodePrefab, _nodeGroup.transform);
                if (_alignment == Alignment.Right)
                    node.transform.SetAsFirstSibling();
                _nodes[i] = node;
            }
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