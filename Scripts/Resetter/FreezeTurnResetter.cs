using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTurnResetter : Resetter
{
    float rotationSpeed = 60.0f;
    Transform realTransform, virtualTransform;
    Quaternion realTargetRotation = Quaternion.identity, virtualTargetRotation = Quaternion.identity;

    public FreezeTurnResetter() : base() {}

    public FreezeTurnResetter(RedirectedUser redirectedUser, Space2D space) : base(redirectedUser, space) {}

    public override void ApplyReset() {
        realTransform = redirectedUnit.GetRealTransform();
        virtualTransform = redirectedUnit.GetVirtualTransform();

        if (!isComplete) {
            if (isFirst) {
                realTargetRotation = realTransform.localRotation * Quaternion.AngleAxis(180, Vector3.up);
                virtualTargetRotation = virtualTransform.localRotation * Quaternion.AngleAxis(360, Vector3.up);
                isFirst = false;
            }

            realTransform.localRotation = Quaternion.RotateTowards(realTransform.localRotation, realTargetRotation, Time.deltaTime * rotationSpeed);
            virtualTransform.localRotation = Quaternion.RotateTowards(virtualTransform.localRotation, virtualTargetRotation, Time.deltaTime * rotationSpeed);

            float realAngle = Quaternion.Angle(realTransform.localRotation, realTargetRotation);

            if (realAngle < 0.1f) {
                isFirst = true;
                isComplete = true;
            }
        }
    }
}
