using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle2D : Shape2D
{
    float radius;

    public Circle2D()
    {
        radius = 0;
    }

    public Circle2D(float radius, Transform2D transform) 
        : base(transform) {
        this.radius = radius;
    }

    public Circle2D(float radius, Vector2 localPosition, float localRotation = 0.0f, Vector2 localScale = new Vector2(), Transform2D parent = null) 
        : base(localPosition, localRotation, localScale, parent) {
        this.radius = radius;
    }

    public override bool IsInside(Vector2 point) // point는 로컬 좌표계 기준
    {
        if (point.magnitude < radius)
            return true;
        else
            return false;
    }
}
