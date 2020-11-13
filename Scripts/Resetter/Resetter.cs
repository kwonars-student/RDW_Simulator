using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resetter
{
    //protected RedirectedUnit redirectedUnit;
    //protected Space2D realSpace;
    //public bool isComplete;
    public bool isFirst;
    private Vector2 realTargetRotation, virtualTargetRotation;

    public Resetter() {
        //redirectedUnit = null;
        //realSpace = null;
        //isComplete = false;
        isFirst = true;
    }

    //public Resetter(Space2D space) {
    //    //realSpace = space;
    //    //isComplete = false;
    //    isFirst = true;
    //}

    //public void SetReferences(RedirectedUnit redirectedUnit)
    //{
    //    this.redirectedUnit = redirectedUnit;
    //}

    public virtual void ApplyReset(Object2D realUser, Object2D virtualUser) {}

    // TODO: UserReset도 모듈화?
    public void ApplyUserReset(Object2D realUser, Object2D virtualUser)
    {
        float targetAngle = 180;
        float ratio = 2;
        float rotationSpeed = 60.0f;

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

    public bool NeedWallReset(Object2D realUser, Space2D realSpace) {
        Vector2 realUserPosition = realUser.transform.localPosition;
        return !realSpace.IsInside(realUserPosition);
    }

    public bool NeedUserReset(Object2D realUser, Object2D otherUser)
    {
        return realUser.IsIntersect(otherUser);
    }
}
