using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S2CRedirector : SteerToTargetRedirector
{
    private const float S2C_BEARING_ANGLE_THRESHOLD_IN_DEGREE = 160;
    private const float S2C_TEMP_TARGET_DISTANCE = 4;

    public override void PickSteeringTarget(List<Vector2> vertices) {

        float n = vertices.Count;
        Vector2 averageVector = Vector2.zero;
        for(int i = 0 ; i < vertices.Count ; i++)
        {
            averageVector += vertices[i];
        }
        averageVector = averageVector/n;

        // 우선적으로 Center는 vertex 벡터들의 평균으로 구현되었으나, 여러 Convex의 경우 적당하지 못할 수 있으므로 이후 S2C에서 Center를 어떻게 정할 것인지에 대한 논의 필요.
        Vector2 trackingAreaPosition = averageVector; // center must be zero in local space.
        Vector2 userToCenter = trackingAreaPosition - userPosition;

        //Compute steering target for S2C
        float bearingToCenter = Vector2.Angle(userDirection, userToCenter);
        float directionToCenter = Mathf.Sign(Vector2.SignedAngle(userDirection, userToCenter)); // if target is to the left of the user, directionToTarget > 0

        if (bearingToCenter >= S2C_BEARING_ANGLE_THRESHOLD_IN_DEGREE) { 
            targetPosition = userPosition + S2C_TEMP_TARGET_DISTANCE * Utility.RotateVector2(userDirection, directionToCenter * 90);
        }
        else {
            targetPosition = trackingAreaPosition;
        }
    }
}
