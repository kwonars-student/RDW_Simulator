using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationController
{
    Episode episode;
    float rotationSpeed = 60.0f;
    float translationSpeed = 4.0f;
    float maxRotTime, maxTransTime, remainRotTime, remainTransTime;
    bool isFirst = true, initializing = false, isFirst2 = true;
    float signAngle = 0.0f;
    Vector2 directionToTarget = Vector2.zero;

    [HideInInspector]
    public float deltaRotation;
    [HideInInspector]
    public Vector2 deltaPosition;

    private Vector2 previousPosition, previousForward;

    public SimulationController() {
        this.episode = new Episode();
    }

    public SimulationController(Episode episode)
    {
        this.episode = episode;
    }

    public void UpdateCurrentState(Transform2D virtualUserTransform) {
        deltaPosition = (virtualUserTransform.localPosition - previousPosition) / Time.deltaTime;
        deltaRotation = Vector2.SignedAngle(previousForward, virtualUserTransform.forward) / Time.deltaTime;
        previousPosition = virtualUserTransform.localPosition;
        previousForward = virtualUserTransform.forward;

    }

    public void ResetCurrentState(Transform2D virtualUserTransform) {
        deltaPosition = Vector2.zero;
        deltaRotation = 0;
        previousPosition = virtualUserTransform.localPosition;
        previousForward = virtualUserTransform.forward;
    }

    public (Vector2, float) GetDelta()
    {
        return (deltaPosition, deltaRotation);
    }

    public (Vector2, float) VirtualMove(Object2D virtualUser, Space2D virtualSpace) {
        Transform2D virtualUserTransform = virtualUser.transform;

        if(!initializing)
        {
            ResetCurrentState(virtualUserTransform);
            initializing = false;
        }

        if (episode.IsNotEnd()) {
            if (isFirst)
            {
                isFirst = false;

                Vector2 targetPosition = episode.GetTarget(virtualUserTransform, virtualSpace);
                directionToTarget = targetPosition - virtualUserTransform.localPosition;
                float distance = directionToTarget.magnitude;
                float angle = Vector2.Angle(virtualUserTransform.forward, directionToTarget);
                signAngle = Mathf.Sign(Vector2.SignedAngle(virtualUserTransform.forward, directionToTarget));

                //Debug.Log(string.Format("targetPosition: {0}", targetPosition));
                //Debug.Log(string.Format("virtualUserTransform.localPosition: {0}", virtualUserTransform.localPosition));

                maxRotTime = angle / rotationSpeed;
                maxTransTime = distance / translationSpeed;
                remainRotTime = 0.0f;
                remainTransTime = 0.0f;
            }

            //Debug.Log("------ Start ------");
            //Debug.Log(string.Format("virtualUserTransform.forward: {0}", virtualUserTransform.forward));
            //Debug.Log(string.Format("directionToTarget: {0}", directionToTarget));
            //Debug.Log(string.Format("signAngle: {0}", signAngle));
            //Debug.Log("------ End ------");

            if (remainRotTime < maxRotTime)
            {
                virtualUser.Rotate(signAngle * rotationSpeed * Time.deltaTime);
                remainRotTime += Time.deltaTime;
            }
            else if (remainTransTime < maxTransTime)
            {
                if(isFirst2 && directionToTarget != Vector2.zero)
                {
                    virtualUserTransform.forward = directionToTarget;
                    isFirst2 = false;
                }

                virtualUser.Translate(virtualUserTransform.forward * translationSpeed * Time.deltaTime, Space.World);
                remainTransTime += Time.deltaTime;
            }
            else
            {
                episode.DeleteTarget();
                isFirst = true;
                isFirst2 = true;
            }
        }

        UpdateCurrentState(virtualUserTransform);

        return GetDelta();
    }

    public (Redirector.GainType, float) RealMove(Object2D realUser, Redirector.GainType type, float degree) {
        Transform2D realUserTransform = realUser.transform;

        //(Redirector.GainType type, float degree) = redirectedUnit.GetRedirector().ApplyRedirection(realUser, deltaPosition, deltaRotation);
        float appliedGain = 0;

        switch (type) {
            case Redirector.GainType.Translation:
                //appliedGain = Math.Abs(redirectedUnit.GetRedirector().GetTranslationGain());
                realUser.Translate(realUserTransform.forward * degree * Time.deltaTime, Space.World);
                break;
            case Redirector.GainType.Rotation:
                //appliedGain = Math.Abs(redirectedUnit.GetRedirector().GetRotationGain());
                realUser.Rotate(degree * Time.deltaTime);
                break;
            case Redirector.GainType.Curvature:
                //appliedGain = Math.Abs(redirectedUnit.GetRedirector().GetCurvatureGain());
                realUser.Translate(realUserTransform.forward * deltaPosition.magnitude * Time.deltaTime, Space.World);
                realUser.Rotate(degree * Time.deltaTime);
                break;
            default:
                break;
        }

        return (type, appliedGain);
    }

    public int GetEpisodeID()
    {
        return episode.getID();
    }

    public Episode GetEpisode()
    {
        return episode;
    }

}
