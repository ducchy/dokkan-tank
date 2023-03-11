using System;
using UnityEngine;

namespace dtank
{
    public class BattlePlayingController : IDisposable
    {
        private readonly BattleCamera _camera;
        
        public BattlePlayingController(BattleCamera camera)
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
            _camera.SetPosition(new Vector3(0f, 0f, 0f));
            _camera.SetEulerAngle(new Vector3(0f, 0f, 0f));
        }
    }
}