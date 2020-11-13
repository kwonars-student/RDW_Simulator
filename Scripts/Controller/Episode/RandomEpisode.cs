using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEpisode : Episode
{
    public RandomEpisode() : base() { }

    public RandomEpisode(int episodelength) : base(episodelength) { }

    protected override void GenerateEpisode() {

        Vector2 tempPosition = Vector2.zero;

        do {
            float angle = Utility.sampleUniform(-180.0f, 180.0f);
            float distance = Utility.sampleUniform(2.0f, 5.0f);

            Vector3 sampleForward = Quaternion.AngleAxis(angle, Vector3.up) * simulationController.GetUserVirtualTransform().forward;
            Vector3 userPosition = simulationController.GetUserVirtualTransform().localPosition;

            tempPosition = Utility.Cast3Dto2D(userPosition + sampleForward * distance); // local 좌표계에서 절대 위치 기준
        } while (!virtualSpace.space.IsInside(tempPosition));

        currentTargetPosition = tempPosition;
    }
}
