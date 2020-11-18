using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSegment2D : Object2D
{
    private Vector2 p1, p2; // local 좌표 기준

    public LineSegment2D() : base()
    {
        this.p1 = Vector2.zero;
        this.p2 = Vector2.zero;
    }

    public LineSegment2D(LineSegment2D lineSegment) : base(lineSegment)
    {
        this.p1 = lineSegment.p1;
        this.p2 = lineSegment.p2;
    }

    public LineSegment2D(Vector2 p1, Vector2 p2, Transform2D parent = null) : base(Vector2.zero, parent)
    {
        this.p1 = p1;
        this.p2 = p2;
    }

    public LineSegment2D(Vector2 p1, Vector2 p2, Vector2 localPosition, Transform2D parent = null) : base(localPosition, parent)
    {
        this.p1 = p1;
        this.p2 = p2;
    }

    public override string ToString()
    {
        return string.Format("p1: {0}, p2: {1}", p1, p2);
    }

    public override bool IsIntersect(Object2D geometry, float epsilon = 0)
    {
        if(geometry is LineSegment2D)
        {
            LineSegment2D otherLine = (LineSegment2D)geometry;
            
            // 두 object2D가 서로 다른 좌표계를 가지므로 global 좌표계로 통일
            Vector2 p1 = transform.TransformPoint(this.p1), p2 = transform.TransformPoint(this.p2);
            Vector2 p3 = otherLine.transform.TransformPoint(otherLine.p1), p4 = otherLine.transform.TransformPoint(otherLine.p2);

            float under = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);

            if (under == 0)
            {
                return false;
            }

            float t = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / under; // this line
            float s = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / under; // other line

            if (t < 0.0 || t > 1.0 || s < 0.0 || s > 1.0)
            {
                return false;
            }

            return true;
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }

    //public bool IsIntersect(LineSegment2D line, out Vector2 result)
    //{
    //    Vector2 p3 = line.p1, p4 = line.p2;
    //    float under = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);

    //    if (under == 0)
    //    {
    //        result = Vector2.positiveInfinity;
    //        return false;
    //    }

    //    float t = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / under; // this line
    //    float s = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / under; // other line

    //    if (t < 0.0 || t > 1.0 || s < 0.0 || s > 1.0)
    //    {
    //        result = Vector2.positiveInfinity;
    //        return false;
    //    }


    //    result = new Vector2(p1.x + t * (p2.x - p1.x), p1.y + t * (p2.y - p1.y));
    //    return true;
    //}

    public bool IsIntersect(Ray2D ray, out Vector2 result, string option = "default", float epsilon = 0)
    {
        Vector2 p3 = ray.origin, p4 = ray.origin + ray.direction;
        float under = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);

        if (under == 0)
        {
            result = Vector2.positiveInfinity;
            return false;
        }

        float t = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / under; // this line
        float s = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / under; // other ray

        if (option == "exclude")
        {
            if((t < 0.0 || t >= 1.0 || s <= 0.0))
            {
                result = Vector2.positiveInfinity;
                return false;
            }
        }
        else
        {
            if (t < 0.0 || t > 1.0 || s < 0.0)
            {
                result = Vector2.positiveInfinity;
                return false;
            }
        }

        //if (option == "exclude" && (t < 0.0 || t >= 1.0 || s <= 0.0)) // exclude last point of this line(p2)
        //{
        //    result = Vector2.positiveInfinity;
        //    return false;
        //}
        //else if (t < 0.0 || t > 1.0 || s < 0.0)
        //{
        //    result = Vector2.positiveInfinity;
        //    return false;
        //}

        result = new Vector2(p1.x + t * (p2.x - p1.x), p1.y + t * (p2.y - p1.y));
        return true;
    }
}
