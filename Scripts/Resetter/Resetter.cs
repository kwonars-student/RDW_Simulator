using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resetter
{
    //protected RedirectedUnit redirectedUnit;
    //protected Space2D realSpace;
    //public bool isComplete;
    public bool isFirst;
    protected float epsilonRotation, epsilonTranslation;
    private Vector2 realTargetRotation, virtualTargetRotation;

    public Resetter() {
        //redirectedUnit = null;
        //realSpace = null;
        //isComplete = false;
        isFirst = true;
        float translationSpeed = 4.0f;
        float rotationSpeed = 60.0f;
        epsilonRotation = (rotationSpeed * Time.fixedDeltaTime / 2) + 0.001f;
        epsilonTranslation = (translationSpeed * Time.fixedDeltaTime / 2) + 0.001f;
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

    public virtual bool ApplyReset(Object2D realUser, Object2D virtualUser, Space2D realSpace) { return true; }

    // TODO: UserReset도 모듈화?
    //public bool ApplyUserReset(Object2D realUser, Object2D virtualUser)
    //{
    //    float targetAngle = 180;
    //    float ratio = 2;
    //    float rotationSpeed = 60.0f;

    //    if (isFirst)
    //    {
    //        realTargetRotation = Matrix3x3.CreateRotation(targetAngle) * realUser.transform.forward;
    //        virtualTargetRotation = Matrix3x3.CreateRotation(ratio * targetAngle) * virtualUser.transform.forward;
    //        isFirst = false;
    //    }

    //    realUser.Rotate(rotationSpeed * Time.deltaTime);
    //    virtualUser.Rotate(ratio * rotationSpeed * Time.deltaTime);

    //    float realAngle = Vector2.SignedAngle(realUser.transform.forward, realTargetRotation);

    //    if (realAngle < 0.1f)
    //    {
    //        Utility.SyncDirection(virtualUser, realUser, virtualTargetRotation, realTargetRotation);
    //        realUser.transform.position = realUser.transform.position + realUser.transform.forward * 0.1f;
    //        isFirst = true;
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    public bool NeedWallReset(Object2D realUser, Space2D realSpace) {
        Vector2 realUserPosition = realUser.transform.localPosition;
        return !realSpace.IsInside(realUserPosition, 0);
    }

    public bool NeedUserReset(Object2D realUser, List<Object2D> otherUsers)
    {
        bool flag = false;
        float translationSpeed = 4;
        float epsilon = translationSpeed * Time.fixedDeltaTime;

        for (int i = 0; i < otherUsers.Count; i++)
        {
            //if (this.id == otherUnits[i].GetID())
            //    continue;

            if (realUser.IsIntersect(otherUsers[i], epsilon))
            {
                //resultData.AddUserReset();
                //isFirst = false;

                Transform2D b = realUser.transform; // (=this)
                Transform2D a = otherUsers[i].transform; // (=unitList[i])

                if (Vector2.Dot(a.forward, b.forward) >= 0) // 한쪽만 reset 해야 되는 경우
                {
                    Vector2 dir = a.localPosition - b.localPosition;

                    if (Vector2.Dot(b.forward, dir) >= 0) // b의 바라보는 방향 기준으로 a가 b보다 앞쪽에 있는 경우 b를 reset 시킨다
                    {
                        Debug.Log(string.Format("{0}의 바라보는 방향 기준으로 {1}가 {0}보다 앞쪽에 있는 경우", "b", "a"));
                        flag = true;
                    }
                    else // b의 바라보는 방향 기준으로 a가 b보다 뒤쪽에 있는 경우 a를 reset 시킨다
                    {
                        Debug.Log(string.Format("{0}의 바라보는 방향 기준으로 {1}가 {0}보다 뒤쪽에 있는 경우", "b", "a"));
                        flag = false;

                        //if (targetUnit.status == "IDLE")// TODO: 다른 경우가 있는가? 즉, intersectedUnit이 IDLE이 아닐 가능성은?
                        //{
                        //    resetOtherUnit = true;
                        //    targetUnit.status = "USER_RESET";
                        //}
                    }
                }
                else // 둘다 reset 해야 되는 경우
                {
                    Debug.Log(string.Format("둘다 reset 해야 되는 경우"));
                    flag = true;
                    //resetThisUnit = true;

                    //if (targetUnit.status == "IDLE")
                    //{
                    //    resetOtherUnit = true;
                    //    targetUnit.status = "USER_RESET";
                    //}
                }
            }
        }

        return flag;
    }

    public bool NeedUserReset(Object2D realUser, Object2D otherUser)
    {
        float translationSpeed = 4;
        return realUser.IsIntersect(otherUser, translationSpeed * Time.fixedDeltaTime);
    }
}
