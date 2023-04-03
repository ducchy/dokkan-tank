using System;
using UnityEngine;

namespace dtank
{
    /// <summary>
    /// https://kazupon.org/unity-color-herper/
    /// </summary>
    public static class ColorHelper
    {
        /// <summary>
        /// 指定した色の補色を取得します(RGB色相環)
        /// </summary>
        /// <param name="baseColor">ベースの色</param>
        /// <returns>補色</returns>
        public static Color GetComplementaryColor(Color baseColor)
        {
            Color.RGBToHSV(baseColor, out var hue, out var saturation, out var value);
            hue += 0.5f;
            if (hue > 1.0f)
                hue -= 1.0f;

            return Color.HSVToRGB(hue, saturation, value);
        }

        /// <summary>
        /// 指定した色の類似色を取得します(RGB色相環)
        /// </summary>
        /// <param name="baseColor">ベースの色</param>
        /// <param name="offsetHue">色相オフセット (-3~+3)</param> 
        /// <returns>類似色</returns>
        public static Color GetAnalogousColor(Color baseColor, float offsetHue)
        {
            Color.RGBToHSV(baseColor, out var hue, out var saturation, out var value);
            var offsetDeg = (360.0f / 24.0f) * offsetHue;
            hue += offsetDeg / 360;
            if (hue > 1.0f)
                hue -= 1.0f;
            else if (hue < 0.0f)
                hue += 1.0f;

            var outColor = Color.HSVToRGB(hue, saturation, value);
            return outColor;
        }

        /// <summary>
        /// 指定した色の明度を変更します(RGB色相環)
        /// </summary>
        /// <param name="baseColor">ベースの色</param>
        /// <param name="brightness">明度</param> 
        /// <returns>変更後の色</returns>
        public static Color SetBrightNess(Color baseColor, float brightness)
        {
            Color.RGBToHSV(baseColor, out var hue, out var saturation, out _);
            return Color.HSVToRGB(hue, saturation, brightness);
        }

        /// <summary>
        /// 指定した色の明度を加算します(RGB色相環)
        /// </summary>
        /// <param name="baseColor">ベースの色</param>
        /// <param name="brightness">明度の加算値</param> 
        /// <returns>変更後の色</returns>
        public static Color AddBrightNess(Color baseColor, float brightness)
        {
            Color.RGBToHSV(baseColor, out var hue, out var saturation, out var value);
            value = Mathf.Min(Mathf.Max(value + brightness, 0), 1);
            var outColor = Color.HSVToRGB(hue, saturation, value);
            return outColor;
        }

        /// <summary>
        /// 指定した色の彩度を変更します(RGB色相環)
        /// </summary>
        /// <param name="baseColor">ベースの色</param>
        /// <param name="inSaturation">彩度の加算値</param> 
        /// <returns>変更後の色</returns>
        public static Color SetSaturation(Color baseColor, float inSaturation)
        {
            Color.RGBToHSV(baseColor, out var hue, out _, out var value);
            return Color.HSVToRGB(hue, inSaturation, value);
        }

        /// <summary>
        /// 指定した色の彩度を加算します(RGB色相環)
        /// </summary>
        /// <param name="baseColor">ベースの色</param>
        /// <param name="inSaturation">彩度の加算値</param> 
        /// <returns>変更後の色</returns>
        public static Color AddSaturation(Color baseColor, float inSaturation)
        {
            Color.RGBToHSV(baseColor, out var hue, out var saturation, out var value);
            saturation = Math.Min(Math.Max(saturation + inSaturation, 0), 1);
            return Color.HSVToRGB(hue, saturation, value);
        }

        public static float GetValue(Color baseColor)
        {
            return baseColor.r * 0.22f + baseColor.g * 0.72f + baseColor.b * 0.06f;
        }

        public static Color SetValue(Color baseColor, float value)
        {
            var baseValue = GetValue(baseColor);
            return baseColor * (value / baseValue);
        }
    }
}