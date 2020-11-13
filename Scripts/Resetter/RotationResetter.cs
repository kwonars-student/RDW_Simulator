using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RotationResetter : Resetter
{
    protected float rotationSpeed = 60.0f;
    protected float targetAngle;
    protected float ratio;
    private Vector2 realTargetRotation, virtualTargetRotation;

    public RotationResetter() : base() {}

    //public RotationResetter(Space2D space) : base(space) { }

    public override void ApplyReset(Object2D realUser, Object2D virtualUser)
    {
        //Object2D realUser = redirectedUnit.GetRealUser();
        //Object2D virtualUser = redirectedUnit.GetVirtualUser();

        if (isFirst)
        {
            realTargetRotation = Matrix3x3.CreateRotation(targetAngle) * realUser.transform.forward;
            virtualTargetRotation = Matrix3x3.CreateRotation(ratio * targetAngle) * virtualUser.transform.forward;
            isFirst = false;
        }

        realUser.Rotate(rotationSpeed * Time.deltaTime);
        virtualUser.Rotate(ratio * rotationSpeed * Time.deltaTime);

        float realAngle = Vector2.SignedAngle(realUser.transform.forward, realTargetRotation);

        if (realAngle < 0.1f)
        {
            virtualUser.transform.forward = virtualTargetRotation;
            realUser.transform.forward = realTargetRotation;
            realUser.transform.position = realUser.transform.position + realUser.transform.forward * 0.1f;
            isFirst = true;
        }
    }
}
