using System;
using System.Collections.Generic;

namespace dtank
{
    public class BattleTankActorContainer : IDisposable
    {
        private readonly Dictionary<int, BattleTankActor> _actorDictionary;
        public IReadOnlyDictionary<int, BattleTankActor> ActorDictionary => _actorDictionary;

        public BattleTankActorContainer(Dictionary<int, BattleTankActor> actorDictionary)
        {
            _actorDictionary = actorDictionary;
        }

        public void Dispose()
        {
            if (_actorDictionary == null)
                return;
            
            foreach (var battleTankActor in _actorDictionary.Values)
                battleTankActor.Dispose();
            
            _actorDictionary.Clear();
        }
    }
}