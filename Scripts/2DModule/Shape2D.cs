using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shape2D
{
    protected Transform2D transform;

    protected Shape2D() {
        transform = new Transform2D();
    }

    protected Shape2D(Transform2D transform)
    {
        this.transform = new Transform2D(transform);
    }

    protected Shape2D(Vector2 localPosition, float localRotation = 0.0f, Vector2 localScale = new Vector2(), Transform2D parent = null) {
        transform = new Transform2D(localPosition, localRotation, localScale, parent);
    }

    public abstract bool IsInside(Vector2 point);
}
