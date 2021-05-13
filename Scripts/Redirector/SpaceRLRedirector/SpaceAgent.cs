using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class SpaceAgent : Agent
{
    RedirectedUnit unit;
    int eachActionSpace = 3; // for each obstacle, they have 3 action space (translation, rotation)

    public override void OnEpisodeBegin()
    {
        unit = GetComponent<RedirectedUnitObject>().unit;
        unit.SetRLAgent(this);
    }

    public override void CollectObservations(VectorSensor sensor) // sensor should be normalized in [-1, 1] or [0, 1], state space : 4 + 4 * 2 + 4 + 4 = 20
    {
        // real user local position
        Bounds2D realSpaceBound = unit.GetRealSpace().spaceObject.bound;
        Vector2 realUserLocalPosition = unit.GetRealUser().transform2D.localPosition;
        Vector2 n_realUserLocalPosition = new Vector2(realUserLocalPosition.x / realSpaceBound.extents.x, realUserLocalPosition.y / realSpaceBound.extents.y); // [-1, 1]

        // real user forward
        Vector2 realUserForward = unit.GetRealUser().transform2D.forward; // [-1, 1], already normalized

        // virtual user local position
        Bounds2D virtualSpaceBound = unit.GetVirtualSpace().spaceObject.bound;
        Vector2 virtualUserLocalPosition = unit.GetVirtualUser().transform2D.localPosition;
        Vector2 n_virtualUserLocalPosition = new Vector2(virtualUserLocalPosition.x / virtualSpaceBound.extents.x, virtualUserLocalPosition.y / virtualSpaceBound.extents.y); // [-1, 1]

        // virtual user forward
        Vector2 n_virtualUserForward = unit.GetVirtualUser().transform2D.forward; // [-1, 1], already normalized

        // virtual obstacle local positions
        List<Object2D> obstacles = unit.GetVirtualSpace().obstacles;
        Vector2[] n_ObstacleLocalPositions = new Vector2[obstacles.Count];
        for(int i=0; i< obstacles.Count; i++)
            n_ObstacleLocalPositions[i] = new Vector2(obstacles[i].transform2D.localPosition.x / virtualSpaceBound.extents.x, obstacles[i].transform2D.localPosition.y / virtualSpaceBound.extents.y); // [-1, 1]

        //sensor.AddObservation(n_realUserLocalPosition);
        //sensor.AddObservation(realUserForward);
        sensor.AddObservation(n_virtualUserLocalPosition);
        sensor.AddObservation(n_virtualUserForward);

        for (int i = 0; i < obstacles.Count; i++)
            sensor.AddObservation(n_ObstacleLocalPositions[i]);

        Vector2 realUserRight = unit.GetRealUser().transform2D.right; // [-1, 1], already normalized
        Edge2D[] realBoundEdges = unit.GetRealSpace().spaceObject.bound.GetEdges();
        Ray2D[] realRay = new Ray2D[4];

        realRay[0] = new Ray2D(realUserLocalPosition, realUserForward); // +z 방향 ray
        realRay[1] = new Ray2D(realUserLocalPosition, -realUserRight); // -x 방향 ray
        realRay[2] = new Ray2D(realUserLocalPosition, -realUserForward); // -z 방향 ray
        realRay[3] = new Ray2D(realUserLocalPosition, realUserRight); // +x 방향 ray    

        // distances from real user to realSpace (wall or obstacle)
        float[] realDistance = new float[4];
        float realSpaceSize = unit.GetRealSpace().spaceObject.bound.size.x;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector2? intersect = realBoundEdges[i].GetIntersect(realRay[j], 0, "exclude");

                if (intersect != null)
                {
                    realDistance[j] = Vector2.Distance(intersect.Value, realUserLocalPosition) / realSpaceSize;
                    sensor.AddObservation(realDistance[j]);
                }
            }
        }

        // distances from virtual user to virtual obstacles
        float[] virtualDistance = new float[obstacles.Count];
        float virtualSpaceDigonalDistance = Vector2.Distance(virtualSpaceBound.max, virtualSpaceBound.min);
        for (int i = 0; i < obstacles.Count; i++)
        {
            virtualDistance[i] = Vector2.Distance(obstacles[i].transform2D.localPosition, virtualUserLocalPosition) / virtualSpaceDigonalDistance;
            sensor.AddObservation(virtualDistance[i]);
        }
    }

    public override void OnActionReceived(float[] vectorAction) // vectorAction is normalized in [-1, 1], action space : 4 * 3 = 12
    {
        SpaceRedirector spaceRedirector = (SpaceRedirector) unit.GetRedirector();
        float maxTranslation = 4;
        //float maxTranslation = unit.GetVirtualSpace().spaceObject.bound.extents.x;

        for (int i =0; i<vectorAction.Length; i += eachActionSpace)
        {
            Vector2 selectedTranslation = new Vector2(vectorAction[i] * maxTranslation, vectorAction[i + 1] * maxTranslation); // denormalized [-1, 1]
            float selectedRotation = vectorAction[i + 2] * 180; // denormalized [-180, 180]
            Vector2 selectedScale = Vector2.zero; // scale value does not use

            int j = i / eachActionSpace;
            spaceRedirector.obstacleActions[j].setObstacleAction(selectedTranslation, selectedRotation, selectedScale);
        }

        //Debug.Log("OnActionReceived: " + i++);
        if (unit.flag == FLAG.IDLE)
        {
            AddReward(+0.005f);
        }
        else if (unit.flag == FLAG.RESET_OCCUR)
        {
            SetReward(-1.0f);
        }
        else if (unit.flag == FLAG.END)
        {
            EndEpisode();
        }

        //Debug.Log(unit.flag);
        //Debug.Log(GetCumulativeReward());

        //// 실제 사용자로 부터 4 방향으로 여유 거리를 계산
        //Object2D realUser = unit.GetRealUser();
        //Edge2D[] realBoundEdges = unit.GetRealSpace().spaceObject.bound.GetEdges();
        //Ray2D[] realRay = new Ray2D[4];
        //float[] realDistance = new float[4];
        ////Edge2D[] temp = new Edge2D[4];

        //realRay[0] = new Ray2D(realUser.transform2D.localPosition, realUser.transform2D.forward); // +z 방향 ray
        //realRay[1] = new Ray2D(realUser.transform2D.localPosition, -realUser.transform2D.right); // -x 방향 ray
        //realRay[2] = new Ray2D(realUser.transform2D.localPosition, -realUser.transform2D.forward); // -z 방향 ray
        //realRay[3] = new Ray2D(realUser.transform2D.localPosition, realUser.transform2D.right); // +x 방향 ray    

        //for (int i=0; i<4; i++)
        //{
        //    for(int j=0; j<4; j++)
        //    {
        //        Vector2? intersect = realBoundEdges[i].GetIntersect(realRay[j], 0, "exclude");

        //        if(intersect != null)
        //        {
        //            realDistance[j] = Vector2.Distance(intersect.Value, realUser.transform2D.localPosition);
        //            //temp[j] = new Edge2D(realUser.transform2D.localPosition, intersect.Value);
        //        }
        //    }
        //}

        ////for (int i = 0; i < 4; i++)
        ////    temp[i].DebugDraw(Color.red);

        //Space2D virtualSpace = unit.GetVirtualSpace();
        //Object2D virtualUser = unit.GetVirtualUser();
        //int[] zone = new int[4];

        //for (int i = 0; i < 4; i++)
        //{
        //    Vector2 directionToObstacle = virtualSpace.obstacles[i].transform2D.localPosition - virtualUser.transform2D.localPosition;
        //    float angle = Vector2.SignedAngle(virtualUser.transform2D.forward, directionToObstacle);

        //    if (angle >= 0 && angle < 90) // 2 사분면
        //    {
        //        zone[i] = 2;
        //    }
        //    else if (angle >= 90 && angle < 180) // 3 사분면
        //    {
        //        zone[i] = 3;
        //    }
        //    else if (angle >= -90 && angle > 0) // 1 사분면
        //    {
        //        zone[i] = 1;
        //    }
        //    else // 4 사분면
        //    {
        //        zone[i] = 4;
        //    }
        //}

        //Dictionary<int, float> realAreas = new Dictionary<int, float>();
        //for (int i = 0; i < 4; i++)
        //    realAreas.Add(i, realDistance[i] * realDistance[(i + 1) % 4]);

        //var sortedAreas = realAreas.OrderBy(x => x.Value);

        //int minIndex = sortedAreas.ElementAt(0).Key; // 가장 여유 공간이 적은 사분면
        //int secondMinIndex = sortedAreas.ElementAt(1).Key; // 두번쨰로 여유 공간이 적은 사분면

        //for (int i = 0; i < 4; i++)
        //{
        //    //Debug.Log(zone[i]);
        //    if (zone[i] == minIndex)
        //        AddReward(+0.01f);
        //    else if (zone[i] == secondMinIndex)
        //        AddReward(+0.005f);
        //}
        ////Debug.Log("");
    }

    public override void Heuristic(float[] actionsOut)
    {
        for(int i=0; i< actionsOut.Length; i++)
        {
            if(i % eachActionSpace == 0 || i % eachActionSpace == 1) // translation 
                actionsOut[i] = UnityEngine.Random.Range(-1.0f, 1.0f);
            else if (i % eachActionSpace == 2) // rotation 
                actionsOut[i] = UnityEngine.Random.Range(-1.0f, 1.0f);
            else // 나머지는 선택 x
                actionsOut[i] = 0;
        }
    }
}
 
