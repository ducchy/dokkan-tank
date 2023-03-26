using System;
using UnityEngine;

namespace dtank
{
    /// <summary>
    /// 移動制御用コントローラ
    /// </summary>
    public class MoveController : IDisposable
    {
        private Rigidbody _rigidbody;

        private readonly float _moveMaxSpeed;
        private readonly float _turnMaxSpeed;

        private float _moveAmount;
        private float _turnAmount;

        private Action<Vector3> _updatePosition;
        private Action<Vector3> _updateForward;
        
        private Vector3 Forward => _rigidbody.rotation * Vector3.forward;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MoveController(Rigidbody rigidbody, float moveMaxSpeed, float turnMaxSpeed, 
            Action<Vector3> updatePosition, Action<Vector3> updateForward)
        {
            _rigidbody = rigidbody;
            _moveMaxSpeed = moveMaxSpeed;
            _turnMaxSpeed = turnMaxSpeed;
            _updatePosition = updatePosition;
            _updateForward = updateForward;
        }

        /// <summary>
        /// 廃棄処理
        /// </summary>
        public void Dispose()
        {
            _rigidbody = null;
            _updatePosition = null;
            _updateForward = null;
        }

        public void SetTransform(TransformData data)
        {
            _rigidbody.transform.Set(data);
            _rigidbody.position = data.Position;
            _rigidbody.rotation = Quaternion.Euler(data.Angle);

            OnUpdatePosition();
            OnUpdateForward();
        }

        public void SetMoveAmount(float moveAmount)
        {
            _moveAmount = moveAmount;
        }

        public void SetTurnAmount(float turnAmount)
        {
            _turnAmount = turnAmount;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public void Update(float deltaTime)
        {
            Move(deltaTime);
            Turn(deltaTime);
        }

        private void Move(float deltaTime)
        {
            var movement = Forward * (_moveAmount * _moveMaxSpeed * deltaTime);
            _rigidbody.velocity = movement;

            OnUpdatePosition();
        }

        private void Turn(float deltaTime)
        {
            var turn = _turnAmount * _turnMaxSpeed * deltaTime;
            _rigidbody.angularVelocity = new Vector3(0f, turn, 0f);

            OnUpdateForward();
        }

        private void OnUpdatePosition()
        {
            _updatePosition?.Invoke(_rigidbody.position);
        }

        private void OnUpdateForward()
        {
            _updateForward?.Invoke(Forward);
        }
    }
}