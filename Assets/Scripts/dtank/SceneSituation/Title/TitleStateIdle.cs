using GameFramework.Core;

namespace dtank
{
    public class TitleStateIdle : TitleStateBase
    {
        public override TitleState Key => TitleState.Idle;

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