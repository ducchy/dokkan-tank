using BrunoMikoski.AnimationSequencer;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace dtank
{
    public class HpGaugeNodeView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private AnimationSequencerController _enableSeq;
        [SerializeField] private AnimationSequencerController _disableSeq;
        
        public void SetEnable(bool enable)
        {
            _image.enabled = enable;
        }

        public Sequence EnableSequence()
        {
            return DOTween.Sequence()
                .AppendCallback(() => SetEnable(true))
                .Append(_enableSeq.GenerateSequence());
        }

        public Sequence DisableSequence()
        {
            return _disableSeq.GenerateSequence()
                .AppendCallback(() => SetEnable(false));
        }
    }
}