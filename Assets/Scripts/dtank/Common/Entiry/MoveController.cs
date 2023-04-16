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

        public Vector3 Position => _rigidbody.position;
        public Vector3 Forward => _rigidbody.rotation * Vector3.forward;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MoveController(Rigidbody rigidbody, float moveMaxSpeed, float turnMaxSpeed)
        {
            _rigidbody = rigidbody;
            _moveMaxSpeed = moveMaxSpeed;
            _turnMaxSpeed = turnMaxSpeed;
        }

        /// <summary>
        /// 廃棄処理
        /// </summary>
        public void Dispose()
        {
            _rigidbody = null;
        }

        public void SetTransform(TransformData data)
        {
            _rigidbody.transform.Set(data);
            _rigidbody.position = data.Position;
            _rigidbody.rotation = Quaternion.Euler(data.Angle);
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
        }

        private void Turn(float deltaTime)
        {
            var turn = _turnAmount * _turnMaxSpeed * deltaTime;
            _rigidbody.angularVelocity = new Vector3(0f, turn, 0f);
        }
    }
}