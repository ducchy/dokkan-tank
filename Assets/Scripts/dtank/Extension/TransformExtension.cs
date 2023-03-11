using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace dtank
{
    public static class TransformExtension
    {
        public static void Set(this Transform @this, TransformData data)
        {
            @this.position = data.Position;
            @this.eulerAngles = data.Angle;
            @this.localScale = data.LocalScale;
        }
    }
}