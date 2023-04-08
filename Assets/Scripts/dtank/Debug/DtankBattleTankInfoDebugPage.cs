using System.Collections.Generic;
using System.Reflection;
using UnityDebugSheet.Runtime.Extensions.Unity;
using UnityEngine;

namespace dtank
{
    public class DtankBattleTankInfoDebugPageInfo
    {
        private BattleTankModel _model;

        public void SetModel(BattleTankModel model)
        {
            if (_model == model)
                return;

            _model = model;

            if (model == null)
                Reset();
            else
                Update();
        }

        public void Update()
        {
            if (_model == null)
                return;

            Id = _model.Id;
            Name = _model.Name;
            Color = _model.ActorModel.SetupData.Color;
            AssetKey = _model.AssetKey;
            BodyId = _model.BodyId;
            CharacterType = _model.CharacterType;
            CurrentState = _model.CurrentState.Value;
            Hp = _model.Hp.Value;
            Score = _model.Score.Value;
            Rank = _model.Rank.Value;
            InvincibleFlag = _model.InvincibleFlag.Value;
            DeadFlag = _model.DeadFlag.Value;
            MovableFlag = _model.MovableFlag;
            MoveAmount = _model.MoveAmount.Value;
            TurnAmount = _model.TurnAmount.Value;
            Position = _model.Position;
            Forward = _model.Forward;
        }

        private void Reset()
        {
            Id = default;
            Name = "未設定";
            Color = default;
            AssetKey = default;
            BodyId = default;
            CharacterType = default;
            CurrentState = default;
            Hp = default;
            Score = default;
            Rank = default;
            InvincibleFlag = default;
            DeadFlag = default;
            MovableFlag = default;
            MoveAmount = default;
            TurnAmount = default;
            Position = default;
            Forward = default;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public Color Color { get; private set; }
        public string AssetKey { get; private set; }
        public int BodyId { get; private set; }
        public CharacterType CharacterType { get; private set; }
        public BattleTankState CurrentState { get; private set; }
        public int Hp { get; private set; }
        public int Score { get; private set; }
        public int Rank { get; private set; }
        public bool InvincibleFlag { get; private set; }
        public bool DeadFlag { get; private set; }
        public bool MovableFlag { get; private set; }
        public float MoveAmount { get; private set; }
        public float TurnAmount { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Forward { get; private set; }
    }

    public sealed class DtankBattleTankInfoDebugPage : PropertyListDebugPageBase<DtankBattleTankInfoDebugPageInfo>
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

        private readonly DtankBattleTankInfoDebugPageInfo _targetObject = new();

        public void SetModel(BattleTankModel model)
        {
            _targetObject.SetModel(model);
        }

        protected override void Update()
        {
            _targetObject.Update();

            base.Update();
        }
    }
}