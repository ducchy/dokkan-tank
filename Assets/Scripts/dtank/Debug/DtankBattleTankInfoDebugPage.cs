using System.Collections.Generic;
using System.Reflection;
using UnityDebugSheet.Runtime.Extensions.Unity;

namespace dtank
{
    public sealed class DtankBattleTankInfoDebugPage : PropertyListDebugPageBase<DtankBattleTankInfoData>
    {
        protected override string Title => $"Tank Info > {_targetObject.Name}";

        public override BindingFlags BindingFlags => BindingFlags.Public | BindingFlags.Instance;
        public override List<string> UpdateTargetPropertyNames => _updateTargetPropertyNames;
        public override object TargetObject => _targetObject;

        private readonly List<string> _updateTargetPropertyNames = new()
        {
            "CurrentState", "Hp", "Score", "Rank", "InvincibleFlag", "DeadFlag", "MovableFlag", "MoveAmount",
            "TurnAmount", "Position", "Forward"
        };

        private readonly DtankBattleTankInfoData _targetObject = new();

        public void SetModel(BattleTankModel model)
        {
            _targetObject.SetModel(model);
        }

        protected override void OnDestroy()
        {
            _targetObject.Dispose();
            
            base.OnDestroy();
        }

        protected override void Update()
        {
            _targetObject.Update();

            base.Update();
        }
    }
}