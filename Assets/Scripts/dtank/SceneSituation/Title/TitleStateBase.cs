using GameFramework.Core;
using GameFramework.StateSystems;

namespace dtank
{
	public abstract class TitleStateBase : IState<TitleState>
	{
		public abstract TitleState Key { get; }

		public abstract void OnEnter(TitleState prevKey, IScope scope);
		public abstract void OnExit(TitleState nextKey);
		public abstract void OnUpdate(float deltaTime);
	}
}
