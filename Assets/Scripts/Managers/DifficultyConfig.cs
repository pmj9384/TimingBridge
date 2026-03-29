using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyConfig", menuName = "TimingBridge/DifficultyConfig")]
public class DifficultyConfig : ScriptableObject
{
    [Header("Bridge Grow Speed")]
    public float baseGrowSpeed = 5f;
    public float maxGrowSpeed = 12f;
    public float growSpeedPerScore = 0.4f;

    [Header("Platform Size")]
    public float basePlatformZScale = 1f;
    public float minPlatformZScale = 0.2f;
    public float platformScaleDecreasePerScore = 0.04f;

    public float GetGrowSpeed(int score)
    {
        return Mathf.Min(maxGrowSpeed, baseGrowSpeed + score * growSpeedPerScore);
    }

    public float GetPlatformZScale(int score)
    {
        return Mathf.Max(minPlatformZScale, basePlatformZScale - score * platformScaleDecreasePerScore);
    }
}
