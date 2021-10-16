using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrightnessDetector : MonoBehaviour
{
    public PlayerData data;
    public RenderTexture renderTexture;
    public enum LuminanceCalculationMethod
    {
        Method1,
        Method2,
        Slower,
        RGBPercentage
    }
    public LuminanceCalculationMethod method;

    private void Update() {
        Color[] colors = RenderTextureToTexture2D(renderTexture).GetPixels();
        data.SetPlayerBrightness(GetMediumBrightness(colors));
    }

    private float GetMediumBrightness(Color[] colors) {
        float total = 0f;
        foreach (Color color in colors)
        {
            total += GetColorBrightness(color);
        }
        return total / colors.Length;
    }
    private float GetColorBrightness(Color color) {
        switch (method)
        {
            case LuminanceCalculationMethod.Method1:
                return 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;
            case LuminanceCalculationMethod.Method2:
                return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
            case LuminanceCalculationMethod.Slower:
                return Mathf.Sqrt(0.299f * color.r * color.r + 0.587f * color.g * color.g + 0.114f * color.b * color.b);
            case LuminanceCalculationMethod.RGBPercentage:
                return (color.r + color.g + color.b) / 3f;
            default: return 0;
        }
    }
    Texture2D RenderTextureToTexture2D(RenderTexture rTex) {
        Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}
