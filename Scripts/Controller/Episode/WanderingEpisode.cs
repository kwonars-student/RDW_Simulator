using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class WanderingEpisode : Episode
{
    private int count;
    private int emergencyExitCount;
    private bool emergencyExit = false;
    private Vector2 previousUserPosition;

    private bool predefinedMode = false;
    private string filePath;
    private TextReader reader;
    private List<Vector2> targetPositionList;

    public WanderingEpisode() : base()
    {
        //GetPreDefinedTargetFile("Test1000");
    }

    public WanderingEpisode(int episodeLength) : base(episodeLength)
    {
        //GetPreDefinedTargetFile("Test1000");
    }

    public void GetPreDefinedTargetFile(string fileName)
    {
        targetPositionList = new List<Vector2>();
        filePath = "Assets/Resources/pathData/" + fileName +".txt";
        reader = File.OpenText(filePath);

        string line = null;
        while ((line = reader.ReadLine()) != null) {
            string[] num = line.Split(',');
            float x = float.Parse(num[0]);
            float y = float.Parse(num[1]);
            targetPositionList.Add(new Vector2(x, y));
        }
        reader.Close();

        if (this.episodeLength != targetPositionList.Count)
            this.episodeLength = targetPositionList.Count;
    }

    protected override void GenerateEpisode(Transform2D virtualUserTransform, Space2D virtualSpace)
    {
        Vector2 samplingPosition = Vector2.zero;
        Vector2 sampleForward = Vector2.zero;
        Vector2 userPosition = virtualUserTransform.localPosition;
        
        count = 0;

        if (GetCurrentEpisodeIndex() <= 1)
        {
            previousUserPosition = virtualAgentInitialPosition;
        }

        do
        {
            count++;
            
            float angle = Utility.sampleUniform(-180.0f, 180.0f);
            float distance = 1.5f; // 0.5f

            sampleForward = Utility.RotateVector2(virtualUserTransform.forward, angle);
            samplingPosition = userPosition + sampleForward * distance; // local 좌표계에서 절대 위치 기준

            if(predefinedMode)
            {
                samplingPosition = targetPositionList[currentEpisodeIndex];

            }


            if (count >= 50)
            {
                //angle = Utility.sampleUniform(90f, 270f);
                angle = Utility.sampleUniform(90f, 270f);
                count = 1;
                emergencyExitCount++;
                
                if(emergencyExitCount == 5)
                {
                    emergencyExit = true;
                    emergencyExitCount = 0;

                    Vector2 movedVector1 = previousUserPosition - userPosition;
                    Vector2 movedVector2 = userPosition - virtualAgentInitialPosition;
                    
                    if (movedVector1.magnitude < distance-0.001f && movedVector2.magnitude < distance - 0.001f) // 이전과 현재 위치가 같으면서 초기 위치일 때
                    {
                        SetWrongEpisode(true);
                        SetCurrentEpisodeIndex(GetEpisodeLength());
                        // Debug.Log("Wrong Initialized Episode!");
                        // sampleForward = Utility.RotateVector2(virtualUserTransform.forward, angle);
                        // samplingPosition = userPosition + sampleForward * distance; // local 좌표계에서 절대 위치 기준
                    }

                    break;
                }
            }
        } while (!virtualSpace.IsInside(samplingPosition, Space.Self, 0.5f)); // !virtualSpace.IsPossiblePath(samplingPosition, userPosition, Space.Self, 0.2f)

        if (emergencyExit)
        {
            emergencyExit = false;

            // float angle = Utility.sampleNormal(0f, 18f, -180f, 180f);
            // float distance = 0.2f;
            // Vector2 sampleForward = Utility.RotateVector2(virtualUserTransform.forward, angle);
            // samplingPosition = userPosition + sampleForward * distance; // local 좌표계에서 절대 위치 기준
            if(GetWrongEpisode())
            {
                currentTargetPosition = userPosition;
            }
            else
            {
                currentTargetPosition = previousUserPosition;
            }
            
        }
        else
        {
            count = 1;
            previousUserPosition = userPosition;
            currentTargetPosition = samplingPosition;
            // Debug.Log("Move to target: " + samplingPosition);
        }

        // Vector2 initialToTarget = previousUserPosition - virtualUserTransform.localPosition;
        // float InitialAngle = Vector2.SignedAngle(virtualUserTransform.forward, initialToTarget);
        // float initialDistance = Vector2.Distance(virtualUserTransform.localPosition, previousUserPosition);
        // float initialAngleDirection = Mathf.Sign(InitialAngle);

        // Vector2 virtualTargetDirection = Matrix3x3.CreateRotation(InitialAngle) * virtualUserTransform.forward; // target을 향하는 direction(forward)를 구함
        // Vector2 virtualTargetPosition = virtualUserTransform.localPosition + virtualTargetDirection * initialDistance; // target에 도달하는 position을 구함

        // float maxRotTime = Mathf.Abs(InitialAngle) / 500;
        // float maxTransTime = initialDistance / 4;
        // float remainRotTime = 0;
        // float remainTransTime = 0;

        // if (maxTransTime - remainTransTime > 0.06f)
        // {
        //     // Debug.Log(maxTransTime - remainTransTime);
        // }
    }
}
