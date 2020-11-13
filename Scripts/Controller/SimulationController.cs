using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationController
{
    RedirectedUser redirectedUser;
    Episode episode;
    float rotationSpeed = 60.0f;
    float translationSpeed = 4.0f;

    [HideInInspector]
    public float deltaRotation;
    [HideInInspector]
    public Vector3 deltaPosition;

    private Vector3 previousPosition, previousForward;

    public SimulationController() {
        this.episode = new Episode();
        redirectedUser = null;

        ResetCurrentState();
    }

    public SimulationController(RedirectedUser redirectedUser, Episode episode, Space2D virtualSpace, GameObject targetPrefab) {
        this.episode = episode;
        this.episode.SetReferences(this, virtualSpace, targetPrefab);
        this.redirectedUser = redirectedUser;

        ResetCurrentState();
    }

    public int getEpisodeID()
    {
        return episode.getID();
    }

    public Transform GetUserVirtualTransform() {
        return redirectedUser.GetVirtualTransform();
    }

    public void UpdateCurrentState() {
        deltaPosition = (redirectedUser.GetVirtualTransform().localPosition - previousPosition) / Time.deltaTime;
        deltaRotation = Vector3.SignedAngle(previousForward, redirectedUser.GetVirtualTransform().forward, Vector3.up) / Time.deltaTime;
        previousPosition = redirectedUser.GetVirtualTransform().localPosition;
        previousForward = redirectedUser.GetVirtualTransform().forward;
    }

    public void ResetCurrentState() {
        deltaPosition = Vector3.zero;
        deltaRotation = 0;
        previousPosition = redirectedUser.GetVirtualTransform().localPosition;
        previousForward = redirectedUser.GetVirtualTransform().forward;
    }

    public void VirtualMove() {
        if (episode.IsNotEnd()) {
            Vector3 targetPosition = episode.GetTarget();
            Vector3 directionToTarget = targetPosition - redirectedUser.GetVirtualTransform().localPosition;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            float distance = directionToTarget.magnitude;
            float angle = Quaternion.Angle(redirectedUser.GetVirtualTransform().localRotation, targetRotation);

            if (angle > 0.1f) {
                redirectedUser.GetVirtualTransform().localRotation = Quaternion.RotateTowards(redirectedUser.GetVirtualTransform().localRotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
            else if (distance > 0.1f) {
                redirectedUser.GetVirtualTransform().Translate(redirectedUser.GetVirtualTransform().forward * translationSpeed * Time.deltaTime, Space.World);
            }
            else {
                episode.DeleteTarget();
            }
        }

        UpdateCurrentState();
    }

    public (Redirector.GainType, float) RealMove() {
        (Redirector.GainType type, float degree) = redirectedUser.GetRedirector().ApplyRedirection(deltaPosition, deltaRotation);
        float appliedGain = 0;

        switch (type) {
            case Redirector.GainType.Translation:
                appliedGain = Math.Abs(redirectedUser.GetRedirector().GetTranslationGain());
                redirectedUser.GetRealTransform().Translate(redirectedUser.GetRealTransform().forward * degree * Time.deltaTime, Space.World);
                break;
            case Redirector.GainType.Rotation:
                appliedGain = Math.Abs(redirectedUser.GetRedirector().GetRotationGain());
                redirectedUser.GetRealTransform().Rotate(Vector3.up, degree * Time.deltaTime);
                break;
            case Redirector.GainType.Curvature:
                appliedGain = Math.Abs(redirectedUser.GetRedirector().GetCurvatureGain());
                redirectedUser.GetRealTransform().Translate(redirectedUser.GetRealTransform().forward * deltaPosition.magnitude * Time.deltaTime, Space.World);
                redirectedUser.GetRealTransform().Rotate(Vector3.up, degree * Time.deltaTime);
                break;
            default:
                break;
        }

        return (type, appliedGain);
    }

    public bool isNotEnd()
    {
        return episode.IsNotEnd();
    }
}
