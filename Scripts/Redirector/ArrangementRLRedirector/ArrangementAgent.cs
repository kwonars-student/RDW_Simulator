using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class ArrangementAgent : Agent
{
    RedirectedUnit unit;
    // int eachActionSpace = 2; // for each obstacle, they have 2 action space (translation,)
    int eachActionSpace = 3; // for each obstacle, they have 3 action space (translation, rotation)
    bool ready = true;
    bool ready2 = true;
    int cnt = 0;

    // 에피소드가 시작할때마다 호출
    public override void OnEpisodeBegin()
    {
        unit = GetComponent<RedirectedUnitObject>().unit;
        unit.SetRLArrangementAgent(this);
        //ArrangementRedirector arrangementRedirector = (ArrangementRedirector)unit.GetRedirector();

        // Debug.Log("OnEpisodeBegin called");

        // if (ready2)
        // {
        //     Debug.Log("EpisodeBegin Called");
        //     unit = GetComponent<RedirectedUnitObject>().unit;
        //     unit.SetRLArrangementAgent(this);
        //     //ArrangementRedirector arrangementRedirector = (ArrangementRedirector)unit.GetRedirector();

        //     ready2 = false;

        // }
        // else if (!ready2 && unit.GetEpisode().GetCurrentEpisodeIndex() == unit.GetEpisode().GetEpisodeLength())
        // {
        //     ready2 = true;
        // }

    }

    //환경 정보를 관측 및 수집해 정책 결정을 위해 브레인에 전달하는 메소드
    public override void CollectObservations(VectorSensor sensor) // sensor should be normalized in [-1, 1] or [0, 1], state space : 2(float) + 4(Vec2) + 4(Vec2) + 8(개) * 2(Vec2)
    {
        // Debug.Log("CollectObservations called");
        unit = GetComponent<RedirectedUnitObject>().unit;
        // if (unit.GetEpisode().GetCurrentEpisodeIndex() == cnt || true)
        // {
        //     // Debug.Log("Sensored : " + cnt);

        // real user local rotation
        float realUserLocalRotation = unit.GetRealUser().transform2D.localRotation;
        // float normalizedRealLocalRotation = ((realUserLocalRotation % 360) + 360) % 360 / 360; // [0, 1]
        float normalizedRealLocalRotation = (realUserLocalRotation + 180) / 180; // [-1, 1]
        sensor.AddObservation(normalizedRealLocalRotation);

        // real user local position
        Bounds2D realSpaceBound = unit.GetRealSpace().spaceObject.bound;
        Vector2 realUserLocalPosition = unit.GetRealUser().transform2D.localPosition;
        Vector2 normalizedRealLocalPosition = new Vector2(realUserLocalPosition.x / realSpaceBound.extents.x, realUserLocalPosition.y / realSpaceBound.extents.y); // [-1, 1]
        sensor.AddObservation(normalizedRealLocalPosition);

        // // real user forward
        // Vector2 realUserForward = unit.GetRealUser().transform2D.forward; // [-1, 1], already normalized
        // sensor.AddObservation(realUserForward);


        // virtual user local rotation
        float virtualUserLocalRotation = unit.GetVirtualUser().transform2D.localRotation;
        // float normalizedVirtualLocalRotation = ((virtualUserLocalRotation % 360) + 360) % 360 / 360; // [0, 1]
        float normalizedVirtualLocalRotation = (virtualUserLocalRotation + 180) / 180; // [-1, 1]
        sensor.AddObservation(normalizedVirtualLocalRotation);

        // virtual user local position
        Bounds2D virtualSpaceBound = unit.GetVirtualSpace().spaceObject.bound;
        Vector2 virtualUserLocalPosition = unit.GetVirtualUser().transform2D.localPosition;
        Vector2 normalizedVirtualLocalPosition = new Vector2(virtualUserLocalPosition.x / virtualSpaceBound.extents.x, virtualUserLocalPosition.y / virtualSpaceBound.extents.y); // [-1, 1]
        sensor.AddObservation(normalizedVirtualLocalPosition);

        // Debug.Log("VirLoc: " + virtualUserLocalPosition + ", VirInitLoc: " + unit.GetEpisode().GetVirtualAgentInitialPosition());

        // // virtual user forward
        // Vector2 virtualUserForward = unit.GetVirtualUser().transform2D.forward; // [-1, 1], already normalized
        // sensor.AddObservation(virtualUserForward);


        // // virtual obstacle local positions
        // // Bounds2D virtualSpaceBound = unit.GetVirtualSpace().spaceObject.bound;
        // List<Object2D> obstacles = unit.GetVirtualSpace().obstacles;
        // Vector2[] normalizedObstacleLocalPositions = new Vector2[obstacles.Count];
        // for (int i = 0; i < obstacles.Count; i++)
        // {
        //     normalizedObstacleLocalPositions[i] = new Vector2(obstacles[i].transform2D.localPosition.x / virtualSpaceBound.extents.x, obstacles[i].transform2D.localPosition.y / virtualSpaceBound.extents.y); // [-1, 1]
        //     sensor.AddObservation(normalizedObstacleLocalPositions[i]);
        // }

        // virtual obstacle relative positions
        // Bounds2D virtualSpaceBound = unit.GetVirtualSpace().spaceObject.bound;
        // List<Object2D> obstacles = unit.GetVirtualSpace().obstacles;
        // Vector2[] normalizedObstacleLocalPositions = new Vector2[obstacles.Count];
        // for (int i = 0; i < obstacles.Count; i++)
        // {
        //     normalizedObstacleLocalPositions[i] =
        //         new Vector2(
        //             (obstacles[i].transform2D.localPosition.x-unit.GetVirtualUser().transform2D.localPosition.x) / virtualSpaceBound.extents.x,
        //             (obstacles[i].transform2D.localPosition.y-unit.GetVirtualUser().transform2D.localPosition.y) / virtualSpaceBound.extents.y
        //             ); // [-1, 1]
        //     sensor.AddObservation(normalizedObstacleLocalPositions[i]);
        // }

        // // virtual obstacle relative rotations
        // // Bounds2D virtualSpaceBound = unit.GetVirtualSpace().spaceObject.bound;
        // // List<Object2D> obstacles = unit.GetVirtualSpace().obstacles;
        // float[] normalizedObstacleLocalRotations = new float[obstacles.Count];
        // Debug.Log(obstacles[0].transform2D.localRotation);
        // for (int i = 0; i < obstacles.Count; i++)
        // {
        //     normalizedObstacleLocalRotations[i] = (obstacles[i].transform2D.localRotation - unit.GetVirtualUser().transform2D.localRotation) / 360f; // [-1, 1]

        //     sensor.AddObservation(normalizedObstacleLocalPositions[i]);
        // }

        //cnt++;
        // }
        // else if (unit.GetEpisode().GetCurrentEpisodeIndex() == unit.GetEpisode().GetEpisodeLength())
        // {
        //     cnt = 0;
        // }

    }

     //브레인(정책)으로 부터 전달 받은 행동을 실행하는 메소드
    public override void OnActionReceived(float[] vectorAction) // vectorAction is normalized in [-1, 1], action space : 8(개) * 2(Vec2)
    {
        // Debug.Log("OnActionReceived called");
        if (unit.GetEpisode().GetCurrentEpisodeIndex() == 0 && ready)
        {
            //Debug.Log("CurrentEpisodeIndex in OnActionReceived : " + unit.GetEpisode().GetCurrentEpisodeIndex());
            ArrangementRedirector arrangementRedirector = (ArrangementRedirector)unit.GetRedirector();
            float maxTranslation = 3f; // Environment1: 2f, Environment2: 2f, Environment2: 3f
            float maxScale = 0;

            for (int i = 0; i < vectorAction.Length; i += eachActionSpace)
            {
                Vector2 selectedTranslation = new Vector2(vectorAction[i] * maxTranslation, vectorAction[i + 1] * maxTranslation);
                float selectedRotation = vectorAction[i + 2] * 45;
                // float selectedRotation = 0;
                //Vector2 selectedScale = new Vector2(vectorAction[i + 3] * maxScale, vectorAction[i + 4] * maxScale);
                Vector2 selectedScale = Vector2.zero;

                int j = i / eachActionSpace;
                arrangementRedirector.obstacleActions[j].setObstacleAction(selectedTranslation, selectedRotation, selectedScale);
            }

            ready = false;
        }
        //else if (!ready && (unit.GetEpisode().GetCurrentEpisodeIndex() == unit.GetEpisode().GetEpisodeLength()))
        //{
        //    ready = true;
        //}

    }

    //개발자(사용자)가 직접 명령을 내릴때 호출하는 메소드(주로 테스트용도 또는 모방학습에 사용)
    public override void Heuristic(float[] actionsOut)
    {
        for(int i=0; i< actionsOut.Length; i++)
        {

            if(i % 5 == 0 || i % 5 == 1) // translation만 랜덤하게 선택
                actionsOut[i] = Random.Range(-1.0f, 1.0f);
            else if(i % 5 == 2)
                actionsOut[i] = Random.Range(-1.0f, 1.0f);
            else if(i % 5 == 3 || i % 5 == 4)
                actionsOut[i] = Random.Range(-1.0f, 1.0f);
            else // 나머지는 선택 x
                actionsOut[i] = 0;
        }
    }


    public void SetActionReady(bool ready)
    {
        this.ready = ready;
    }
}
 
