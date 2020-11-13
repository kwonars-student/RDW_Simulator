using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Redirector
{
    public enum GainType {Translation = 0, Rotation = 1, Curvature = 2, Undefined = -1};

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
    //protected RedirectedUnit redirectedUnit;

    public Redirector() {
        //this.redirectedUnit = null;
    }

    //public Redirector(RedirectedUnit redirectedUnit) {
    //    this.redirectedUnit = redirectedUnit;
    //}

    //public void SetReferences(RedirectedUnit redirectedUnit) {
    //    this.redirectedUnit = redirectedUnit;
    //}

    public virtual (GainType, float) ApplyRedirection(Object2D realUser, Vector2 deltaPosition, float deltaRotation) {
        float degree = 0;
        GainType type = GainType.Undefined;

        return (type, degree);
    }

    public float GetApplidedGain(GainType type)
    {
        switch (type)
        {
            case Redirector.GainType.Translation:
                return Mathf.Abs(GetTranslationGain());
            case Redirector.GainType.Rotation:
                return Mathf.Abs(GetRotationGain());
            case Redirector.GainType.Curvature:
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
