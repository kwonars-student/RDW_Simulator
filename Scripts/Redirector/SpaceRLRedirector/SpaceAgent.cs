using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class SpaceAgent : Agent
{
    RedirectedUnit unit;


    public override void OnEpisodeBegin()
    {
        unit = GetComponent<RedirectedUnitObject>().unit;
        unit.SetRLAgent(this);
        unit.GetEpisode().ResetEpisode();
    }

    public override void CollectObservations(VectorSensor sensor) // sensor should be normalized in [-1, 1] or [0, 1], state space : 3
    {
        Bounds2D realSpaceBound = unit.GetRealSpace().spaceObject.bound;
        Vector2 realUserLocalPosition = unit.GetRealUser().transform2D.localPosition;
        Vector2 normalizedLocalPosition = new Vector2(realUserLocalPosition.x / realSpaceBound.extents.x, realUserLocalPosition.y / realSpaceBound.extents.y); // [-1, 1]
        float realUserLocalRotation = unit.GetRealUser().transform2D.localRotation;
        float normalizedLocalRotation = ((realUserLocalRotation % 360) + 360) % 360 / 360; // [0, 1]

        sensor.AddObservation(normalizedLocalPosition);
        sensor.AddObservation(normalizedLocalRotation);
    }

    public override void OnActionReceived(float[] vectorAction) // vectorAction is normalized in [-1, 1], action space : 20
    {
        SpaceRedirector spaceRedirector = (SpaceRedirector) unit.GetRedirector();
        int eachActionSpace = 5; // for each obstacle, they have 5 action space
        float maxTranslation = 0 ;
        float maxScale = 0;
        float rotationParameter = 0;

        //Debug.Log("Number of resets:" + unit.resultData.getTotalReset());

        for(int i =0; i<vectorAction.Length; i += eachActionSpace)
        {
            Vector2 selectedTranslation = new Vector2(vectorAction[i] * maxTranslation, vectorAction[i + 1] * maxTranslation);
            float selectedRotation = vectorAction[i + 2] * rotationParameter;
            Vector2 selectedScale = new Vector2(vectorAction[i + 3] * maxScale, vectorAction[i + 4] * maxScale);

            int j = i / eachActionSpace;
            spaceRedirector.obstacleActions[j].setObstacleAction(selectedTranslation, selectedRotation, selectedScale);
        }
    }

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
}
 
