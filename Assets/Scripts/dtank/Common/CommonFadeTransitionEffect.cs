using UnityEngine;

namespace dtank
{
    public class CommonFadeTransitionEffect : FadeTransitionEffect
    {
        public CommonFadeTransitionEffect(float enterDuration, float exitDuration) : base(Color.black, enterDuration, exitDuration)
        {
        }
    }
}