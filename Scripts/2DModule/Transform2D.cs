using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Transform2D
{
    public Transform2D parent;
    private Vector2 LocalPosition;
    private float LocalRotation;
    private Vector2 LocalScale;
    private Matrix3x3 transformMatrix; // local <-> global 좌표 변환을 위한 matrix

    public Vector2 forward
    {
        get { return Matrix3x3.CreateRotation(this.rotation) * Vector2.up; }
        set
        {
            if(value.magnitude > 1)
                value = value.normalized;
            float angle = Vector2.SignedAngle(this.forward, value);
            float newRotation = this.rotation + angle;
            this.rotation = newRotation;
        }
    }

    public Vector2 localPosition
    {
        get { return this.LocalPosition; }
        set {
            this.LocalPosition = value;
            SetTransformMatrix();
        }
    }

    public Vector2 position
    {
        get { return (this.parent != null) ? this.parent.transformMatrix * this.localPosition : this.localPosition; }
        set {
            if (this.parent != null)
                this.localPosition = this.parent.transformMatrix.Invert() * (value); 
            else
                this.localPosition = value;
        }
    }

    public float localRotation
    {
        get { return this.LocalRotation; }
        set {
            this.LocalRotation = value;
            SetTransformMatrix();
        }
    }

    public float rotation
    {
        get { return (this.parent != null) ? this.parent.rotation + this.localRotation : this.localRotation; }
        set {
            if (this.parent != null)
                this.localRotation = value - this.parent.rotation;
            else
                this.localRotation = value;
        }
    }

    public Vector2 localScale
    {
        get { return this.LocalScale; }
        set {
            this.LocalScale = value;
            SetTransformMatrix();
        }
    }

    public Vector2 scale
    {
        get { return (this.parent != null) ? this.parent.scale * this.localScale : this.localScale; }
        set {
            if (this.parent != null)
                this.localScale = value / this.parent.scale;
            else
                this.localScale = value;
        }
    }

    public Transform2D() // default constructor
    {
        this.localPosition = Vector2.zero;
        this.localRotation = 0.0f;
        this.localScale = Vector2.one;
        this.forward = Vector2.up;
        this.parent = null;

        SetTransformMatrix();
    }

    public Transform2D(Transform2D transform) // copy constructor
    {
        this.localPosition = transform.localPosition;
        this.localRotation = transform.localRotation;
        this.localScale = transform.localScale;
        this.parent = (transform.parent != null) ? new Transform2D(transform.parent) : null;
        this.position = transform.position;
        this.rotation = transform.rotation;
        this.scale = transform.scale;
        this.forward = transform.forward;
        this.transformMatrix = transform.transformMatrix;
    }

    public Transform2D(Vector2 localPosition, Transform2D parent = null)
    {
        this.localPosition = localPosition;
        this.localRotation = 0.0f;
        this.localScale = Vector2.one;
        this.forward = Vector2.up;
        this.parent = parent;

        SetTransformMatrix();
    }

    public Transform2D(Vector2 localPosition, float localRotation, Transform2D parent = null)
    {
        this.localPosition = localPosition;
        this.localRotation = localRotation;
        this.localScale = Vector2.one;
        this.parent = parent;

        SetTransformMatrix();
    }

    public Transform2D(Vector2 localPosition, float localRotation, Vector2 localScale, Transform2D parent = null)
    {
        this.localPosition = localPosition;
        this.localRotation = localRotation;
        this.localScale = localScale;
        this.parent = parent;

        SetTransformMatrix();
    }

    private void SetTransformMatrix()
    {
        Matrix3x3 localTransformMatrix = Matrix3x3.CreateTRS(this.LocalPosition, this.LocalRotation, this.LocalScale);

        if (this.parent != null)
            this.transformMatrix = this.parent.transformMatrix * localTransformMatrix; // 1번식
        else
            this.transformMatrix = localTransformMatrix;

        //this.forward = Matrix3x3.CreateRotation(rotation) * Vector2.up;
    }


    public void Translate(Vector2 translation, Space relativeTo=Space.Self)
    {
        if(relativeTo == Space.World)
        {
            Vector2 newPosition = this.position + translation;
            this.position = newPosition;
        }
        else if(relativeTo == Space.Self)
        {
            Matrix3x3 rotationMatrix = Matrix3x3.CreateRotation(this.localRotation);
            Vector2 newPosition = this.localPosition + rotationMatrix * translation;
            this.localPosition = newPosition;
        }


    }

    public void Rotate(float degree, Space relativeTo = Space.Self)
    {
        if (relativeTo == Space.World)
        {
            float newRotation = this.rotation + degree;
            this.rotation = newRotation;
        }
        else if (relativeTo == Space.Self) {
            float newRotation = this.localRotation + degree;
            this.localRotation = newRotation;
        }
    }

    public override string ToString()
    {
        return string.Format("position: {0}, rotation: {1}, scale: {2}\n" +
            "localPosition: {3}, localRotation: {4}, localScale: {5}\n" +
            "forward: {6}", position, rotation, scale, localPosition, localRotation, localScale, forward);
    }
}
