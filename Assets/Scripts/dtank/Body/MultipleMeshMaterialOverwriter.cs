using System;
using UnityEngine;

namespace dtank
{
    public class MultipleMeshMaterialOverwriter : MonoBehaviour
    {
        [Serializable]
        private class MeshMaterialIndex
        {
            public MeshRenderer meshRenderer;
            public int index;
        }

        [SerializeField] private Material _materialPrefab;
        [SerializeField] private MeshMaterialIndex[] _targets;

        private Material _material;

        public void Setup(Color color)
        {
            _material = Instantiate(_materialPrefab);
            foreach (var target in _targets)
            {
                var materials = target.meshRenderer.materials;
                var index = target.index;
                if (index < 0 || materials.Length <= index)
                    continue;
                
                materials[index] = _material;
                target.meshRenderer.materials = materials;
            }
            
            SetMaterialColor(color);
        }

        private void SetMaterialColor(Color color)
        {
            _material.color = color;
        }
    }
}