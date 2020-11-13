using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEpisode : Episode
{
    public RandomEpisode() : base() { }

    public RandomEpisode(int episodeLength) : base(episodeLength) { }

    //public RandomEpisode(Space2D virtualSpace, int episodeLength) : base(virtualSpace, episodeLength) { }

    protected override void GenerateEpisode(Transform2D virtualUserTransform, Space2D virtualSpace) {

        Vector2 tempPosition = Vector2.zero;

        do {
            float angle = Utility.sampleUniform(-180.0f, 180.0f);
            float distance = Utility.sampleUniform(2.0f, 5.0f);

            Vector2 sampleForward = Matrix3x3.CreateRotation(angle) * virtualUserTransform.forward;
            //Vector3 sampleForward = Quaternion.AngleAxis(angle, Vector3.up) * simulationController.GetUserVirtualTransform().forward;
            Vector2 userPosition = virtualUserTransform.localPosition;

            tempPosition = userPosition + sampleForward * distance; // local 좌표계에서 절대 위치 기준
        } while (!virtualSpace.IsInside(tempPosition));

        currentTargetPosition = tempPosition;
    }
}
