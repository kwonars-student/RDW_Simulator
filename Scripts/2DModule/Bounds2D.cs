using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds2D
{
    public Bounds bounds;

    public Bounds2D(Bounds bounds)
    {
        this.bounds = bounds;
    }

    public Bounds2D(Vector2 center, Vector2 size)
    {
        bounds = new Bounds(Utility.CastVector2Dto3D(center), Utility.CastVector2Dto3D(size));
    }

    public Vector2 center
    {
        get { return Utility.CastVector3Dto2D(bounds.center); }
        set
        {
            bounds.center = Utility.CastVector2Dto3D(value);
        }
    }

    public Vector2 extents
    {
        get { return Utility.CastVector3Dto2D(bounds.extents); }
        set
        {
            bounds.extents = Utility.CastVector2Dto3D(value);
        }
    }

    public Vector2 max
    {
        get { return Utility.CastVector3Dto2D(bounds.max); }
        set
        {
            bounds.max = Utility.CastVector2Dto3D(value);
        }
    }

    public Vector2 min
    {
        get { return Utility.CastVector3Dto2D(bounds.min); }
        set
        {
            bounds.min = Utility.CastVector2Dto3D(value);
        }
    }

    public Vector2 size
    {
        get { return Utility.CastVector3Dto2D(bounds.size); }
        set
        {
            bounds.size = Utility.CastVector2Dto3D(value);
        }
    }

    public Vector2 GetVertex(int index)
    {
        int realIndex = Utility.mod(index, 4);

        switch (realIndex)
        {
            case 0:
                return this.max;
            case 1:
                return new Vector2(center.x - extents.x, center.y + extents.y);
            case 2:
                return this.min;
            case 3:
                return new Vector2(center.x + extents.x, center.y - extents.y);
            default:
                throw new System.NotImplementedException();
        }
    }

    public Edge2D GetEdge(int index)
    {
        return new Edge2D(GetVertex(index), GetVertex(index + 1));
    }

    public Edge2D[] GetEdges()
    {
        Edge2D[] result = new Edge2D[4];
        
        for(int i=0; i<4; i++)
        {
            result[i] = GetEdge(i);
        }

        return result;
    }

    public bool Contains(Vector2 point)
    {
        return bounds.Contains(Utility.CastVector2Dto3D(point));
    }
}
