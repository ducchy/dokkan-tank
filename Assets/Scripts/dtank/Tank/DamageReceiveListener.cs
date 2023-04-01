using System;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class DamageReceiveListener : MonoBehaviour, IDamageReceiver
    {
        private Func<IAttacker, bool> _conditionFunc;
        
        private readonly Subject<IAttacker> _receiveDamageSubject = new();
        public IObservable<IAttacker> ReceiveDamageObservable => _receiveDamageSubject.AsObservable();

        string IDamageReceiver.Name => name;

        private void OnDestroy()
        {
            _receiveDamageSubject.Dispose();
        }

        public void SetCondition(Func<IAttacker, bool> conditionFunc)
        {
            _conditionFunc = conditionFunc;
        }
        
        public bool ReceiveDamage(IAttacker attacker)
        {
            if (_conditionFunc != null && !_conditionFunc(attacker)) 
                return false;
            
            _receiveDamageSubject.OnNext(attacker);
            return true;

        }
    }
}