using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongWalkEpisode : Episode
{
    public LongWalkEpisode() : base() { }

    public LongWalkEpisode(int episodeLength) : base(episodeLength) { }

    //public LongWalkEpisode(Space2D virtualSpace, int episodeLength) : base(virtualSpace, episodeLength) { }

    protected override void GenerateEpisode(Transform2D virtualUserTransform, Space2D virtualSpace) {
        float angle = 0;
        float distance = 12.0f;

        Vector2 sampleForward = Utility.rotateVector2(virtualUserTransform.forward, angle);
        Vector2 userPosition = virtualUserTransform.localPosition;

        currentTargetPosition = userPosition + sampleForward * distance;
    }
}
