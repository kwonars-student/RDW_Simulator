using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resetter
{
    protected static float translationSpeed;
    protected static float rotationSpeed;
    //protected RedirectedUnit redirectedUnit;
    //protected Space2D realSpace;
    //public bool isComplete;
    public bool isFirst;
    protected float epsilonRotation, epsilonTranslation;
    private Vector2 realTargetRotation, virtualTargetRotation;

    private float adjusmentEpsilon = 0.01f;

    public Resetter() {
        //redirectedUnit = null;
        //realSpace = null;
        //isComplete = false;
        isFirst = true;
        epsilonRotation = (rotationSpeed * Time.fixedDeltaTime / 2) + 0.001f;
        epsilonTranslation = (translationSpeed * Time.fixedDeltaTime / 2) + 0.001f;
    }

    public static void SetTransRotSpeed(float translationSpeed, float rotationSpeed){
        Resetter.translationSpeed = translationSpeed;
        Resetter.rotationSpeed = rotationSpeed;
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

    public virtual bool ApplyReset(Object2D realUser, Object2D virtualUser, Space2D realSpace, string resetType) { return true; }

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
                        //Debug.Log(string.Format("{0}의 바라보는 방향 기준으로 {1}가 {0}보다 앞쪽에 있는 경우", "b", "a"));
                        flag = true;
                    }
                    else // b의 바라보는 방향 기준으로 a가 b보다 뒤쪽에 있는 경우 a를 reset 시킨다
                    {
                        //Debug.Log(string.Format("{0}의 바라보는 방향 기준으로 {1}가 {0}보다 뒤쪽에 있는 경우", "b", "a"));
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
                    //Debug.Log(string.Format("둘다 reset 해야 되는 경우"));
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
        return realUser.IsIntersect(otherUser, translationSpeed * Time.fixedDeltaTime);
    }

    public void CalculationErrorAdjustment(Transform2D transform, string type, Polygon2D polygon)
    {
        List<Vector2> vertices = polygon.GetVertices();
        float minDis = 1000.0f;
        float dis = 0.0f;
        Vector2 minVer1 = Vector2.zero;
        Vector2 minVer2 = Vector2.zero;

        switch (type)
        {
            case "Wall":
                for (int i = 0; i < vertices.Count; i++)
                {
                    if (i + 1 == vertices.Count)
                    {
                        dis = GetMinimumDistanceToVertexLine(vertices[i], vertices[0], transform.localPosition);
                    }
                    else
                    {
                        dis = GetMinimumDistanceToVertexLine(vertices[i], vertices[i + 1], transform.localPosition);
                    }

                    if (dis < minDis)
                    {
                        minDis = dis;
                        minVer1 = vertices[i];
                        if (i + 1 == vertices.Count)
                        {
                            minVer2 = vertices[0];
                        }
                        else
                        {
                            minVer2 = vertices[i + 1];
                        }
                    }
                }
                Vector2 vertexLine = minVer2 - minVer1;
                Vector2 pointLine = transform.localPosition - minVer1;
                Vector3 dir = new Vector3(pointLine.x, 0.0f, pointLine.y);
                Vector3 val = Vector3.Cross(dir.normalized, new Vector3(vertexLine.x, 0.0f, vertexLine.y));

                if (val.y >= 0)
                {
                    Vector2 forwardVec = new Vector2(vertexLine.y, -vertexLine.x);
                    transform.localPosition = transform.localPosition + forwardVec.normalized * (minDis + adjusmentEpsilon);
                }
                else
                {
                    Vector2 forwardVec = new Vector2(-vertexLine.y, vertexLine.x);
                    transform.localPosition = transform.localPosition + forwardVec.normalized * (minDis + adjusmentEpsilon);
                }
                break;
            case "User":
                break;
        }
    }

    private float GetMinimumDistanceToVertexLine(Vector2 S, Vector2 E, Vector2 P)
    {
        Vector2 SE = E - S;
        Vector2 EP = P - E;
        Vector2 SP = P - S;

        float SEdotEP = (SE.x * EP.x + SE.y * EP.y);
        float SEdotSP = (SE.x * SP.x + SE.y * SP.y);

        float res = 0.0f;

        if(SEdotEP > 0)
        {
            float y = P.y - E.y;
            float x = P.x - E.x;
            res = Mathf.Sqrt(x * x + y * y);
        }else if(SEdotSP < 0)
        {
            float y = P.y - S.y;
            float x = P.x - S.x;
            res = Mathf.Sqrt(x * x + y * y);
        }
        else
        {
            float x1 = SE.x;
            float y1 = SE.y;
            float x2 = SP.x;
            float y2 = SP.y;
            float mod = Mathf.Sqrt(x1 * x1 + y1 * y1);
            res = Mathf.Abs(x1 * y2 - y1 * x2) / mod;
        }
        return res;
    }
    
}
