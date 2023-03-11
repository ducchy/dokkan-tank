using System;
using UnityEngine;

namespace dtank
{
    public class BattleResultController : IDisposable
    {
        private readonly BattleCamera _camera;
        
        public BattleResultController(BattleCamera camera)
        {
            _camera = camera;
        }

        public void Dispose()
        {
        }

        public void Activate()
        {
            SetCamera();
        }

        public void Deactivate()
        {
        }

        public void SetCamera()
        {
            var tankPos = new Vector3(-2f, 1f, 2f);
            var tankAngle = new Vector3(0f, 45f, 0f);
            var cameraOffset = Vector3.zero;
            
            _camera.SetPosition(tankPos + cameraOffset);
            _camera.SetEulerAngle(tankAngle);
        }
    }
}