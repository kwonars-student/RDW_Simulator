using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class SpaceAgent : Agent
{
    RedirectedUnit unit;
    int eachActionSpace = 2; // for each obstacle, they have 2 action space (translation,)

    public override void OnEpisodeBegin()
    {
        unit = GetComponent<RedirectedUnitObject>().unit;
        unit.SetRLAgent(this);
    }

    public override void CollectObservations(VectorSensor sensor) // sensor should be normalized in [-1, 1] or [0, 1], state space : 4 + 4 * 2
    {
        // real user local rotation
        //float realUserLocalRotation = unit.GetRealUser().transform2D.localRotation;
        //float normalizedLocalRotation = ((realUserLocalRotation % 360) + 360) % 360 / 360; // [0, 1]
        //sensor.AddObservation(normalizedLocalRotation);

        // real user local position
        Bounds2D realSpaceBound = unit.GetRealSpace().spaceObject.bound;
        Vector2 realUserLocalPosition = unit.GetRealUser().transform2D.localPosition;
        Vector2 normalizedLocalPosition = new Vector2(realUserLocalPosition.x / realSpaceBound.extents.x, realUserLocalPosition.y / realSpaceBound.extents.y); // [-1, 1]

        // real user forward
        Vector2 realUserForward = unit.GetRealUser().transform2D.forward; // [-1, 1], already normalized

        // virtual obstacle local positions
        Bounds2D virtualSpaceBound = unit.GetVirtualSpace().spaceObject.bound;
        List<Object2D> obstacles = unit.GetVirtualSpace().obstacles;
        Vector2[] normalizedObstacleLocalPositions = new Vector2[obstacles.Count];
        for(int i=0; i< obstacles.Count; i++)
            normalizedObstacleLocalPositions[i] = new Vector2(obstacles[i].transform2D.localPosition.x / virtualSpaceBound.extents.x, obstacles[i].transform2D.localPosition.y / virtualSpaceBound.extents.y); // [-1, 1]

        sensor.AddObservation(normalizedLocalPosition);
        sensor.AddObservation(realUserForward);
        for (int i = 0; i < obstacles.Count; i++)
            sensor.AddObservation(normalizedObstacleLocalPositions[i]);
    }

    [System.Obsolete]
    public override void OnActionReceived(float[] vectorAction) // vectorAction is normalized in [-1, 1], action space : 4 * 2
    {
        SpaceRedirector spaceRedirector = (SpaceRedirector) unit.GetRedirector();
        float maxTranslation = 1;
        float maxScale = 2;

        for (int i =0; i<vectorAction.Length; i += eachActionSpace)
        {
            Vector2 selectedTranslation = new Vector2(vectorAction[i] * maxTranslation, vectorAction[i + 1] * maxTranslation);
            //float selectedRotation = vectorAction[i + 2] * 180;
            //Vector2 selectedScale = new Vector2(vectorAction[i + 3] * maxScale, vectorAction[i + 4] * maxScale);
            float selectedRotation = 0;
            Vector2 selectedScale = Vector2.zero;

            int j = i / eachActionSpace;
            spaceRedirector.obstacleActions[j].setObstacleAction(selectedTranslation, selectedRotation, selectedScale);
        }
    }

    [System.Obsolete]
    public override void Heuristic(float[] actionsOut)
    {
        for(int i=0; i< actionsOut.Length; i++)
        {
            if(i % eachActionSpace == 0 || i % eachActionSpace == 1) // translation 
                actionsOut[i] = Random.Range(-1.0f, 1.0f);
            //else if(i % 5 == 2) // rotation 
            //    actionsOut[i] = Random.Range(-1.0f, 1.0f);
            //else if(i % 5 == 3 || i % 5 == 4) // localScale
            //    actionsOut[i] = 0;
            else // 나머지는 선택 x
                actionsOut[i] = 0;
        }
    }
}
 
