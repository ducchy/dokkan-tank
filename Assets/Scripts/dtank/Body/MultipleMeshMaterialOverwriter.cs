using System;
using UnityEngine;

namespace dtank
{
    public class MultipleMeshMaterialOverwriter : MonoBehaviour
    {
        [Serializable]
        private class MeshMaterialIndex
        {
            [SerializeField] private MeshRenderer _meshRenderer;
            [SerializeField] private int _index;

            public MeshRenderer MeshRenderer => _meshRenderer;
            public int Index => _index;
        }

        [SerializeField] private Material _materialPrefab;
        [SerializeField] private MeshMaterialIndex[] _targets;

        private Material _material;

        public void Setup(Color color)
        {
            _material = Instantiate(_materialPrefab);
            foreach (var target in _targets)
            {
                var materials = target.MeshRenderer.materials;
                var index = target.Index;
                if (index < 0 || materials.Length <= index)
                    continue;
                
                materials[index] = _material;
                target.MeshRenderer.materials = materials;
            }
            
            SetMaterialColor(color);
        }

        private void SetMaterialColor(Color color)
        {
            _material.color = color;
        }
    }
}