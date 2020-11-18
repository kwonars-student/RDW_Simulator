using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Polygon2D : Object2D
{
    private List<Vector2> vertices; // local 좌표계 기준

    public Polygon2D() : base()
    {
        this.vertices = new List<Vector2>();
    }

    public Polygon2D(Polygon2D polygon) : base(polygon)
    {
        this.vertices = new List<Vector2>(polygon.vertices);
    }

    public Polygon2D(List<Vector2> vertices) : base()
    {
        this.vertices = new List<Vector2>(vertices);
    }

    public Polygon2D(int count, float size) : base()
    {
        // TODO: 정수 N에 대해서 정N각형을 만들도록 구현
        // 현재 count 값은 무시됨
        vertices = new List<Vector2>();
        vertices.Add(new Vector2(size / 2, size / 2));
        vertices.Add(new Vector2(-size / 2, size / 2));
        vertices.Add(new Vector2(-size / 2, -size / 2));
        vertices.Add(new Vector2(size / 2, -size / 2));
    }

    public Polygon2D(List<Vector2> vertices, Vector2 localPosition, Transform2D parent = null) : base(localPosition, parent)
    {
        this.vertices = new List<Vector2>(vertices);
    }

    public Polygon2D(int count, float size, Vector2 localPosition, Transform2D parent = null) : base(localPosition, parent)
    {
        // TODO: 정수 N에 대해서 정N각형을 만들도록 구현
        // 현재 count 값은 무시됨
        vertices = new List<Vector2>();
        vertices.Add(new Vector2(size / 2, size / 2));
        vertices.Add(new Vector2(-size / 2, size / 2));
        vertices.Add(new Vector2(-size / 2, -size / 2));
        vertices.Add(new Vector2(size / 2, -size / 2));
    }

    public Polygon2D(List<Vector2> vertices, Vector2 localPosition, float localRotation, Transform2D parent = null) : base(localPosition, localRotation, parent)
    {
        this.vertices = new List<Vector2>(vertices);
    }

    public Polygon2D(int count, float size, Vector2 localPosition, float localRotation, Transform2D parent = null) : base(localPosition, localRotation, parent)
    {
        // TODO: 정수 N에 대해서 정N각형을 만들도록 구현
        // 현재 count 값은 무시됨
        vertices = new List<Vector2>();
        vertices.Add(new Vector2(size / 2, size / 2));
        vertices.Add(new Vector2(-size / 2, size / 2));
        vertices.Add(new Vector2(-size / 2, -size / 2));
        vertices.Add(new Vector2(size / 2, -size / 2));
    }

    public override string ToString()
    {
        return base.ToString() + string.Format("vertices: {0}\n", string.Join(",", vertices));
    }

    public List<Vector2> GetVertices()
    {
        return vertices;
    }

    public Vector2 GetVertex(int index)
    {
        return vertices[index];
    }

    public override bool IsIntersect(Object2D geometry, float epsilon = 0)
    {
        if(geometry is LineSegment2D)
        {
            LineSegment2D line = (LineSegment2D)geometry;
            int numOfIntersect = 0;

            for (int i = 0; i < vertices.Count; i++)
            {
                LineSegment2D boundary = new LineSegment2D(vertices[i], vertices[(i + 1) % 4], transform);

                //Vector2 result;
                if (boundary.IsIntersect(line))
                    numOfIntersect += 1;
            }

            if (numOfIntersect == 0)
                return false;
            else
                return true;
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }

    public override bool IsInside(Vector2 point, float bound = 0)
    { // point는 로컬 좌표계 기준
        float epsilon = bound;
        Vector2[] signVector =
        {
            new Vector2(1, 1),
            new Vector2(-1, 1),
            new Vector2(-1, -1),
            new Vector2(1, -1)
        };

        Ray2D ray = new Ray2D(point, Vector2.right);
        int numOfIntersect = 0;

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 p1 = vertices[i] - signVector[i] * epsilon;
            Vector2 p2 = vertices[(i + 1) % 4] - signVector[(i + 1) % 4] * epsilon;
            LineSegment2D boundary = new LineSegment2D(p1, p2);

            Vector2 result;
            if (boundary.IsIntersect(ray, out result, "exclude"))
                numOfIntersect += 1;
        }

        if (numOfIntersect % 2 == 0)
            return false;
        else
            return true;
    }
}
