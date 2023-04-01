using GameFramework.Core;

namespace dtank
{
    public class TitleStateStart : TitleStateBase
    {
        public override TitleState Key => TitleState.Start;

        public override void OnEnter(TitleState prevKey, IScope scope)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit(TitleState nextKey)
        {
        }
    }
}