using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingEpisode : Episode
{
    private int count;
    private int emergencyExitCount;
    private bool emergencyExit = false;
    private Vector2 previousUserPosition;

    public WanderingEpisode() : base() { }

    public WanderingEpisode(int episodeLength) : base(episodeLength) { }


    protected override void GenerateEpisode(Transform2D virtualUserTransform, Space2D virtualSpace)
    {
        Vector2 samplingPosition = Vector2.zero;
        Vector2 userPosition = virtualUserTransform.localPosition;
        count = 0;

        do
        {
            count++;
           
            float angle = Utility.sampleNormal(0f, 18f, -180f, 180f);
            float distance = 0.2f;
            //float distance = Utility.sampleNormal(0.4f, 2f, 0.25f, 5f); Distance도 랜덤. 깊숙히 안들어가는 문제 약간 보임.
            // Debug.Log(-virtualUserTransform.localPosition * Time.fixedDeltaTime);
            if(count >= 100)
            {
                angle = Utility.sampleUniform(90f, 270f);
                count = 1;
                emergencyExitCount++;
                
                if(emergencyExitCount == 5)
                {
                    emergencyExit = true;
                    emergencyExitCount = 0;
                    virtualUserTransform.Translate(previousUserPosition - virtualUserTransform.localPosition, Space.World);
                    break;
                }
            }

            Vector2 sampleForward = Utility.RotateVector2(virtualUserTransform.forward, angle);
            samplingPosition = userPosition + sampleForward * distance; // local 좌표계에서 절대 위치 기준

        } while (!virtualSpace.IsInside(samplingPosition, Space.Self, 0.0f)); // !virtualSpace.IsPossiblePath(samplingPosition, userPosition, Space.Self, 0.2f)

        if (emergencyExit)
        {
            emergencyExit = false;
            currentTargetPosition = previousUserPosition;
        }
        else
        {
            count = 1;
            previousUserPosition = userPosition;
            currentTargetPosition = samplingPosition;
        }
        
    }
}
