using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEpisode : Episode
{
    public RandomEpisode() : base() { }

    public RandomEpisode(int episodeLength) : base(episodeLength) { }

    protected override void GenerateEpisode(Transform2D virtualUserTransform, Space2D virtualSpace)
    {
        Vector2 samplingPosition = Vector2.zero;
        Vector2 userPosition = virtualUserTransform.localPosition;


        do
        {
            float angle = Utility.sampleUniform(-180.0f, 180.0f);
            float distance = Utility.sampleUniform(2.0f, 5.0f);
            Vector2 sampleForward = Utility.RotateVector2(virtualUserTransform.forward, angle);

            samplingPosition = userPosition + sampleForward * distance; // local 좌표계에서 절대 위치 기준

        } while (!virtualSpace.IsInside(samplingPosition, Space.Self, 0.3f)); // !virtualSpace.IsPossiblePath(samplingPosition, userPosition, Space.Self, 0.2f)

        currentTargetPosition = samplingPosition;
    }
}
