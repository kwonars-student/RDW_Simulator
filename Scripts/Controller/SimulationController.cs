using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationController
{
    Episode episode;
    float rotationSpeed;
    float translationSpeed;
    //float maxRotTime, maxTransTime, remainRotTime, remainTransTime;
    bool isFirst = true, initializing = false, isFirst2 = true, isFirst3 = true;
    float initialAngleDirection = 0.0f;
    float epsilonRotation;
    float epsilonTranslation;
    Vector2 initialToTarget;
    Vector2 targetPosition;
    Vector2 virtualTargetDirection = Vector2.zero;
    Vector2 realTargetDirection = Vector2.zero;
    Vector2 virtualTargetPosition;
    Vector2 realTargetPosition;
    float InitialAngle, initialDistance;

    [HideInInspector]
    public float deltaRotation;
    [HideInInspector]
    public Vector2 deltaPosition;

    private Vector2 previousPosition;


    private float previousRotation;

    public SimulationController() {
        this.episode = new Episode();
    }

    public SimulationController(Episode episode, float translationSpeed, float rotationSpeed) {
        this.episode = episode;
        this.translationSpeed = translationSpeed;
        this.rotationSpeed = rotationSpeed;
        epsilonRotation = (rotationSpeed * Time.fixedDeltaTime / 2) + 0.001f;
        epsilonTranslation = (translationSpeed * Time.fixedDeltaTime / 2) + 0.001f;
    }

    public void UpdateCurrentState(Transform2D virtualUserTransform) {
        deltaPosition = (virtualUserTransform.localPosition - previousPosition) / Time.fixedDeltaTime;
        deltaRotation = (virtualUserTransform.localRotation - previousRotation) / Time.fixedDeltaTime;
        previousPosition = virtualUserTransform.localPosition;
        previousRotation = virtualUserTransform.localRotation;
    }

    public void ResetCurrentState(Transform2D virtualUserTransform) {
        deltaPosition = Vector2.zero;
        deltaRotation = 0;
        previousPosition = virtualUserTransform.localPosition;
        previousRotation = virtualUserTransform.localRotation;
    }

    public (Vector2, float) GetDelta(Vector2 directionToTarget)
    {
        return (deltaPosition, deltaRotation);
        //Vector2 retTrans = Vector2.zero;
        //float retRot = 0;

        //if (deltaPosition.magnitude > (translationSpeed - 0.1f))
        //{
        //    retTrans = directionToTarget * translationSpeed;
        //}
        //else
        //{
        //    retTrans = deltaPosition;
        //}

        //if (Mathf.Abs(deltaRotation) > (rotationSpeed - 0.1f))
        //{
        //    retRot = Mathf.Sign(deltaRotation) * rotationSpeed;
        //}
        //else
        //{
        //    retRot = deltaRotation;
        //}

        //return (retTrans, retRot);
    }

    public void SyncDirection(Object2D virtualUser, Vector2 virtualTargetDirection)
    {
        if (virtualTargetDirection.magnitude > 1)
            virtualTargetDirection = virtualTargetDirection.normalized;

        virtualUser.transform.forward = virtualTargetDirection;
        if (virtualUser.gameObject != null) virtualUser.gameObject.transform.forward = Utility.Cast2Dto3D(virtualTargetDirection);
    }

    public void SyncPosition(Object2D virtualUser, Vector2 virtualTargetPosition)
    {
        virtualUser.transform.localPosition = virtualTargetPosition;
        if (virtualUser.gameObject != null) virtualUser.gameObject.transform.localPosition = Utility.Cast2Dto3D(virtualTargetPosition);
    }

    public (Vector2, float) VirtualMove(Object2D realUser, Object2D virtualUser, Space2D virtualSpace)
    {
        Transform2D virtualUserTransform = virtualUser.transform;

        if (!initializing)
        {
            ResetCurrentState(virtualUserTransform);
            initializing = false;
        }

        if (episode.IsNotEnd())
        {
            if (isFirst)
            {
                isFirst = false;
                targetPosition = episode.GetTarget(virtualUserTransform, virtualSpace);
                initialToTarget = targetPosition - virtualUserTransform.localPosition;
                float InitialAngle = Vector2.SignedAngle(virtualUserTransform.forward, initialToTarget);
                float initialDistance = Vector2.Distance(virtualUserTransform.localPosition, targetPosition);

                virtualTargetDirection = Matrix3x3.CreateRotation(InitialAngle) * virtualUser.transform.forward; // target을 향하는 direction(forward)를 구함
                //realTargetDirection = Matrix3x3.CreateRotation(InitialAngle) * realUser.transform.forward;

                virtualTargetPosition = virtualUser.transform.localPosition + virtualTargetDirection * initialDistance; // target에 도달하는 position을 구함
                //realTargetPosition = realUser.transform.localPosition + realTargetDirection * initialDistance;

                initialAngleDirection = Mathf.Sign(InitialAngle);
            }

            float distance = (targetPosition - virtualUserTransform.localPosition).magnitude;
            float angle = Vector2.SignedAngle(virtualUserTransform.forward, initialToTarget);

            if (Mathf.Abs(angle) >= epsilonRotation)
            {
                virtualUser.Rotate(initialAngleDirection * rotationSpeed * Time.fixedDeltaTime);
            }
            else if (distance >= epsilonTranslation)
            {
                if (isFirst2) // 방향을 동기화
                {
                    isFirst2 = false;
                    SyncDirection(virtualUser, virtualTargetDirection);
                }
                else
                {
                    virtualUser.Translate(virtualUserTransform.forward * translationSpeed * Time.fixedDeltaTime, Space.World);
                }
            }
            else
            {
                if(isFirst3) // 위치를 동기화
                {
                    isFirst3 = false;
                    SyncPosition(virtualUser, virtualTargetPosition);
                }
                else
                {
                    episode.DeleteTarget();

                    //Debug.Log(string.Format("realUser: {0}", realUser.transform));
                    //Debug.Log(string.Format("virtualUser: {0}", virtualUser.transform));

                    isFirst = true;
                    isFirst2 = true;
                    isFirst3 = true;
                }
            }
        }

        UpdateCurrentState(virtualUserTransform);

        return GetDelta(virtualUserTransform.forward);
    }

    public (Redirector.GainType, float) RealMove(Object2D realUser, Redirector.GainType type, float degree) {
        Transform2D realUserTransform = realUser.transform;
        float appliedGain = 0;

        switch (type) {
            case Redirector.GainType.Translation:
                realUser.Translate(realUserTransform.forward * degree * Time.fixedDeltaTime, Space.World);
                break;
            case Redirector.GainType.Rotation:
                realUser.Rotate(degree * Time.fixedDeltaTime);
                break;
            case Redirector.GainType.Curvature:
                realUser.Translate(realUserTransform.forward * deltaPosition.magnitude * Time.fixedDeltaTime, Space.World);
                realUser.Rotate(degree * Time.fixedDeltaTime);
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
