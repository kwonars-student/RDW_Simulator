using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RotationResetter : Resetter
{
    public static int testInt = 1;
    //protected float rotationSpeed = 60.0f;
    protected float targetAngle;
    protected float ratio;
    private Vector2 realTargetRotation, virtualTargetRotation;

    public RotationResetter() : base() {
    }

    public override string ApplyReset(Object2D realUser, Object2D virtualUser, Space2D realSpace, string resetType)
    {
        if (isFirst)
        {
            realTargetRotation = Matrix3x3.CreateRotation(targetAngle) * realUser.transform.forward;
            virtualTargetRotation = Matrix3x3.CreateRotation(ratio * targetAngle) * virtualUser.transform.forward;
            isFirst = false;
        }

        float realAngle = Vector2.SignedAngle(realUser.transform.forward, realTargetRotation);

        if (Mathf.Abs(realAngle) >= epsilonRotation)
        {
            realUser.Rotate(rotationSpeed * Time.deltaTime);
            virtualUser.Rotate(ratio * rotationSpeed * Time.deltaTime);
        }
        else
        {
            Utility.SyncDirection(virtualUser, realUser, virtualTargetRotation, realTargetRotation);

            switch (resetType)
            {
                case "Wall":
                    while (NeedWallReset(realUser, realSpace))
                    {
                        CalculationErrorAdjustment(realUser.transform, resetType, (Polygon2D)realSpace.space);
                    }
                    break;
                case "User":
                    break;
            }
            
            if (realUser.gameObject != null) realUser.gameObject.transform.position = Utility.Cast2Dto3D(realUser.transform.position);

            isFirst = true;
            return "DONE";
        }

        return "NOT_YET";
    }
}
