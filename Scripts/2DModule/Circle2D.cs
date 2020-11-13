using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Circle2D : Object2D
{
    private float radius;

    public Circle2D() : base()
    {
        this.radius = 0.0f;
    }

    public Circle2D(Circle2D circle) : base(circle)
    {
        this.radius = circle.radius;
    }

    public Circle2D(float radius) : base()
    {
        this.radius = radius;
    }

    public Circle2D(float radius, Vector2 localPosition, Transform2D parent=null) : base(localPosition, parent)
    {
        this.radius = radius;
    }

    public float GetRadius()
    {
        return radius;
    }

    public override bool IsIntersect(Object2D geometry)
    {
        if(geometry is Circle2D)
        {
            Circle2D other = (Circle2D) geometry;
            Vector2 otherPosition = other.transform.position;
            Vector2 thisPosition = this.transform.position;
            float otherRadius = other.GetRadius();

            if (Vector2.Distance(thisPosition, otherPosition) <= otherRadius + this.radius)
                return true;
            else
                return false;
        }
        else if(geometry is Polygon2D)
        {
            throw new System.NotImplementedException();
        }
        else if(geometry is LineSegment2D)
        {
            throw new System.NotImplementedException();
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }
}
