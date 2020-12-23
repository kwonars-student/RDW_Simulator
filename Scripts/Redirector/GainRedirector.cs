using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class GainRedirector : Redirector
{
    [HideInInspector]
    public const float MIN_ROTATION_GAIN = -0.2f;
    [HideInInspector]
    public const float MAX_ROTATION_GAIN = 0.49f;
    [HideInInspector]
    public const float MIN_CURVATURE_GAIN = -0.045f; // turn radius : 22m
    [HideInInspector]
    public const float MAX_CURVATURE_GAIN = 0.045f;
    [HideInInspector]
    public const float HODGSON_MIN_CURVATURE_GAIN = -0.133f; // turn radius : 7.5m
    [HideInInspector]
    public const float HODGSON_MAX_CURVATURE_GAIN = 0.133f;
    [HideInInspector]
    public const float MIN_TRANSLATION_GAIN = -0.14f;
    [HideInInspector]
    public const float MAX_TRANSLATION_GAIN = 0.26f;

    protected float translationGain;
    protected float rotationGain;
    protected float curvatureGain;

    public float GetApplidedGain(GainType type) // TODO: 얘를 추상화 할순 없나?
    {
        switch (type)
        {
            case GainType.Translation:
                return Mathf.Abs(GetTranslationGain());
            case GainType.Rotation:
                return Mathf.Abs(GetRotationGain());
            case GainType.Curvature:
                return Mathf.Abs(GetCurvatureGain());
            default:
                return 0;
        }
    }

    public float GetTranslationGain()
    {
        return translationGain;
    }

    public float GetRotationGain()
    {
        return rotationGain;
    }

    public float GetCurvatureGain()
    {
        return curvatureGain;
    }
}
