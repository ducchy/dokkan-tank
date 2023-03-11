using GameFramework.Core;
using GameFramework.SituationSystems;
using GameFramework.StateSystems;

namespace dtank
{
    public abstract class BattleStateBase : IState<BattleState>
    {
        protected StateContainer<BattleStateBase, BattleState> StateContainer =>
            _stateContainer ??= Services.Get<StateContainer<BattleStateBase, BattleState>>();
        private StateContainer<BattleStateBase, BattleState> _stateContainer;
        
        protected SceneSituationContainer SceneSituationContainer =>
            _sceneSituationContainer ??= Services.Get<SceneSituationContainer>();
        private SceneSituationContainer _sceneSituationContainer;
        
        public abstract BattleState Key { get; }

        public abstract void OnEnter(BattleState prevKey, IScope scope);
        public abstract void OnExit(BattleState nextKey);
        public abstract void OnUpdate(float deltaTime);
    }
}
