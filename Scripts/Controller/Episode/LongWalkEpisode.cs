using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongWalkEpisode : Episode
{
    public LongWalkEpisode() : base() { }

    public LongWalkEpisode(int episodelength) : base(episodelength) { }

    protected override void GenerateEpisode() {
        float angle = 0;
        float distance = 5.0f;

        Vector3 sampleForward = Quaternion.AngleAxis(angle, Vector3.up) * simulationController.GetUserVirtualTransform().forward;
        Vector3 userPosition = simulationController.GetUserVirtualTransform().localPosition;

        currentTargetPosition = Utility.Cast3Dto2D(userPosition + sampleForward * distance);
    }
}
