using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class HpGaugeNodeView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Color _enableColor;
        [SerializeField] private Color _disableColor;
        
        public void SetEnable(bool enable)
        {
            _image.color = enable ? _enableColor : _disableColor;
            _image.transform.localScale = Vector3.one * (enable ? 1f : 0.5f);
        }

        public Sequence EnableSequence()
        {
            return DOTween.Sequence()
                .Append(_image.DOColor(_enableColor, 0.3f))
                .Join(_image.transform.DOScale(1f, 0.3f));
        }

        public Sequence DisableSequence()
        {
            return DOTween.Sequence()
                .Append(_image.DOColor(_disableColor, 0.3f))
                .Join(_image.transform.DOScale(0.5f, 0.3f));
        }
    }
}