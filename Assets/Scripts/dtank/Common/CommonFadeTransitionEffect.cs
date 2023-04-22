using UnityEngine;

namespace dtank
{
    public class CommonFadeTransitionEffect : FadeTransitionEffect
    {
        private const float FadeDuration = 0.3f;

        public CommonFadeTransitionEffect(bool fadeInFlag, bool fadeOutFlag)
            : base(Color.black,
                fadeInFlag ? FadeDuration : 0f,
                fadeOutFlag ? FadeDuration : 0f)
        {
        }
    }
}