using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpaceSetting
{
    public bool usePredefinedSpace;
    public bool usePredefinedOrigin;
    public Vector2 originPosition; // simulationManager local 2D 좌표계 기준
    public float originRotation;
    public Vector2 size; // n각형으로 일반화 어떻게 시키지?

    public Space2D GetSpace2D() {
        return new Space2D(size, originPosition, originRotation);
    }
}
