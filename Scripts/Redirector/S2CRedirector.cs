using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S2CRedirector : SteerToTargetRedirector
{
    private const float S2C_BEARING_ANGLE_THRESHOLD_IN_DEGREE = 160;
    private const float S2C_TEMP_TARGET_DISTANCE = 4;

    public override void PickSteeringTarget() {
        Vector3 trackingAreaPosition = Vector3.zero; // center must be zero in local space (정사각형인경우)
        Vector3 userToCenter = trackingAreaPosition - userPosition;

        //Compute steering target for S2C
        float bearingToCenter = Vector3.Angle(userDirection, userToCenter);
        float directionToCenter = Mathf.Sign(Vector3.SignedAngle(userDirection, userToCenter, Vector3.up));

        if (bearingToCenter >= S2C_BEARING_ANGLE_THRESHOLD_IN_DEGREE) {
            targetPosition = userPosition + S2C_TEMP_TARGET_DISTANCE * (Quaternion.Euler(0, directionToCenter * 90, 0) * userDirection);
        }
        else {
            targetPosition = trackingAreaPosition;
        }

        //if (bearingToCenter >= S2C_BEARING_ANGLE_THRESHOLD_IN_DEGREE && !dontUseTempTargetInS2C) {
        //    //Generate temporary target
        //    if (noTmpTarget) {
        //        tmpTarget = new GameObject("S2C Temp Target");
        //        tmpTarget.transform.localPosition = userPosition + S2C_TEMP_TARGET_DISTANCE * (Quaternion.Euler(0, directionToCenter * 90, 0) * userDirection);
        //        tmpTarget.transform.parent = GameObject.Find("Real Space").transform;
        //        noTmpTarget = false;
        //    }

        //    targetPosition = tmpTarget.transform.localPosition;
        //}
        //else {
        //    targetPosition = trackingAreaPosition;
        //    if (!noTmpTarget) {
        //        GameObject.Destroy(tmpTarget);
        //        noTmpTarget = true;
        //    }
        //}
    }
}
