using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class Transform2D
{
    public Transform2D parent;
    public Vector2 localPosition, position;
    public float localRotation, rotation;
    public Vector2 localScale, scale;

    public Transform2D() { // default constructor
        parent = null;
        position = Vector2.zero;
        rotation = 0.0f;
        scale = Vector2.one;
        localPosition = Vector2.zero;
        localRotation = 0.0f;
        localScale = Vector2.one;
    }

    public Transform2D(Transform2D transform) // copy constructor
    {
        parent = new Transform2D(transform.parent);
        position = transform.position;
        rotation = transform.rotation;
        scale = transform.scale;
        localPosition = transform.localPosition;
        localRotation = transform.localRotation;
        localScale = transform.localScale;
    }

    public Transform2D(Vector2 localPosition, float localRotation, Vector2 localScale, Transform2D parent = null)
    {
        this.localPosition = localPosition;
        this.localRotation = localRotation;
        this.localScale = localScale;
        this.parent = parent;

        if (this.parent != null)
        {
            this.position = this.localPosition + this.parent.position;
            this.rotation = this.localRotation + this.parent.rotation;
            this.scale = this.localScale * this.parent.scale;
        }
        else
        {
            this.position = this.localPosition;
            this.rotation = this.localRotation;
            this.scale = this.localScale ;
        }
    }

    public override string ToString()
    {
        return string.Format("position: {0}, rotation: {1}, scale: {2}\nlocalPosition: {3}, localRotation: {4}, localScale: {5}", position, rotation, scale, localPosition, localRotation, localScale);
    }
}
