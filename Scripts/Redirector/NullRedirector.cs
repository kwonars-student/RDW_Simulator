using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullRedirector : Redirector
{
    public override (GainType, float) ApplyRedirection(Object2D realUser, Vector2 deltaPosition, float deltaRotation) {
        float degree = 0;
        GainType type = GainType.Undefined;

        if (deltaPosition.magnitude > 0.1f) {
            degree = deltaPosition.magnitude;
            type = GainType.Translation;
        }
        else if (Mathf.Abs(deltaRotation) > 0.1f) {
            degree = deltaRotation;
            type = GainType.Rotation;
        }
        else {
            type = GainType.Undefined;
        }

        return (type, degree);
    }
}
