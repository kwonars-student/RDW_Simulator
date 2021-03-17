using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resetter
{
    protected float translationSpeed;
    protected float rotationSpeed;
    public bool isFirst;
    public int wallResetCount;
    //protected float epsilonRotation, epsilonTranslation;

    float initialAngle;
    float maxRotTime, remainRotTime;

    public Resetter() { // 기본 생성자
        isFirst = true;
        //epsilonRotation = (rotationSpeed * Time.fixedDeltaTime / 2) + 0.001f;
        //epsilonTranslation = (translationSpeed * Time.fixedDeltaTime / 2) + 0.001f;
    }

    public Resetter(float translationSpeed, float rotationSpeed)
    {
        this.translationSpeed = translationSpeed;
        this.rotationSpeed = rotationSpeed;
        isFirst = true;
        //epsilonRotation = (rotationSpeed * Time.fixedDeltaTime / 2) + 0.001f;
        //epsilonTranslation = (translationSpeed * Time.fixedDeltaTime / 2) + 0.001f;
    }

    public void SyncDirection(Object2D realUser, Vector2 realTargetDirection)
    {
        if (realTargetDirection.magnitude > 1)
            realTargetDirection = realTargetDirection.normalized;

        realUser.transform2D.forward = realTargetDirection;
        if (realUser.gameObject != null) realUser.gameObject.transform.forward = Utility.CastVector2Dto3D(realTargetDirection);
    }

    public virtual string ApplyWallReset(Object2D realUser, Object2D virtualUser, Space2D realSpace) {
        realUser.transform2D.localPosition = Vector2.zero;
        return "WALL_RESET_DONE";
    }

    public string ApplyUserReset(Object2D realUser, Vector2 resetDirection)
    {
        float rotationSpeed = 60.0f;

        if (isFirst)
        {
            initialAngle = Vector2.SignedAngle(realUser.transform2D.forward, resetDirection);
            isFirst = false;
            maxRotTime = Mathf.Abs(initialAngle) / rotationSpeed;
            remainRotTime = 0;
        }
        if (remainRotTime < maxRotTime)
        {
            realUser.transform2D.Rotate(Mathf.Sign(initialAngle) * rotationSpeed * Time.fixedDeltaTime);
            remainRotTime += Time.fixedDeltaTime;
        }
        else
        {
            SyncDirection(realUser, resetDirection);
            isFirst = true;
            return "USER_RESET_DONE";
        }

        return "IDLE";
    }

    public bool NeedWallReset(Object2D realUser, Space2D realSpace)
    {
        Vector2 realUserPosition = realUser.transform2D.localPosition;
        return !realSpace.IsInside(realUserPosition, Space.Self, 0.00001f); // 0.2f
        //return !realSpace.IsInside(realUser, 0);
    }

    public bool NeedUserReset(Object2D realUser, List<Object2D> otherUsers, out Object2D intersectedUser)
    {
        bool flag = false;
        float translationSpeed = 4;
        float epsilon = translationSpeed * Time.fixedDeltaTime;
        Object2D targetUser = null;

        for (int i = 0; i < otherUsers.Count; i++)
        {
            if (realUser.IsIntersect(otherUsers[i]))
            {
                Transform2D b = realUser.transform2D; // (=this)
                Transform2D a = otherUsers[i].transform2D; // (=unitList[i])

                if (Vector2.Dot(a.forward, b.forward) >= 0) // 한쪽만 reset 해야 되는 경우
                {
                    Vector2 dir = a.localPosition - b.localPosition;

                    if (Vector2.Dot(b.forward, dir) >= 0) // b의 바라보는 방향 기준으로 a가 b보다 앞쪽에 있는 경우 b를 reset 시킨다
                    {
                        flag = true;
                        targetUser = otherUsers[i];
                    }
                    else // b의 바라보는 방향 기준으로 a가 b보다 뒤쪽에 있는 경우 a를 reset 시킨다
                    {
                        flag = false;
                    }
                }
                else // 둘다 reset 해야 되는 경우
                {
                    flag = true;
                    targetUser = otherUsers[i];
                }
            }
        }

        intersectedUser = targetUser;
        return flag;
    }
}
