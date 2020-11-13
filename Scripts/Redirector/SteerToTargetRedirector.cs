using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteerToTargetRedirector : Redirector
{
    private const float DISTANCE_THRESHOLD_FOR_DAMPENING = 1.25f; // Distance threshold to apply dampening (meters)
    private const float ANGLE_THRESHOLD_FOR_DAMPENING = 45f; // Angle threshold to apply dampening (degrees)
    private const float SMOOTHING_FACTOR = 0.125f; // Smoothing factor for redirection rotations
    private const float MOVEMENT_THRESHOLD = 0.2f; // meters per second
    private const float ROTATION_THRESHOLD = 1.5f; // degrees per second

    private float previousMagnitude = 0f;

    protected Vector3 userPosition; // user localPosition
    protected Vector3 userDirection; // user local direction (localforward)
    protected Vector3 targetPosition; // steerting target localPosition

    public virtual void PickSteeringTarget() {}

    public override (GainType, float) ApplyRedirection(Vector3 deltaPosition, float deltaRotation) {
        // define some variables for redirection
        Transform realUserTransform = redirectedUnit.GetRealTransform();
        userPosition = realUserTransform.localPosition;
        userDirection = realUserTransform.forward;

        // pick a target to where user steer
        PickSteeringTarget();

        Vector3 userToTarget = targetPosition - userPosition;
        float angleToTarget = Vector3.Angle(userDirection, userToTarget);
        float distanceToTarget = userToTarget.magnitude;

        // control applied gains according to user and target
        float directionToTarget = Mathf.Sign(Vector3.SignedAngle(userDirection, userToTarget, Vector3.up)); // if target is to the right of the user, directionToTarget > 0
        float directionRotation = Mathf.Sign(deltaRotation); // If user is rotating to the right, directionRotation > 0

        if (directionToTarget < 0)  // If the target is to the left of the user,
            curvatureGain = HODGSON_MIN_CURVATURE_GAIN;
        else
            curvatureGain = HODGSON_MAX_CURVATURE_GAIN;

        if (directionToTarget * directionRotation < 0) // if user rotates away from the target (if their direction are opposite),
            rotationGain = MIN_ROTATION_GAIN;
        else
            rotationGain = MAX_ROTATION_GAIN;

        // select the largest magnitude
        float rotationMagnitude = 0, curvatureMagnitude = 0; // actually, not ABS(절대값)

        if (Mathf.Abs(deltaRotation) >= ROTATION_THRESHOLD)
            rotationMagnitude = rotationGain * deltaRotation;
        if (deltaPosition.magnitude > MOVEMENT_THRESHOLD)
            curvatureMagnitude = Mathf.Rad2Deg * curvatureGain * deltaPosition.magnitude; 

        float selectedMagnitude = Mathf.Max(Mathf.Abs(rotationMagnitude), Mathf.Abs(curvatureMagnitude)); // selectedMagnitude is ABS(절대값)
        bool isCurvatureSelected = Mathf.Abs(curvatureMagnitude) > Mathf.Abs(rotationMagnitude);

        // dampening 
        if (angleToTarget <= ANGLE_THRESHOLD_FOR_DAMPENING)
            selectedMagnitude *= Mathf.Sin(Mathf.Deg2Rad * 90 * angleToTarget / ANGLE_THRESHOLD_FOR_DAMPENING);
        if (distanceToTarget <= DISTANCE_THRESHOLD_FOR_DAMPENING) {
            selectedMagnitude *= distanceToTarget / DISTANCE_THRESHOLD_FOR_DAMPENING;
        }

        // smoothing
        float finalRotation = (1.0f - SMOOTHING_FACTOR) * previousMagnitude + SMOOTHING_FACTOR * selectedMagnitude;
        previousMagnitude = finalRotation;

        // apply final redirection
        if (!isCurvatureSelected) {
            float direction = directionRotation; //float direction = -directionRotation * Mathf.Sign(rotationGain);

            return (GainType.Rotation, finalRotation * direction);
        }
        else {
            float direction = Mathf.Sign(curvatureGain);

            return (GainType.Curvature, finalRotation * direction);
        }
    }
}
