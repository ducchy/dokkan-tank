using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class BattleTankStatusUiView : MonoBehaviour
    {
        [SerializeField] private Image[] _hpImages;

        private int _hp;

        public void Construct()
        {
        }

        public void Reset()
        {
            SetHp(3);
        }

        public void SetHp(int hp)
        {
            if (_hp == hp)
                return;

            for (var i = 0; i < _hpImages.Length; i++)
            {
                var hpImage = _hpImages[i];
                hpImage.enabled = (i + 1) <= hp;
            }

            _hp = hp;
        }
    }
}